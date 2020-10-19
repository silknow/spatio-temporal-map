/*         INFINITY CODE         */
/*   https://infinity-code.com   */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// Component that controls the buildings.
/// </summary>
[AddComponentMenu("Infinity Code/Online Maps/Plugins/Buildings")]
[Serializable]
[OnlineMapsPlugin("Buildings", typeof(OnlineMapsControlBaseDynamicMesh))]
public class OnlineMapsBuildings : MonoBehaviour, IOnlineMapsSavableComponent
{
    #region Actions

    /// <summary>
    /// The event, which occurs when all buildings has been created.
    /// </summary>
    public Action OnAllBuildingsCreated;

    /// <summary>
    /// The event, which occurs when creating of the building.
    /// </summary>
    public Action<OnlineMapsBuildingBase> OnBuildingCreated;

    /// <summary>
    /// The event, which occurs when disposing of the building.
    /// </summary>
    public Action<OnlineMapsBuildingBase> OnBuildingDispose;

    /// <summary>
    /// This event allows you to intercept the calculation of the center point of the building, and return your own center point.
    /// </summary>
    public Func<List<Vector3>, Vector3> OnCalculateBuildingCenter;

    /// <summary>
    /// This event is triggered before create a building.<br/>
    /// Return TRUE - if you want to create this building, FALSE - avoid creating this building.
    /// </summary>
    public Predicate<OnlineMapsBuildingsNodeData> OnCreateBuilding;

    /// <summary>
    /// This event is fired when the height of the building is unknown.<br/>
    /// It allows you to control the height of buildings.<br/>
    /// Return - the height of buildings.
    /// </summary>
    public Func<OnlineMapsOSMWay, float> OnGenerateBuildingHeight;

    /// <summary>
    /// The event, which occurs when the new building was received.
    /// </summary>
    public Action OnNewBuildingsReceived;

    /// <summary>
    /// This event is triggered when preparing to create a building, and allows you to make the necessary changes and additions to the way and used nodes.
    /// </summary>
    public Action<OnlineMapsOSMWay, List<OnlineMapsOSMNode>> OnPrepareBuildingCreation;

    /// <summary>
    /// This event is called when creating a request to OSM Overpass API.
    /// </summary>
    public Func<string, Vector2, Vector2, string> OnPrepareRequest;

    /// <summary>
    /// The event, which occurs after the response has been received.
    /// </summary>
    public Action OnRequestComplete;

    /// <summary>
    /// The event, which occurs when the request is failed.
    /// </summary>
    public Action OnRequestFailed;

    /// <summary>
    /// The event, which occurs when the request for a building sent.
    /// </summary>
    public Action OnRequestSent;

    /// <summary>
    /// This event is triggered before show a building.<br/>
    /// Return TRUE - if you want to show this building, FALSE - do not show this building.
    /// </summary>
    public Predicate<OnlineMapsBuildingBase> OnShowBuilding;

    #endregion

    #region Fields

    #region Static

    private static OnlineMapsBuildings _instance;

    /// <summary>
    /// GameObject, which will create the building.
    /// </summary>
    public static GameObject buildingContainer
    {
        get { return _instance.container; }
    }

    public static float requestRate = 0.1f;

    #endregion

    #region Public

    [NonSerialized]
    public Dictionary<string, OnlineMapsBuildingBase> buildings;

    [NonSerialized]
    public GameObject container;

    [NonSerialized]
    public OnlineMapsControlBaseDynamicMesh control;

    /// <summary>
    /// Range levels of buildings, if the description of the building is not specified.
    /// </summary>
    public OnlineMapsRange levelsRange = new OnlineMapsRange(3, 7, 1, 100);

    /// <summary>
    /// Height of the building level.
    /// </summary>
    public float levelHeight = 4.5f;

    /// <summary>
    /// Need to generate a collider?
    /// </summary>
    public bool generateColliders = true;

    /// <summary>
    /// Scale height of the building.
    /// </summary>
    public float heightScale = 1;

    [NonSerialized]
    public OnlineMaps map;

    /// <summary>
    /// Materials of buildings.
    /// </summary>
    public OnlineMapsBuildingMaterial[] materials;

    /// <summary>
    /// The maximum number of active buildings (0 - unlimited).
    /// </summary>
    public int maxActiveBuildings = 0;

    /// <summary>
    /// The maximum number of buildings (0 - unlimited).
    /// </summary>
    public int maxBuilding = 0;

    /// <summary>
    /// Building download mode
    /// </summary>
    //public Mode mode = Mode.tile;

    /// <summary>
    /// Minimal height of the building.
    /// </summary>
    public float minHeight = 4.5f;

    /// <summary>
    /// Instance of the request
    /// </summary>
    public OnlineMapsOSMAPIQuery osmRequest;

    /// <summary>
    /// Use the Color tag for buildings?
    /// </summary>
    public bool useColorTag = true;

    /// <summary>
    /// Use the Height tag for buildings?
    /// </summary>
    public bool useHeightTag = true;

    /// <summary>
    /// Range of zoom, in which the building will be created.
    /// </summary>
    public OnlineMapsRange zoomRange = new OnlineMapsRange(19, OnlineMaps.MAXZOOM);

    #endregion

    #region Private

    private float _heightScale;
    private OnlineMapsVector2i bottomRight;
    private float lastRequestTime;
    private bool needUpdatePosition;
    private bool needUpdateScale;
    private Queue<OnlineMapsBuildingsNodeData> newBuildingsData;
    private string requestData;
    private OnlineMapsSavableItem[] savableItems;
    private bool sendBuildingsReceived = false;
    private Dictionary<ulong, Tile> tiles;
    private OnlineMapsVector2i topLeft;

    private Dictionary<string, OnlineMapsBuildingBase> unusedBuildings;
    private Vector2 lastSizeInScene;

    #endregion

    #endregion

    #region Properties

    /// <summary>
    /// Instance of OnlineMapsBuildings.
    /// </summary>
    public static OnlineMapsBuildings instance
    {
        get { return _instance; }
    }

    /// <summary>
    /// Returns the active (visible) building.
    /// </summary>
    public IEnumerable<OnlineMapsBuildingBase> activeBuildings
    {
        get { return buildings.Select(b => b.Value); }
    }

    #endregion

    #region Methods

    public void CreateBuilding(OnlineMapsBuildingsNodeData data)
    {
        if (OnCreateBuilding != null && !OnCreateBuilding(data)) return;
        if (buildings.ContainsKey(data.way.id) || unusedBuildings.ContainsKey(data.way.id)) return;

        int initialZoom = map.buffer.renderState.zoom;

        OnlineMapsBuildingBase building = OnlineMapsBuildingBuiltIn.Create(this, data.way, data.nodes);

        if (building != null)
        {
            building.LoadMeta(data.way);
            if (OnBuildingCreated != null) OnBuildingCreated(building);
            unusedBuildings.Add(data.way.id, building);
            if (Math.Abs(map.buffer.lastState.floatZoom - initialZoom) > float.Epsilon) UpdateBuildingScale(building);
            building.transform.localScale.Scale(new Vector3(1, heightScale, 1));
        }
        else
        {
            //Debug.Log("Null building");
        }
    }

    private void GenerateBuildings()
    {
        float startTicks = Time.realtimeSinceStartup;
        const float maxTicks = 0.05f;

        lock (newBuildingsData)
        {
            int newBuildingIndex = newBuildingsData.Count;
            int needCreate = newBuildingIndex;

            while (newBuildingIndex > 0)
            {
                if (maxBuilding > 0 && unusedBuildings.Count + buildings.Count >= maxBuilding) break;

                newBuildingIndex--;
                OnlineMapsBuildingsNodeData data = newBuildingsData.Dequeue();
                CreateBuilding(data);
                needUpdatePosition = true;

                data.Dispose();

                if (Time.realtimeSinceStartup > maxTicks) break;
            }
            if (needCreate > 0 &&
                (newBuildingIndex == 0 || (maxBuilding > 0 && unusedBuildings.Count + buildings.Count >= maxBuilding)) &&
                OnAllBuildingsCreated != null) OnAllBuildingsCreated();
        }

        OnlineMapsBuildingBase.roofIndices = null;
    }

    public OnlineMapsSavableItem[] GetSavableItems()
    {
        if (savableItems != null) return savableItems;

        savableItems = new[]
        {
            new OnlineMapsSavableItem("buildings", "Buildings", SaveSettings)
            {
                loadCallback = LoadSettings
            }
        };

        return savableItems;
    }

    public void LoadBuildingsFromOSM(string osmData)
    {
        Action action = () =>
        {
            Dictionary<string, OnlineMapsOSMNode> nodes;
            Dictionary<string, OnlineMapsOSMWay> ways;
            List<OnlineMapsOSMRelation> relations;

            OnlineMapsOSMAPIQuery.ParseOSMResponseFast(osmData, out nodes, out ways, out relations);

            lock (newBuildingsData)
            {
                MoveRelationsToWays(relations, ways, nodes);
            }

            sendBuildingsReceived = true;
        };

#if !UNITY_WEBGL
        OnlineMapsThreadManager.AddThreadAction(action);
#else
        action();
#endif
    }

    private void LoadSettings(OnlineMapsJSONObject json)
    {
        json.DeserializeObject(this);
    }

    private void MoveRelationToWay(OnlineMapsOSMRelation relation, Dictionary<string, OnlineMapsOSMWay> ways, List<string> waysInRelation, Dictionary<string, OnlineMapsOSMNode> nodes)
    {
        if (relation.members.Count == 0) return;

        OnlineMapsOSMWay way = new OnlineMapsOSMWay();
        List<string> nodeRefs = new List<string>();

        List<OnlineMapsOSMRelationMember> members = relation.members.Where(m => m.type == "way" && m.role == "outer").ToList();
        if (members.Count == 0) return;

        OnlineMapsOSMWay relationWay;
        if (!ways.TryGetValue(members[0].reference, out relationWay) || relationWay == null) return;

        nodeRefs.AddRange(relationWay.nodeRefs);
        members.RemoveAt(0);

        while (members.Count > 0)
        {
            if (!MoveRelationMemberToWay(nodeRefs, members, ways)) break;
        }

        waysInRelation.AddRange(relation.members.Select(m => m.reference));
        way.nodeRefs = nodeRefs;
        way.id = relation.id;
        way.tags = relation.tags;
        newBuildingsData.Enqueue(new OnlineMapsBuildingsNodeData(way, nodes));
    }

    private static bool MoveRelationMemberToWay(List<string> nodeRefs, List<OnlineMapsOSMRelationMember> members, Dictionary<string, OnlineMapsOSMWay> ways)
    {
        string lastRef = nodeRefs[nodeRefs.Count - 1];

        int memberIndex = -1;
        for (int i = 0; i < members.Count; i++)
        {
            OnlineMapsOSMRelationMember member = members[i];
            OnlineMapsOSMWay w = ways[member.reference];
            if (w.nodeRefs[0] == lastRef)
            {
                nodeRefs.AddRange(w.nodeRefs.Skip(1));
                memberIndex = i;
                break;
            }
            if (w.nodeRefs[w.nodeRefs.Count - 1] == lastRef)
            {
                List<string> refs = w.nodeRefs;
                refs.Reverse();
                nodeRefs.AddRange(refs.Skip(1));
                memberIndex = i;
                break;
            }
        }

        if (memberIndex != -1) members.RemoveAt(memberIndex);
        else return false;
        return true;
    }

    public void MoveRelationsToWays(List<OnlineMapsOSMRelation> relations, Dictionary<string, OnlineMapsOSMWay> ways, Dictionary<string, OnlineMapsOSMNode> nodes)
    {
        List<string> waysInRelation = new List<string>();

        foreach (OnlineMapsOSMRelation relation in relations) MoveRelationToWay(relation, ways, waysInRelation, nodes);

        foreach (string id in waysInRelation)
        {
            if (!ways.ContainsKey(id)) continue;

            OnlineMapsOSMWay way = ways[id];
            way.Dispose();
            ways.Remove(id);
        }

        foreach (KeyValuePair<string, OnlineMapsOSMWay> pair in ways)
        {
            newBuildingsData.Enqueue(new OnlineMapsBuildingsNodeData(pair.Value, nodes));
        }
    }

    private void OnBuildingRequestFailed(OnlineMapsTextWebService request)
    {
        if (OnRequestFailed != null)
        {
            try
            {
                OnRequestFailed();
            }
            catch
            {
                
            }
        }
        osmRequest = null;
    }

    private void OnBuildingRequestSuccess(OnlineMapsTextWebService request)
    {
        string response = request.response;
        if (response.Length < 300)
        {
            if (OnRequestFailed != null)
            {
                try
                {
                    OnRequestFailed();
                }
                catch
                {

                }
            }
            return;
        }

        LoadBuildingsFromOSM(response);

        if (OnRequestComplete != null)
        {
            try
            {
                OnRequestComplete();
            }
            catch
            {

            }
        }

        osmRequest = null;
    }

    private void OnDisable()
    {
        RemoveAllBuildings();
        OnlineMapsUtils.Destroy(container);

        if (osmRequest != null)
        {
            osmRequest.OnComplete = null;
            osmRequest = null;
        }
        sendBuildingsReceived = false;
        topLeft = OnlineMapsVector2i.zero;
        bottomRight = OnlineMapsVector2i.zero;

        if (map != null)
        {
            map.OnChangePosition -= OnMapPositionChanged;
            map.OnChangeZoom -= OnMapZoomChanged;
            map.OnLateUpdateAfter -= OnUpdate;
        }
    }

    private void OnDestroy()
    {
        OnlineMapsTileManager.OnPrepareDownloadTile -= OnPrepareDownloadTile;
    }

    private void OnEnable()
    {
        map = GetComponent<OnlineMaps>();
        control = map.control as OnlineMapsControlBaseDynamicMesh;

        bool isFirstEnable = _instance == null;
        _instance = this;

        buildings = new Dictionary<string, OnlineMapsBuildingBase>();
        unusedBuildings = new Dictionary<string, OnlineMapsBuildingBase>();
        newBuildingsData = new Queue<OnlineMapsBuildingsNodeData>();

        container = new GameObject("Buildings");
        container.transform.parent = transform;
        container.transform.localPosition = Vector3.zero;
        container.transform.localRotation = Quaternion.Euler(Vector3.zero);
        container.transform.localScale = Vector3.one;

        if (!isFirstEnable) Start();
    }

    private void OnMapPositionChanged()
    {
        needUpdatePosition = true;
    }

    private void OnMapZoomChanged()
    {
        needUpdateScale = true;
    }

    private void OnUpdate()
    {
        //if (mode == Mode.view)
        {
            if (sendBuildingsReceived)
            {
                if (OnNewBuildingsReceived != null) OnNewBuildingsReceived();
                sendBuildingsReceived = false;
            }
        }
        /*else if (mode == Mode.tile)
        {
            UpdateTiles();
        }*/

        GenerateBuildings();
        UpdateBuildings();
    }

    private void UpdateTiles()
    {
        
    }

    private void RemoveAllBuildings()
    {
        foreach (KeyValuePair<string, OnlineMapsBuildingBase> building in buildings)
        {
            if (OnBuildingDispose != null) OnBuildingDispose(building.Value);
            building.Value.Dispose();
            OnlineMapsUtils.Destroy(building.Value.gameObject);
        }

        foreach (KeyValuePair<string, OnlineMapsBuildingBase> building in unusedBuildings)
        {
            if (OnBuildingDispose != null) OnBuildingDispose(building.Value);
            building.Value.Dispose();
            OnlineMapsUtils.Destroy(building.Value.gameObject);
        }

        buildings.Clear();
        unusedBuildings.Clear();
        newBuildingsData.Clear();
    }

    private void RequestNewBuildings()
    {
        double tlx, tly, brx, bry;
        map.projection.TileToCoordinates(topLeft.x, topLeft.y, map.zoom, out tlx, out tly);
        map.projection.TileToCoordinates(bottomRight.x, bottomRight.y, map.zoom, out brx, out bry);

        requestData = string.Format(OnlineMapsUtils.numberFormat, "(way[{4}]({0},{1},{2},{3});relation[{4}]({0},{1},{2},{3}););out;>;out skel qt;", 
            bry, tlx, tly, brx, "'building'");
        if (OnPrepareRequest != null) requestData = OnPrepareRequest(requestData, new Vector2((float)tlx, (float)tly), new Vector2((float)brx, (float)bry));
    }

    private OnlineMapsJSONItem SaveSettings()
    {
        OnlineMapsJSONItem json = OnlineMapsJSON.Serialize(new
        {
            zoomRange,
            levelsRange,
            levelHeight,
            minHeight,
            heightScale,
            maxBuilding,
            maxActiveBuildings,
            generateColliders,
            useColorTag,
            materials
        });

        return json;
    }

    private void SendRequest()
    {
        if (osmRequest != null || string.IsNullOrEmpty(requestData)) return;

        osmRequest = OnlineMapsOSMAPIQuery.Find(requestData);
        osmRequest.OnSuccess += OnBuildingRequestSuccess;
        osmRequest.OnFailed += OnBuildingRequestFailed;
        if (OnRequestSent != null) OnRequestSent();
        lastRequestTime = Time.time;
        requestData = null;
    }

    private void Start()
    {
        _heightScale = heightScale;
        lastSizeInScene = control.sizeInScene;

        container.layer = map.gameObject.layer;

        map.OnChangePosition += OnMapPositionChanged;
        map.OnChangeZoom += OnMapZoomChanged;
        map.OnLateUpdateAfter += OnUpdate;

        UpdateBuildings();
    }

    private void OnPrepareDownloadTile(OnlineMapsTile tile)
    {
        
    }

    private void UpdateBuildings()
    {
        if (!zoomRange.InRange(map.floatZoom))
        {
            RemoveAllBuildings();
            return;
        }

        if (Math.Abs(heightScale - _heightScale) > float.Epsilon || lastSizeInScene != control.sizeInScene)
        {
            needUpdateScale = true;
        }

        double tlx, tly, brx, bry;
        map.GetTileCorners(out tlx, out tly, out brx, out bry);

        OnlineMapsVector2i newTopLeft = new OnlineMapsVector2i((int)Math.Round(tlx - 2), (int)Math.Round(tly - 2));
        OnlineMapsVector2i newBottomRight = new OnlineMapsVector2i((int)Math.Round(brx + 2), (int)Math.Round(bry + 2));

        if (newTopLeft != topLeft || newBottomRight != bottomRight)
        {
            topLeft = newTopLeft;
            bottomRight = newBottomRight;
            RequestNewBuildings();
        }

        if (lastRequestTime + requestRate < Time.time) SendRequest();

        if (needUpdateScale)
        {
            UpdateBuildingsScale();
        }
        else if (needUpdatePosition)
        {
            UpdateBuildingsPosition();
        }

        needUpdatePosition = false;
        needUpdateScale = false;
    }

    private void UpdateBuildingsPosition()
    {
        Bounds bounds = new Bounds();

        double tlx, tly, brx, bry;
        map.GetCorners(out tlx, out tly, out brx, out bry);

        bounds.min = new Vector3((float)tlx, (float)bry);
        bounds.max = new Vector3((float)brx, (float)tly);

        List<string> unusedKeys = new List<string>();

        foreach (KeyValuePair<string, OnlineMapsBuildingBase> building in buildings)
        {
            if (!bounds.Intersects(building.Value.boundsCoords)) unusedKeys.Add(building.Key);
            else
            {
                UpdateBuildingPosition(building.Value, control, tlx, tly, brx, bry);
            }
        }

        List<string> usedKeys = new List<string>();
        List<string> destroyKeys = new List<string>();

        double px, py;
        map.GetTilePosition(out px, out py);

        float maxDistance = (Mathf.Pow((map.width / OnlineMapsUtils.tileSize) >> 1, 2) + Mathf.Pow((map.height / OnlineMapsUtils.tileSize) >> 1, 2)) * 4;

        foreach (KeyValuePair<string, OnlineMapsBuildingBase> building in unusedBuildings)
        {
            OnlineMapsBuildingBase value = building.Value;
            if (bounds.Intersects(value.boundsCoords))
            {
                usedKeys.Add(building.Key);
                UpdateBuildingPosition(value, control, tlx, tly, brx, bry);
            }
            else
            {
                double bx, by;
                map.projection.CoordinatesToTile(value.centerCoordinates.x, value.centerCoordinates.y, map.zoom, out bx, out @by);
                if (OnlineMapsUtils.SqrMagnitude(0, 0, bx - px, @by - py) > maxDistance) destroyKeys.Add(building.Key);
            }
        }

        for (int i = 0; i < unusedKeys.Count; i++)
        {
            string key = unusedKeys[i];
            OnlineMapsBuildingBase value = buildings[key];
            value.gameObject.SetActive(false);
            unusedBuildings.Add(key, value);
            buildings.Remove(key);
        }

        for (int i = 0; i < usedKeys.Count; i++)
        {
            if (maxActiveBuildings > 0 && buildings.Count >= maxActiveBuildings) break;

            string key = usedKeys[i];
            OnlineMapsBuildingBase value = unusedBuildings[key];

            if (OnShowBuilding != null && !OnShowBuilding(value)) continue;
            value.gameObject.SetActive(true);
            buildings.Add(key, value);
            unusedBuildings.Remove(key);
        }

        for (int i = 0; i < destroyKeys.Count; i++)
        {
            string key = destroyKeys[i];
            OnlineMapsBuildingBase value = unusedBuildings[key];
            if (OnBuildingDispose != null) OnBuildingDispose(value);
            value.Dispose();
            OnlineMapsUtils.Destroy(value.gameObject);
            unusedBuildings.Remove(key);
        }
    }

    private static void UpdateBuildingPosition(OnlineMapsBuildingBase building, OnlineMapsControlBaseDynamicMesh control, double tlx, double tly, double brx, double bry)
    {
        Vector3 newPosition = control.GetWorldPositionWithElevation(building.centerCoordinates.x, building.centerCoordinates.y, tlx, tly, brx, bry);
        building.transform.position = newPosition;
    }

    private void UpdateBuildingScale(OnlineMapsBuildingBase building)
    {
        Vector3 s = control.sizeInScene;
        Vector3 c = new Vector3(s.x / building.initialSizeInScene.x, 1, s.y / building.initialSizeInScene.y);
        c.y = (c.x + c.z) / 2 * instance.heightScale;

        if (Math.Abs(building.initialZoom - map.floatZoom) < float.Epsilon) s = c;
        else if (building.initialZoom < map.floatZoom) s = c * Mathf.Pow(2, map.floatZoom - building.initialZoom);
        else if (building.initialZoom > map.floatZoom) s = c / Mathf.Pow(2, building.initialZoom - map.floatZoom);

        building.transform.localScale = s;
    }

    private void UpdateBuildingsScale()
    {
        lastSizeInScene = control.sizeInScene;
        UpdateBuildingsPosition();
        foreach (KeyValuePair<string, OnlineMapsBuildingBase> building in buildings) UpdateBuildingScale(building.Value);
        foreach (KeyValuePair<string, OnlineMapsBuildingBase> building in unusedBuildings) UpdateBuildingScale(building.Value);
    }

    #endregion

    public enum Mode
    {
        view,
        tile
    }

    public class Tile
    {
        /// <summary>
        /// Is the tile loaded?
        /// </summary>
        public bool loaded = false;

        /// <summary>
        /// Tile X
        /// </summary>
        public int x;

        /// <summary>
        /// Tile Y
        /// </summary>
        public int y;

        /// <summary>
        /// Tile zoom
        /// </summary>
        public int zoom;

        public string GetCacheKey(string prefix)
        {
            return prefix + OnlineMapsTileManager.GetTileKey(zoom, x, y);
        }
    }
}