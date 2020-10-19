/*         INFINITY CODE         */
/*   https://infinity-code.com   */

using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class of buildings.
/// </summary>
public abstract class OnlineMapsBuildingBase:MonoBehaviour
{
    public static List<int> roofIndices;

    protected static Shader defaultShader;
    protected static Material defaultWallMaterial;
    protected static Material defaultRoofMaterial;
    protected static List<Vector3> vertices;
    protected static List<Vector2> uvs;
    protected static List<int> wallTriangles;
    protected static List<int> roofTriangles;
    protected static List<Vector3> roofVertices;

    /// <summary>
    /// Events that occur when user click on the building.
    /// </summary>
    public Action<OnlineMapsBuildingBase> OnClick;

    /// <summary>
    /// Events that occur when dispose building.
    /// </summary>
    public Action<OnlineMapsBuildingBase> OnDispose;

    /// <summary>
    /// Events that occur when user press on the building.
    /// </summary>
    public Action<OnlineMapsBuildingBase> OnPress;

    /// <summary>
    /// Events that occur when user release on the building.
    /// </summary>
    public Action<OnlineMapsBuildingBase> OnRelease;

    /// <summary>
    /// Geographical coordinates of the boundaries of the building.
    /// </summary>
    public Bounds boundsCoords;

    /// <summary>
    /// Geographical coordinates of the center point.
    /// </summary>
    public Vector2 centerCoordinates;

    /// <summary>
    /// ID of building.
    /// </summary>
    public string id;

    public Vector2 initialSizeInScene;

    /// <summary>
    /// Zoom, in which this building was created.
    /// </summary>
    public float initialZoom;

    /// <summary>
    /// Array of building meta key-value pair.
    /// </summary>
    public MetaInfo[] metaInfo;

    /// <summary>
    /// Perimeter of building.
    /// </summary>
    public float perimeter;

    public OnlineMapsOSMWay way;
    public List<OnlineMapsOSMNode> nodes;

    private int lastTouchCount = 0;

    /// <summary>
    /// Collider of building.
    /// </summary>
    protected Collider buildingCollider;

    protected OnlineMapsBuildings container;

    /// <summary>
    /// Indicates that the building has an error.
    /// </summary>
    protected bool hasErrors = false;

    private bool isPressed;
    private Vector2 pressPoint;

    /// <summary>
    /// Checks ignore the building.
    /// </summary>
    /// <param name="way">Building way.</param>
    /// <returns>TRUE - ignore building, FALSE - generate building.</returns>
    protected static bool CheckIgnoredBuildings(OnlineMapsOSMWay way)
    {
        string buildingType = way.GetTagValue("building");
        if (buildingType == "bridge" || buildingType == "roof") return true;

        string layer = way.GetTagValue("layer");
        if (!string.IsNullOrEmpty(layer) && int.Parse(layer) < 0) return true;

        return false;
    }

    /// <summary>
    /// Creates a new child GameObject, with the specified name.
    /// </summary>
    /// <param name="container">Reference to OnlineMapsBuildings</param>
    /// <param name="id">Name of GameObject.</param>
    /// <returns></returns>
    protected static GameObject CreateGameObject(OnlineMapsBuildings container, string id)
    {
        GameObject buildingGameObject = new GameObject(id);
        buildingGameObject.SetActive(false);

        buildingGameObject.transform.parent = container.container.transform;
        buildingGameObject.layer = container.gameObject.layer;
        return buildingGameObject;
    }

    /// <summary>
    /// Dispose of building.
    /// </summary>
    public virtual void Dispose()
    {
        if (OnDispose != null) OnDispose(this);

        OnClick = null;
        OnDispose = null;
        OnPress = null;
        OnRelease = null;

        buildingCollider = null;
        container = null;
        metaInfo = null;
    }

    /// <summary>
    /// Loads the meta data from the XML.
    /// </summary>
    /// <param name="item">Object that contains meta description.</param>
    public void LoadMeta(OnlineMapsOSMBase item)
    {
        metaInfo = new MetaInfo[item.tags.Count];
        for (int i = 0; i < item.tags.Count; i++)
        {
            OnlineMapsOSMTag tag = item.tags[i];
            metaInfo[i] = new MetaInfo()
            {
                info = tag.value,
                title = tag.key
            };
        }
    }

    protected static bool GetHeightFromString(string str, ref float height)
    {
        if (string.IsNullOrEmpty(str)) return false;

        int l = str.Length;
        if (l > 2 && str[l - 2] == 'c' && str[l - 1] == 'm')
        {
            if (TryGetFloat(str, 0, l - 2, out height))
            {
                height /= 10;
                return true;
            }

            return false;
        }

        if (l > 1 && str[l - 1] == 'm') return TryGetFloat(str, 0, l - 1, out height);
        return TryGetFloat(str, 0, l, out height);
    }

    /// <summary>
    /// Converts a list of nodes into an list of points in Unity World Space
    /// </summary>
    /// <param name="nodes">List of nodes</param>
    /// <param name="container">Reference to OnlineMapsBuildings</param>
    /// <returns>List of points in Unity World Space</returns>
    protected static List<Vector3> GetLocalPoints(List<OnlineMapsOSMNode> nodes, OnlineMapsBuildings container)
    {
        Vector2 tl = container.map.buffer.topLeftPosition;

        double sx, sy;
        container.map.projection.CoordinatesToTile(tl.x, tl.y, container.map.buffer.renderState.zoom, out sx, out sy);

        List<Vector3> localPoints = new List<Vector3>(Mathf.Min(nodes.Count, 8));
        float zoomCoof = container.map.buffer.renderState.zoomCoof;

        float sw = OnlineMapsUtils.tileSize * container.control.sizeInScene.x / container.map.width / zoomCoof;
        float sh = OnlineMapsUtils.tileSize * container.control.sizeInScene.y / container.map.height / zoomCoof;

        for (int i = 0; i < nodes.Count; i++)
        {
            double px, py;
            container.map.projection.CoordinatesToTile(nodes[i].lon, nodes[i].lat, container.map.buffer.renderState.zoom, out px, out py);
            localPoints.Add(new Vector3((float)((sx - px) * sw), 0, (float)((py - sy) * sh)));
        }
        return localPoints;
    }

    private bool HitTest()
    {
        if (buildingCollider == null) return false;

        RaycastHit hit;
        OnlineMapsControlBaseDynamicMesh control = container.control;
        return buildingCollider.Raycast(control.activeCamera.ScreenPointToRay(control.GetInputPosition()), out hit, OnlineMapsUtils.maxRaycastDistance);
    }

    /// <summary>
    /// This method is called when you press a building.
    /// </summary>
    protected void OnBasePress()
    {
        isPressed = true;
        if (OnPress != null) OnPress(this);
        pressPoint = container.map.control.GetInputPosition();
    }

    /// <summary>
    /// This method is called when you release a building.
    /// </summary>
    protected void OnBaseRelease()
    {
        isPressed = false;
        if (OnRelease != null) OnRelease(this);
        if ((pressPoint - container.map.control.GetInputPosition()).magnitude < 10)
        {
            if (OnClick != null) OnClick(this);
        }
    }

#if !UNITY_ANDROID
    /// <summary>
    /// This method is called when you mouse down on a building.
    /// </summary>
    protected void OnMouseDown()
    {
        OnBasePress();
    }

    /// <summary>
    /// This method is called when you mouse up on a building.
    /// </summary>
    protected void OnMouseUp()
    {
        OnBaseRelease();
    }
#endif

    private static bool TryGetFloat(string s, int index, int count, out float result)
    {
        result = 0;
        long n = 0;
        bool hasDecimalPoint = false;
        bool neg = false;
        long decimalV = 1;
        for (int x = 0; x < count; x++, index++)
        {
            char c = s[index];
            if (c == '.') hasDecimalPoint = true;
            else if (c == '-') neg = true;
            else if (c < '0' || c > '9') return false;
            else
            {
                n *= 10;
                n += c - '0';
                if (hasDecimalPoint) decimalV *= 10;
            }
        }

        if (neg) n = -n;

        result = n / (float)decimalV;

        return true;
    }

    private void Update()
    {
        int touchCount = container.control.GetTouchCount();
        if (touchCount != lastTouchCount)
        {
            if (touchCount == 1)
            {
                if (HitTest())
                {
                    OnBasePress();
                }
            }
            else if (touchCount == 0)
            {
                if (isPressed && HitTest()) OnBaseRelease();
                isPressed = false;
            }

            lastTouchCount = touchCount;
        }
    }

    /// <summary>
    /// Type the building's roof.
    /// </summary>
    protected enum RoofType
    {
        /// <summary>
        /// Dome roof.
        /// </summary>
        dome,

        /// <summary>
        /// Flat roof.
        /// </summary>
        flat
    }

    /// <summary>
    /// Building meta key-value pair.
    /// </summary>
    public struct MetaInfo
    {
        /// <summary>
        /// Meta value.
        /// </summary>
        public string info;

        /// <summary>
        /// Meta key.
        /// </summary>
        public string title;
    }
}