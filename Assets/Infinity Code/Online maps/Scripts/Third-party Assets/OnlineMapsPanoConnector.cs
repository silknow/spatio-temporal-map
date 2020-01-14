/*     INFINITY CODE 2013-2019      */
/*   http://www.infinity-code.com   */

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UPANO
using InfinityCode.uPano;
using InfinityCode.uPano.Controls;
using InfinityCode.uPano.Plugins;
using InfinityCode.uPano.Renderers;
using InfinityCode.uPano.Requests;
#endif

/// <summary>
/// Plugin for displaying Google Street View panoramas using uPano.
/// </summary>
[AddComponentMenu("Infinity Code/Online Maps/Plugins/uPano Connector")]
[OnlineMapsPlugin("uPano Connector", typeof(OnlineMapsControlBase))]
public class OnlineMapsPanoConnector : MonoBehaviour
{
    private const string overlayURL = "https://maps.google.com/maps/vt?pb=!1m5!1m4!1i{0}!2i{1}!3i{2}!4i256!2m8!1e2!2ssvv!4m2!1scb_client!2sapiv3!4m2!1scc!2s*211m3*211e3*212b1*213e2*211m3*211e2*212b1*213e2!3m5!3sUS!12m1!1e40!12m1!1e18";

    /// <summary>
    /// Google API key
    /// </summary>
    public string googleApiKey = "";

    /// <summary>
    /// The minimum zoom for which overlay will be displayed
    /// </summary>
    public int minZoom = 10;

    /// <summary>
    /// Use progressive loading panoramas (zoom - 0, 1, 2, 3).
    /// </summary>
    public bool usePreview = true;

    /// <summary>
    /// Check the color of the overlay texture before opening the panorama.
    /// </summary>
    public bool checkOverlayColor = true;

    /// <summary>
    /// Overlay layer will be drawn
    /// </summary>
    public OverlayLayer overlayLayer = OverlayLayer.front;

    /// <summary>
    /// Prefab of UI button to close the panorama
    /// </summary>
    [Header("Panorama close settings")]
    public GameObject closeButtonPrefab;

    /// <summary>
    /// Hot key to close the panorama
    /// </summary>
    public KeyCode closeByKeyCode = KeyCode.Escape;

    /// <summary>
    /// Radius of the panorama sphere
    /// </summary>
    [Header("Panorama mesh settings")]
    public int radius = 10;

    /// <summary>
    /// Number of segments of the panorama sphere
    /// </summary>
    public int segments = 32;

    private OnlineMaps map;
    private Dictionary<ulong, Texture2D> overlays;
    private GameObject closeButtonInstance;

#if UPANO
    private SphericalPanoRenderer panoRenderer;
    private GoogleStreetViewRequest currentRequest;
    private int zoom = 3;
    private double lng, lat;

    private bool CheckOverlayColor(double clng, double clat)
    {
        double tx, ty;
        map.projection.CoordinatesToTile(clng, clat, map.zoom, out tx, out ty);
        OnlineMapsTile tile = map.tileManager.GetTile(map.zoom, (int) tx, (int) ty);
        if (tile == null) return false;
        while (tile.status != OnlineMapsTileStatus.loaded)
        {
            tile = tile.parent;
            if (tile == null) return false;
        }

        Texture2D overlay = null;

        if (overlayLayer == OverlayLayer.front) overlay = tile.overlayFrontTexture;
        else if (overlayLayer == OverlayLayer.back) overlay = tile.overlayBackTexture;
        else if (overlayLayer == OverlayLayer.traffic) overlay = (tile as OnlineMapsRasterTile).trafficTexture;

        if (overlay == null) return false;

        int px = (int) Math.Round((tx - (int) tx) * 255);
        int py = 255 - (int) Math.Round((ty - (int) ty) * 255);

        for (int x = Mathf.Max(0, px - 2); x < Mathf.Min(255, px + 3); x++)
        {
            for (int y = Mathf.Max(0, py - 2); y < Mathf.Min(255, py + 3); y++)
            {
                if (overlay.GetPixel(x, y) != Color.clear) return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Closes the active instance of the panorama.
    /// </summary>
    public void Close()
    {
        if (panoRenderer != null)
        {
            if (panoRenderer.texture != null) Destroy(panoRenderer.texture);

            Destroy(panoRenderer.gameObject);
            panoRenderer = null;
        }

        if (closeButtonInstance != null)
        {
            Destroy(closeButtonInstance);
            closeButtonInstance = null;
        }

        if (currentRequest != null)
        {
            currentRequest.Dispose();
            currentRequest = null;
        }

        if (map != null) map.control.allowUserControl = true;
    }

    private void OnDisable()
    {
        foreach (OnlineMapsTile tile in map.tileManager.tiles) SetOverlay(tile as OnlineMapsRasterTile, null);

        if (map != null) map.Redraw();
    }

    private void OnEnable()
    {
        if (string.IsNullOrEmpty(googleApiKey))
        {
            Debug.LogWarning("Please specify Online Maps Pano Connector/Google Api Key!!!");
            Destroy(this);
            return;
        }

        if (overlays == null) overlays = new Dictionary<ulong, Texture2D>();

        foreach (KeyValuePair<ulong, Texture2D> pair in overlays)
        {
            if (!map.tileManager.dTiles.ContainsKey(pair.Key)) continue;
            SetOverlay(map.tileManager.dTiles[pair.Key] as OnlineMapsRasterTile, pair.Value);
        }

        if (map != null) map.Redraw();
    }

    private void OnGoogleStreetViewSuccess(GoogleStreetViewRequest request)
    {
        string panoID = request.panoID;
        currentRequest = null;
        if (!request.hasErrors)
        {
            if (panoRenderer == null)
            {
                map.control.allowUserControl = false;
                panoRenderer = SphericalPanoRenderer.CreateSphere(request.texture, radius, segments);
                panoRenderer.shader = Shader.Find("Unlit/Texture");
                panoRenderer.gameObject.AddComponent<KeyboardControl>();
                panoRenderer.gameObject.AddComponent<MouseControl>();
                panoRenderer.gameObject.AddComponent<Limits>();

                if (closeButtonPrefab != null && closeButtonInstance == null)
                {
                    Canvas canvas = CanvasUtils.GetCanvas();
                    if (canvas != null)
                    {
                        closeButtonInstance = Instantiate(closeButtonPrefab);
                        closeButtonInstance.transform.SetParent(canvas.transform, false);
                        closeButtonInstance.GetComponentInChildren<Button>().onClick.AddListener(Close);
                    }
                }
            }
            else
            {
                if (panoRenderer.texture != null) Destroy(panoRenderer.texture);
                panoRenderer.texture = request.texture;
            }
        }
        else Debug.Log(request.error);

        if (zoom < 3)
        {
            zoom++;
            currentRequest = new GoogleStreetViewRequest(googleApiKey, panoID, zoom, panoRenderer.texture as Texture2D);
            currentRequest.OnComplete += OnGoogleStreetViewSuccess;
        }
    }

    private void OnMapClick()
    {
        if (!enabled) return;
        if (map.zoom < minZoom) return;

        double clng, clat;
        map.control.GetCoords(out clng, out clat);

        if (checkOverlayColor && !CheckOverlayColor(clng, clat)) return;

        if (currentRequest != null)
        {
            currentRequest.Dispose();
            currentRequest = null;
        }

        lng = clng;
        lat = clat;

        int zoom = usePreview ? 0 : 3;
        currentRequest = new GoogleStreetViewRequest(googleApiKey, lng, lat, zoom);
        currentRequest.OnComplete += OnGoogleStreetViewSuccess;
        this.zoom = zoom;
    }

    private void OnPanoDestroy(Pano pano)
    {
        Close();
    }

    private void OnStartDownloadTile(OnlineMapsTile tile)
    {
        OnlineMapsTileManager.StartDownloadTile(tile);
        StartDownloadOverlay(tile);
    }

    private void OnTileDisposed(OnlineMapsTile tile)
    {
        if (!overlays.ContainsKey(tile.key)) return;
        if (overlays[tile.key] != null) OnlineMapsUtils.Destroy(overlays[tile.key]);
        overlays.Remove(tile.key);
    }

    private void SetOverlay(OnlineMapsRasterTile tile, Texture2D texture)
    {
        if (overlayLayer == OverlayLayer.front) tile.overlayFrontTexture = texture;
        else if (overlayLayer == OverlayLayer.back) tile.overlayBackTexture = texture;
        else if (overlayLayer == OverlayLayer.traffic)
        {
            if (map.control.resultIsTexture)
            {
                if (tile.SetLabelData(texture.EncodeToPNG())) map.buffer.ApplyTile(tile);
            }
            else tile.trafficTexture = texture;
        }
    }

    private void Start()
    {
        map = GetComponent<OnlineMaps>();

        if (map.control.resultIsTexture && overlayLayer != OverlayLayer.traffic)
        {
            Debug.LogWarning("This Control only supports Overlay Layer - Traffic.");
            overlayLayer = OverlayLayer.traffic;
        }
        
        if (OnlineMapsCache.instance != null) OnlineMapsCache.instance.OnLoadedFromCache += StartDownloadOverlay;

        OnlineMapsTileManager.OnStartDownloadTile += OnStartDownloadTile;

        map.control.OnMapClick += OnMapClick;
        Pano.OnPanoDestroy += OnPanoDestroy;
    }

    private void StartDownloadOverlay(OnlineMapsTile tile)
    {
        if (tile.zoom < minZoom) return;
        if (overlays.ContainsKey(tile.key)) return;

        string url = string.Format(overlayURL, tile.zoom, tile.x, tile.y);
        OnlineMapsWWW www = new OnlineMapsWWW(url);
        www.OnComplete += delegate
        {
            if (tile.status == OnlineMapsTileStatus.disposed) return;

            Texture2D texture = new Texture2D(256, 256, TextureFormat.ARGB32, map.control.mipmapForTiles);
            www.LoadImageIntoTexture(texture);
            texture.wrapMode = TextureWrapMode.Clamp;
            overlays.Add(tile.key, texture);
            tile.OnDisposed += OnTileDisposed;

            if (enabled)
            {
                SetOverlay(tile as OnlineMapsRasterTile, texture);
                map.Redraw();
            }
        };
    }

    private void Update()
    {
        if (Input.GetKeyDown(closeByKeyCode))
        {
            Close();
        }
    }

#endif

    public enum OverlayLayer
    {
        front,
        back,
        traffic
    }
}
