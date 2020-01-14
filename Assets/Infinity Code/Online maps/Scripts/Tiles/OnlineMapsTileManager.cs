/*     INFINITY CODE 2013-2019      */
/*   http://www.infinity-code.com   */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// Manages map tiles
/// </summary>
public class OnlineMapsTileManager
{
    /// <summary>
    /// The maximum number simultaneously downloading tiles.
    /// </summary>
    public static int maxTileDownloads = 5;

    public static Action<OnlineMapsTile> OnLoadFromCache;

    /// <summary>
    /// The event occurs after generating buffer and before update control to preload tiles for tileset.
    /// </summary>
    public static Action OnPreloadTiles;

    public static Action<OnlineMapsTile> OnPrepareDownloadTile;

    /// <summary>
    /// An event that occurs when loading the tile. Allows you to intercept of loading tile, and load it yourself.
    /// </summary>
    public static Action<OnlineMapsTile> OnStartDownloadTile;

    /// <summary>
    /// This event is occurs when a tile is loaded.
    /// </summary>
    public static Action<OnlineMapsTile> OnTileLoaded;

    private OnlineMapsTile[] downloadTiles;
    private Dictionary<ulong, OnlineMapsTile> _dtiles;
    private OnlineMaps _map;
    private List<OnlineMapsTile> _tiles;
    private List<OnlineMapsTile> unusedTiles;

    public Dictionary<ulong, OnlineMapsTile> dTiles
    {
        get { return _dtiles; }
    }

    public List<OnlineMapsTile> tiles
    {
        get { return _tiles; }
        set { _tiles = value; }
    }

    public OnlineMaps map
    {
        get { return _map; }
    }

    public OnlineMapsTileManager(OnlineMaps map)
    {
        _map = map;
        unusedTiles = new List<OnlineMapsTile>();
        _tiles = new List<OnlineMapsTile>();
        _dtiles = new Dictionary<ulong, OnlineMapsTile>();
    }

    public void Add(OnlineMapsTile tile)
    {
        tiles.Add(tile);
        if (dTiles.ContainsKey(tile.key)) dTiles[tile.key] = tile;
        else dTiles.Add(tile.key, tile);
    }

    public void Dispose()
    {
        foreach (OnlineMapsTile tile in tiles) tile.Dispose();

        _map = null;
        _dtiles = null;
        _tiles = null;
    }

    /// <summary>
    /// Gets a tile for zoom, x, y.
    /// </summary>
    /// <param name="zoom">Tile zoom</param>
    /// <param name="x">Tile X</param>
    /// <param name="y">Tile Y</param>
    /// <returns>Tile or null</returns>
    public OnlineMapsTile GetTile(int zoom, int x, int y)
    {
        ulong key = GetTileKey(zoom, x, y);
        if (dTiles.ContainsKey(key))
        {
            OnlineMapsTile tile = dTiles[key];
            if (tile.status != OnlineMapsTileStatus.disposed) return tile;
        }
        return null;
    }

    /// <summary>
    /// Gets a tile for zoom, x, y.
    /// </summary>
    /// <param name="zoom">Tile zoom</param>
    /// <param name="x">Tile X</param>
    /// <param name="y">Tile Y</param>
    /// <param name="tile">Tile</param>
    /// <returns>True - success, false - otherwise</returns>
    public bool GetTile(int zoom, int x, int y, out OnlineMapsTile tile)
    {
        tile = null;
        ulong key = GetTileKey(zoom, x, y);
        OnlineMapsTile t;
        if (dTiles.TryGetValue(key, out t))
        {
            if (t.status != OnlineMapsTileStatus.disposed)
            {
                tile = t;
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Gets a tile key for zoom, x, y.
    /// </summary>
    /// <param name="zoom">Tile zoom</param>
    /// <param name="x">Tile X</param>
    /// <param name="y">Tile Y</param>
    /// <returns>Tile key</returns>
    public static ulong GetTileKey(int zoom, int x, int y)
    {
        return ((ulong)zoom << 58) + ((ulong)x << 29) + (ulong)y;
    }

    private static void OnTileWWWComplete(OnlineMapsWWW www)
    {
        OnlineMapsTile tile = www["tile"] as OnlineMapsTile;

        if (tile == null) return;
        tile.LoadFromWWW(www);
    }

    public static void OnTrafficWWWComplete(OnlineMapsWWW www)
    {
        OnlineMapsRasterTile tile = www["tile"] as OnlineMapsRasterTile;

        if (tile == null) return;
        if (tile.trafficWWW == null || !tile.trafficWWW.isDone) return;

        if (tile.status == OnlineMapsTileStatus.disposed)
        {
            tile.trafficWWW = null;
            return;
        }

        if (!www.hasError)
        {
            if (tile.map.control.resultIsTexture)
            {
                if (tile.OnLabelDownloadComplete()) tile.map.buffer.ApplyTile(tile);
            }
            else if (tile.trafficWWW != null && tile.map.traffic)
            {
                Texture2D trafficTexture = new Texture2D(256, 256, TextureFormat.ARGB32, false)
                {
                    wrapMode = TextureWrapMode.Clamp
                };
                if (tile.map.useSoftwareJPEGDecoder) OnlineMapsRasterTile.LoadTexture(trafficTexture, www.bytes);
                else tile.trafficWWW.LoadImageIntoTexture(trafficTexture);
                tile.trafficTexture = trafficTexture;
            }

            if (OnlineMapsTile.OnTrafficDownloaded != null) OnlineMapsTile.OnTrafficDownloaded(tile);

            tile.map.Redraw();
        }

        tile.trafficWWW = null;
    }

    public void Remove(OnlineMapsTile tile)
    {
        unusedTiles.Add(tile);
        if (_dtiles.ContainsKey(tile.key)) _dtiles.Remove(tile.key);
    }

    public void Reset()
    {
        foreach (OnlineMapsTile tile in tiles) tile.Dispose();
        tiles.Clear();
        dTiles.Clear();
    }

    public void StartDownloading()
    {
        if (tiles == null) return;
        float startTime = Time.realtimeSinceStartup;

        int countDownload = 0;
        int c = 0;

        lock (OnlineMapsTile.lockTiles)
        {
            for (int i = 0; i < tiles.Count; i++)
            {
                OnlineMapsTile tile = tiles[i];
                if (tile.status == OnlineMapsTileStatus.loading && tile.www != null)
                {
                    countDownload++;
                    if (countDownload >= maxTileDownloads) return;
                }
            }

            int needDownload = maxTileDownloads - countDownload;

            if (downloadTiles == null) downloadTiles = new OnlineMapsTile[maxTileDownloads];

            for (int i = 0; i < tiles.Count; i++)
            {
                OnlineMapsTile tile = tiles[i];
                if (tile.status != OnlineMapsTileStatus.none) continue;

                if (c == 0)
                {
                    downloadTiles[0] = tile;
                    c++;
                }
                else
                {
                    int index = c;
                    int index2 = index - 1;

                    while (index2 >= 0)
                    {
                        if (downloadTiles[index2].zoom <= tile.zoom) break;

                        index2--;
                        index--;
                    }

                    if (index < needDownload)
                    {
                        for (int j = needDownload - 1; j > index; j--) downloadTiles[j] = downloadTiles[j - 1];
                        downloadTiles[index] = tile;
                        if (c < needDownload) c++;
                    }
                }
            }
        }

        for (int i = 0; i < c; i++)
        {
            if (Time.realtimeSinceStartup - startTime > 0.02) break;
            OnlineMapsTile tile = downloadTiles[i];

            countDownload++;
            if (countDownload > maxTileDownloads) break;

            if (OnPrepareDownloadTile != null) OnPrepareDownloadTile(tile);

            if (OnLoadFromCache != null) OnLoadFromCache(tile);
            else if (OnStartDownloadTile != null) OnStartDownloadTile(tile);
            else StartDownloadTile(tile);
        }
    }

    /// <summary>
    /// Starts dowloading of specified tile.
    /// </summary>
    /// <param name="tile">Tile to be downloaded.</param>
    public static void StartDownloadTile(OnlineMapsTile tile)
    {
        tile.status = OnlineMapsTileStatus.loading;
        tile.map.StartCoroutine(StartDownloadTileAsync(tile));
    }

    private static IEnumerator StartDownloadTileAsync(OnlineMapsTile tile)
    {
        bool loadOnline = true;

        if (tile.map.source != OnlineMapsSource.Online)
        {
            ResourceRequest resourceRequest = Resources.LoadAsync(tile.resourcesPath);
            yield return resourceRequest;

            if (tile.map == null)
            {
                tile.MarkError();
                yield break;
            }

            Texture2D texture = resourceRequest.asset as Texture2D;

            if (texture != null)
            {
                texture.wrapMode = TextureWrapMode.Clamp;
                if (tile.map.control.resultIsTexture)
                {
                    (tile as OnlineMapsRasterTile).ApplyTexture(texture);
                    tile.map.buffer.ApplyTile(tile);
                    OnlineMapsUtils.Destroy(texture);
                }
                else
                {
                    tile.texture = texture;
                    tile.status = OnlineMapsTileStatus.loaded;
                }
                tile.MarkLoaded();
                tile.map.Redraw();
                loadOnline = false;
            }
            else if (tile.map.source == OnlineMapsSource.Resources)
            {
                tile.MarkError();
                yield break;
            }
        }

        if (loadOnline)
        {
            if (tile.www != null)
            {
                Debug.Log("tile has www " + tile + "   " + tile.status);
                yield break;
            }

            tile.www = new OnlineMapsWWW(tile.url);
            tile.www["tile"] = tile;
            tile.www.OnComplete += OnTileWWWComplete;
            tile.status = OnlineMapsTileStatus.loading;
        }

        OnlineMapsRasterTile rTile = tile as OnlineMapsRasterTile;

        if (tile.map.traffic && !string.IsNullOrEmpty(rTile.trafficURL))
        {
            rTile.trafficWWW = new OnlineMapsWWW(rTile.trafficURL);
            rTile.trafficWWW["tile"] = tile;
            rTile.trafficWWW.OnComplete += OnTrafficWWWComplete;
        }
    }

    public void UnloadUnusedTiles()
    {
        if (unusedTiles == null) return;

        for (int i = 0; i < unusedTiles.Count; i++) unusedTiles[i].Destroy();
        unusedTiles.Clear();
    }
}