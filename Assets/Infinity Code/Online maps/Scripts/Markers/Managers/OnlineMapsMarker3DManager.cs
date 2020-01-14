/*     INFINITY CODE 2013-2019      */
/*   http://www.infinity-code.com   */

using System;
using UnityEngine;

/// <summary>
/// This component manages 3D markers.
/// </summary>
[Serializable]
[DisallowMultipleComponent]
public class OnlineMapsMarker3DManager : OnlineMapsMarkerManagerBase<OnlineMapsMarker3DManager, OnlineMapsMarker3D>
{
    /// <summary>
    /// Specifies whether to create a 3D marker by pressing N under the cursor.
    /// </summary>
    public bool allowAddMarker3DByN = true;

    /// <summary>
    /// Default 3D marker.
    /// </summary>
    public GameObject defaultPrefab;

    /// <summary>
    /// Create a new 3D marker
    /// </summary>
    /// <param name="longitude">Longitude</param>
    /// <param name="latitude">Latitude</param>
    /// <param name="prefab">Prefab</param>
    /// <returns>Instance of the marker</returns>
    public OnlineMapsMarker3D Create(double longitude, double latitude, GameObject prefab)
    {
        OnlineMapsMarker3D marker = _CreateItem(longitude, latitude);
        marker.prefab = prefab;
        marker.manager = this;
        marker.scale = defaultScale;
        marker.Init(map.transform);
        Redraw();
        return marker;
    }

    /// <summary>
    /// Creates a new 3D marker from an existing GameObject in the scene.
    /// </summary>
    /// <param name="longitude">Longitude</param>
    /// <param name="latitude">Latitude</param>
    /// <param name="markerGameObject">GameObject in the scene</param>
    /// <returns>Instance of the marker</returns>
    public OnlineMapsMarker3D CreateFromExistGameObject(double longitude, double latitude, GameObject markerGameObject)
    {
        OnlineMapsMarker3D marker = _CreateItem(longitude, latitude);
        marker.prefab = marker.instance = markerGameObject;
        marker.control = map.control as OnlineMapsControlBase3D;
        marker.manager = this;
        marker.scale = defaultScale;
        markerGameObject.AddComponent<OnlineMapsMarker3DInstance>().marker = marker;
        marker.inited = true;

        Update();

        if (marker.OnInitComplete != null) marker.OnInitComplete(marker);
        Redraw();
        return marker;
    }

    /// <summary>
    /// Create a new 3D marker
    /// </summary>
    /// <param name="location">Location of the marker (X - longitude, Y - latitude)</param>
    /// <param name="prefab">Prefab</param>
    /// <returns>Instance of the marker</returns>
    public static OnlineMapsMarker3D CreateItem(Vector2 location, GameObject prefab)
    {
        if (instance != null) return instance.Create(location.x, location.y, prefab);
        return null;
    }

    /// <summary>
    /// Create a new 3D marker
    /// </summary>
    /// <param name="lng">Longitude</param>
    /// <param name="lat">Latitude</param>
    /// <param name="prefab">Prefab</param>
    /// <returns>Instance of the marker</returns>
    public static OnlineMapsMarker3D CreateItem(double lng, double lat, GameObject prefab)
    {
        if (instance != null) return instance.Create(lng, lat, prefab);
        return null;
    }

    /// <summary>
    /// Creates a new 3D marker from an existing GameObject in the scene.
    /// </summary>
    /// <param name="longitude">Longitude</param>
    /// <param name="latitude">Latitude</param>
    /// <param name="markerGameObject">GameObject in the scene</param>
    /// <returns>Instance of the marker</returns>
    public static OnlineMapsMarker3D CreateItemFromExistGameObject(double longitude, double latitude, GameObject markerGameObject)
    {
        if (instance != null) return instance.CreateFromExistGameObject(longitude, latitude, markerGameObject);
        return null;
    }

    public override OnlineMapsSavableItem[] GetSavableItems()
    {
        if (savableItems != null) return savableItems;

        savableItems = new[]
        {
            new OnlineMapsSavableItem("markers3D", "3D Markers", SaveSettings)
            {
                priority = 90,
                loadCallback = LoadSettings
            }
        };

        return savableItems;
    }

    /// <summary>
    /// Load items and component settings from JSON
    /// </summary>
    /// <param name="json">JSON item</param>
    public void LoadSettings(OnlineMapsJSONItem json)
    {
        OnlineMapsJSONItem jitems = json["items"];
        RemoveAll();
        foreach (OnlineMapsJSONItem jitem in jitems)
        {
            OnlineMapsMarker3D marker = new OnlineMapsMarker3D();

            double mx = jitem.ChildValue<double>("longitude");
            double my = jitem.ChildValue<double>("latitude");

            marker.SetPosition(mx, my);

            marker.range = jitem.ChildValue<OnlineMapsRange>("range");
            marker.label = jitem.ChildValue<string>("label");
            marker.prefab = OnlineMapsUtils.GetObject(jitem.ChildValue<int>("prefab")) as GameObject;
            marker.rotationY = jitem.ChildValue<float>("rotationY");
            marker.scale = jitem.ChildValue<float>("scale");
            marker.enabled = jitem.ChildValue<bool>("enabled");
            Add(marker);
        }

        (json["settings"] as OnlineMapsJSONObject).DeserializeObject(this);
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        foreach (OnlineMapsMarker3D item in _items) item.DestroyInstance();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        OnlineMapsControlBase3D control = map.control as OnlineMapsControlBase3D;
        if (control != null)
        {
            foreach (OnlineMapsMarker3D item in _items)
            {
                bool isFirstStart = item.manager == null;
                item.manager = this;
                item.control = control;
                if (!isFirstStart) item.Update();
            }
        }
    }

    protected override OnlineMapsJSONItem SaveSettings()
    {
        OnlineMapsJSONItem jitem = base.SaveSettings();
        jitem["settings"].AppendObject(new
        {
            allowAddMarker3DByN,
            defaultPrefab = defaultPrefab != null? defaultPrefab.GetInstanceID(): -1,
            defaultScale
        });
        return jitem;
    }

    protected override void Update()
    {
        base.Update();

        if (allowAddMarker3DByN && Input.GetKeyUp(KeyCode.N))
        {
            double lng, lat;
            if (map.control.GetCoords(out lng, out lat))
            {
                OnlineMapsMarker3D marker3D = Create(lng, lat, defaultPrefab);
                marker3D.scale = defaultScale;
            }
        }
    }
}
 