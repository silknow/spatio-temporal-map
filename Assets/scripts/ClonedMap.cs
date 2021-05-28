/**
 * This script is in charge to generate the stacked map gameobjects related with the displayed data on map.
 * 
 * */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClonedMap : MonoBehaviour
{
    protected GameObject objectMap=null;
    protected GameObject objectTile=null;

    protected Vector2 topLeft;
    protected Vector2 bottomRight;

    public GameObject clonedMap;
    public GameObject tileBase;

    public static ClonedMap instance;


    protected OnlineMapsTile tileTopLeft;
    protected OnlineMapsTile tileBottomRight;
    protected MapSnapshot snapShot;

    protected Vector2 mapTileResolution = new Vector2();

    protected Vector2 topLeftMap = new Vector2();
    protected Vector2 bottomRightMap = new Vector2();
    protected List<MapPointMarker> stackedPoints = new List<MapPointMarker>();


    Map map;
    protected List<OnlineMapsTile> listTiles = new List<OnlineMapsTile>();
    protected Dictionary<int, List<OnlineMapsTile>> tilesCandidates = new Dictionary<int, List<OnlineMapsTile>>();
    int height = 0;
    int width = 0;
    float validPercentageTile = 0.35f;
    int numColumns = 0;
    int numRows = 0;
    OnlineMapsTile topLeftCorner;
    OnlineMapsTile bottomRightCorner;    
    GameObject snapShotMesh;

    public static ClonedMap Instance
    {
        // Here we use the ?? operator, to return 'instance' if 'instance' does not equal null
        // otherwise we assign instance to a new component and return that
        get { return instance ?? (instance = new GameObject("ClonedMap").AddComponent<ClonedMap>()); }
    }

    // This script will simply instantiate the Prefab when the game starts.
    void Start()
    {
        //Debug.Log("INICIA --------------------->");
        // Instantiate at position (0, 0, 0) and zero rotation.
        if (objectMap == null && instance == null)
        {
            instance = this;
            this.objectMap = Instantiate(clonedMap, new Vector3(0, 0, 0), Quaternion.identity);
            this.objectMap.name = "MapSnapshots";
            this.map = SilkMap.instance.map;
           

        }




        //Debug.Log("object map vale " + objectMap);
    }




    public void setMap(Map map)
    {
        this.map = map;
    }

    public void init()
    {
         //this.clonedMap = 
    }

    public GameObject getObjectMap()
    {
        return objectMap;
    }

    public ClonedMap()
    {
        //this.objectMap = Instantiate(clonedMap, new Vector3(0, 0, 0), Quaternion.identity);
        //this.objectMap.name = "clonacion1";
        //cloneCurrent();
    }

    public GameObject take(int height, int width, int from, int to)
    {
        double sw = Screen.width;
        double sh = Screen.height;
        Vector2 topLeftScreen = new Vector2(0, height);
        Vector2 bottomRightScreen = new Vector2(width, 0);

        bottomRight = OnlineMaps.instance.bottomRightPosition;
        topLeft = OnlineMaps.instance.topLeftPosition;

        listTiles.Clear();
        this.height = height;
        this.width = width;
        loadTilesAtLevel((int)map.getZoom());

        GameObject groupObject = new GameObject();
        groupObject.name = "shot0";
        
        createGameObject(groupObject, from, to);

        groupObject.transform.parent = objectMap.transform;

        return groupObject;
    }


    public void updateVisualization()
    {
        foreach (MapPointMarker p in stackedPoints)
        {
            p.forceShow();
        }
    }

    protected void loadTilesAtLevel(int level)
    {
        int i = 0;
        int numTiles = OnlineMaps.instance.tileManager.tiles.Count;        

        numRows = 0;

        // Load the candidate tiles and know the number of rows of the snapshot
        while (i < numTiles)
        {
            OnlineMapsTile tile = OnlineMaps.instance.tileManager.tiles[i];

            if (tile.zoom == level && inScreen(tile) && tile.status == OnlineMapsTileStatus.loaded)
            {
                if (!tilesCandidates.ContainsKey(tile.x))
                {
                    List<OnlineMapsTile> valuesList = new List<OnlineMapsTile>();
                    tilesCandidates.Add(tile.x, valuesList);
                }

                tilesCandidates[tile.x].Add(tile);

                if (tilesCandidates[tile.x].Count > numRows)
                    numRows = tilesCandidates[tile.x].Count;
            }

            i++;
        }

        numColumns = 0;

        // Put in listTiles the definitive tile list, and get the numColumns property
        foreach (int key in tilesCandidates.Keys)
        {
            List<OnlineMapsTile> candidateList = tilesCandidates[key];
            if (candidateList.Count == numRows)
            {
                listTiles.AddRange(candidateList);
                numColumns++;
            }
        }

        tilesCandidates.Clear();

        updateCorners();
    }

    protected bool inScreen(OnlineMapsTile tile)
    {
        bool inside = true;

        // Reject all the tiles out of the screen (0,height),(width,0)
        if (tile.bottomRight.x < topLeft.x || tile.bottomRight.y > topLeft.y)
            return false;

        if (tile.topLeft.y < bottomRight.y || tile.topLeft.x > bottomRight.x)
            return false;

        // Reject all the tiles inside the screen, which its area inside is minor than valid percentage

        // Calculate the initial area of the tile
        double tileWidth = tile.bottomRight.x - tile.topLeft.x;
        double tileHeight = tile.topLeft.y - tile.bottomRight.y;
        double iniArea = Mathf.Abs((float)(tileWidth * tileHeight));

        // Get the width and height inside the  screen
        bool outLimit = false;

        if (tile.topLeft.x < topLeft.x) {
            tileWidth = tile.bottomRight.x - topLeft.x;
            outLimit = true;
        }

        if (tile.bottomRight.x > bottomRight.x) { 
            tileWidth = bottomRight.x - tile.topLeft.x;
            outLimit = true;
        }

        if (tile.topLeft.y > topLeft.y) { 
            tileHeight = topLeft.y - tile.bottomRight.y;
            outLimit = true;
        }

        if (tile.bottomRight.y < bottomRight.y) { 
            tileHeight = (tile.topLeft.y-bottomRight.y);
            outLimit = true;
        }

        // Check if area is minor than valid percentage
        if (outLimit && (tileHeight * tileWidth <= this.validPercentageTile * iniArea))
            return false;


        return inside;
    }

    public void updateCorners()
    {

        if (listTiles.Count > 0)
        {
            topLeftCorner = listTiles[0];
            bottomRightCorner = listTiles[0];

            foreach (OnlineMapsTile tile in listTiles)
            {
                if (tile.topLeft.x <= topLeftCorner.x && tile.topLeft.y >= topLeftCorner.y)
                    topLeftCorner = tile;

                if (tile.bottomRight.x >= bottomRightCorner.x && tile.bottomRight.y <= bottomRightCorner.y)
                    bottomRightCorner = tile;
            }


        }
    }

    protected void createGameObject(GameObject groupObject, int from, int to)
    {
        //destroyGameObject();

        GameObject groupCanvasObject = new GameObject();
        groupCanvasObject.name = "Canvas";
        groupCanvasObject.transform.parent = groupObject.transform;

        groupCanvasObject.AddComponent<Canvas>();
        //groupCanvasObject.transform.parent = groupObject.transform;


        if (listTiles.Count > 0)
        {

            Vector2 centerMap = new Vector2(topLeft.x + (bottomRight.x - topLeft.x) / 2f,
                                            topLeft.y + (bottomRight.y - topLeft.y) / 2f);

            List<Vector3> centerTileList = new List<Vector3>();

            foreach (OnlineMapsTile tile in listTiles)
            {
                //OnlineMapsTile tile = listTiles[i];

                snapShotMesh = Instantiate(tileBase, new Vector3(0, 0, 0), Quaternion.identity, groupObject.transform);

                Mesh planeMesh = snapShotMesh.GetComponent<MeshFilter>().mesh;
                Bounds bounds = planeMesh.bounds;
                float boundsX = bounds.size.x;
                float boundsZ = bounds.size.z;

                float currentTileWidth = Mathf.Abs((float)(tile.bottomRight.x - tile.topLeft.x));
                float currentTileHeight = Mathf.Abs((float)(tile.topLeft.y - tile.bottomRight.y));

                float scaleX = currentTileWidth / boundsX;
                float scaleZ = currentTileHeight / boundsZ;

                /*snapShotMesh.transform.SetPositionAndRotation(snapShotMesh.transform.position +
                    new Vector3((-(float)(tile.topLeft.x + currentTileWidth / 2.0)) + centerMap.x,
                    0.0f,
                    (-(float)(tile.topLeft.y - currentTileHeight / 2.0)) + centerMap.y), snapShotMesh.transform.rotation);*/
                snapShotMesh.transform.localPosition = new Vector3(
                    (-(float) (tile.topLeft.x + currentTileWidth / 2.0f)) + centerMap.x,
                    0.0f,
                    (-(float) (tile.topLeft.y - currentTileHeight / 2.0f)) + centerMap.y);
                

                snapShotMesh.transform.localScale = new Vector3(scaleX, 1.0f, scaleZ);

                if (tile != null)
                {
                    Texture2D tex = tile.texture;
                    tex.Apply();
                    MeshRenderer renderer = snapShotMesh.GetComponent<MeshRenderer>();
                    renderer.material.mainTexture = tex;
                }
                else
                    Debug.Log("TILE IS NULL");



                //putMarkers(tile, groupCanvasObject, groupObject, centerMap,from, to);

                Vector3 pPos = snapShotMesh.transform.position;
                //pPos.x = pPos.x * scaleX;
                //pPos.z = pPos.z * scaleZ;
                centerTileList.Add(pPos);

            }


            // Obtiene las bounds del grupo con el snapshot
            Bounds boundsG = getBounds(groupObject);
            //Bounds boundsG = CalculateLocalBounds(groupObject);


            // Crea cubo que va en la parte inferior
            // Pasando la lista de centros de cada tile y las bounds 
            GameObject cube = createBase(centerTileList, boundsG, groupObject.transform);
            cube.name = groupObject.name + " Base";

            // Lo añadoal grupo de snapshot
            cube.transform.parent = groupObject.transform;


            Canvas myCanvas = groupCanvasObject.GetComponent<Canvas>();
            myCanvas.renderMode = RenderMode.WorldSpace;
            RectTransform canvasRect = myCanvas.GetComponent<RectTransform>();
            //canvasRect.localRotation = Quaternion.Euler(-90,0,0);
            //canvasRect.localPosition = boundsG.center;
            canvasRect.transform.parent = groupObject.transform;
            canvasRect.SetPositionAndRotation(cube.transform.position, Quaternion.identity);
            canvasRect.Rotate(new Vector3(-90, 0, 0));
            canvasRect.sizeDelta = new Vector3(boundsG.size.x, boundsG.size.z, boundsG.size.y);
    

            foreach (OnlineMapsTile tile in listTiles)
                putMarkers(tile, groupCanvasObject, groupObject, centerMap, from, to, -boundsG.size.x * 0.0015f, cube.transform);
        }

    }

    Bounds getBounds(GameObject objeto)
    {
        Bounds bounds;
        Renderer childRender;
        bounds = getRenderBounds(objeto);
        if (bounds.extents.x == 0)
        {
            bounds = new Bounds(objeto.transform.position, Vector3.zero);
            foreach (Transform child in objeto.transform)
            {
                childRender = child.GetComponent<Renderer>();
                if (childRender)
                {
                    bounds.Encapsulate(childRender.bounds);
                }
                else
                {
                    bounds.Encapsulate(getBounds(child.gameObject));
                }
            }
        }

        return bounds;
    }

    Bounds getRenderBounds(GameObject objeto)
    {
        Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
        Renderer render = objeto.GetComponent<Renderer>();
        if (render != null)
        {
            return render.bounds;
        }
        return bounds;
    }

    /**
     * Crea un cubo, cuyo centro está está el centro de los puntos pasados en el primer parámetro
     * y la escala viene definida por los bounds de los tiles, que están en el segundo parámetro
     * */
    protected GameObject createBase(List<Vector3> centerTileList, Bounds bounds, Transform parent)
    {

        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //cube.transform.parent = parent;

        /*
        // Obtener el punto (minX,maxX) y (minZ,maxZ) que comprenden las tiles.
        float minX = centerTileList[0].x, maxX = centerTileList[0].y;
        float minZ = centerTileList[0].z, maxZ= centerTileList[0].z;
        float centerY = centerTileList[0].y;

        foreach (Vector3 center in centerTileList)
        {
            if (center.x < minX) minX  =center.x;
            if (center.x > maxX) maxX = center.x;
            if (center.z < minZ) minZ = center.z;
            if (center.z > maxZ) maxZ = center.z;
        }

        // Obtiene las coordenadas x,y del centro de las tiles
        float cubeCenterX = minX + (maxX - minX) / 2.0f;
        float cubeCenterZ = minZ + (maxZ - minZ) / 2.0f;

        //cube.transform.position = new Vector3(cubeCenterX, centerY, cubeCenterZ);

        //cube.transform.Translate(new Vector3(0, -bounds.center.x / 90, 0));

        // Obtiene la escala en x y z del cubo a partir de las bounds
        //float scaleX = (maxX-minX) / bounds.size.x;        
        //float scaleZ = (maxZ-minZ) / bounds.size.z;
        */
        cube.transform.localScale = new Vector3(bounds.size.x, bounds.size.x*0.008f, bounds.size.z);
        cube.transform.SetPositionAndRotation(new Vector3(bounds.center.x, bounds.center.y - cube.transform.localScale.y*0.501f, bounds.center.z), Quaternion.identity);

        //cube.transform.localPosition = -cube.transform.localScale.y*0.501f*cube.transform.up;

        var sc = parent.gameObject.AddComponent<StackedMapInstance>();
        sc.cube = cube.transform;

        return cube;
    }

    protected void destroyGameObject()
    {
        List<GameObject> modelElements = new List<GameObject>();

        for (int i = 0; i < objectMap.transform.childCount; i++)
            modelElements.Add(objectMap.transform.GetChild(i).gameObject);

        foreach (GameObject gObject in modelElements)
            Destroy(gObject);
        //Resources.UnloadUnusedAssets();
    }


    protected void putMarkers(OnlineMapsTile tile, GameObject parentCanvas, GameObject parent, Vector2 centerMap, int from, int to, float desp, Transform baseT)
    {

        if (!map.pointsViewing())
            putClusterMarkers(tile, parentCanvas, parent, centerMap, from, to, desp, baseT);
        else
        {
            List<GridCluster> groupPointList = putPointMarkers(tile, parent, centerMap, from, to, desp, baseT);
            putGroupPointMarkers(tile, parentCanvas, parent, centerMap, from, to, desp, baseT, groupPointList);
        }

    }

    protected void putGroupPointMarkers(OnlineMapsTile tile, GameObject parentCanvas, GameObject parent, Vector2 centerMap, int from, int to, float desp, Transform baseT, List<GridCluster> groupPointList)
    {
        int level = map.getLevel();


        //List<MapPointMarker> clusterList = ((MapMarker)(map)).getClusterMarkersAtLevel(level);

        List<MapPointMarker> clusterList = new List<MapPointMarker>();

        foreach (GridCluster gCluster in groupPointList)
            clusterList.Add((MapPointMarker)gCluster.getCenter());


        for (int m = 0; m < clusterList.Count; m++)
        {

            List<MapPoint> pointList = clusterList[m].getClusteredPoints();
            int numData = 0;

            foreach (MapPoint pointC in pointList)
                if (pointC.getFrom() >= from && pointC.getTo() <= to)
                    numData++;

            if (numData > 0)
            {
                double longitud, latitud;

                MapPointMarker gPoint = clusterList[m];

                gPoint.getMarker3D().GetPosition(out longitud, out latitud);

                if (longitud >= tile.topLeft.x && longitud <= tile.bottomRight.x)
                    if (latitud <= tile.topLeft.y && latitud >= tile.bottomRight.y)
                    {

                        //------------------
                        GameObject marker = Instantiate(clusterList[m].getMarker3D().prefab,
                            new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
                        marker.transform.SetPositionAndRotation(marker.transform.position +
                                               new Vector3((-(float)(longitud)) + centerMap.x,
                                                            desp,
                                                           (-(float)(latitud)) + centerMap.y), marker.transform.rotation);

                        marker.transform.parent = parent.transform;



                        marker.name = gPoint.getLabel();

                        /*
                        GameObject markerCluster = Instantiate(clusterList[m].markerInstance.gameObject,
                            new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
                        markerCluster.transform.SetPositionAndRotation(markerCluster.transform.position +
                                               new Vector3((-(float)(longitud)) + centerMap.x,
                                                            -1.0f,
                                                           (-(float)(latitud)) + centerMap.y), markerCluster.transform.rotation);
                        markerCluster.transform.parent = parent.transform;
                        */





                        Canvas canvasSlice = parentCanvas.GetComponent<Canvas>();

                        GameObject markerCluster = GameObject.Instantiate(clusterList[m].markerInstance.gameObject, canvasSlice.transform) as GameObject;
                        markerCluster.transform.SetPositionAndRotation(marker.transform.localPosition, marker.transform.rotation);
                        //markerCluster.transform.parent = parent.transform;

                        RectTransform rectTransform = markerCluster.transform as RectTransform;
                        //markerCluster.transform.localScale = new Vector3(0.1f,0.1f,0.1f);
                        float factor = baseT.localScale.x / 1000.0f;
                        markerCluster.transform.localScale = new Vector3(factor, factor, factor);
                        markerCluster.transform.localRotation = Quaternion.Euler(new Vector3(-60, 180, 0));
                        markerCluster.transform.localPosition += Vector3.up * 0.6f;

                        string titleData = numData.ToString();
                        SetText(rectTransform, "Title", titleData);



                        //float scale = (clusterList[m].getMarker3D().scale / 15.0f) * numData;
                        //4.0f;
                        //marker.transform.localScale = new Vector3(scale, scale, scale);


                        gPoint.setStackedGameObject(marker);
                        stackedPoints.Add(gPoint);

                        if (gPoint.isFiltered() || !gPoint.isVisible())
                            marker.SetActive(false);
                        else
                            marker.SetActive(true);



                        //------------------

                        /*
                        
                        gPoint.setStackedPosition(new Vector3((-(float)(gPoint.getX())) + centerMap.x,
                                                        -1.0f,
                                                    (-(float)(gPoint.getY())) + centerMap.y), parent.transform);
                        float scale = gPoint.getMarker3D().scale /6.0f;
                        gPoint.setStackedScale(new Vector3(scale, scale, scale));

                        stackedPoints.Add(gPoint);
                        */

                    }
            }

        }
    }

    protected void putClusterMarkers(OnlineMapsTile tile, GameObject parentCanvas, GameObject parent, Vector2 centerMap, int from, int to, float desp, Transform baseT)
    {
        int level = map.getLevel();

        
        List<MapPointMarker> clusterList = ((MapMarker)(map)).getClusterMarkersAtLevel(level);


        for (int m = 0; m < clusterList.Count; m++)
        {

            List<MapPoint> pointList = clusterList[m].getClusteredPoints();
            int numData = 0;

            foreach (MapPoint pointC in pointList)
                if (pointC.getFrom() >= from && pointC.getTo() <= to)
                    numData++;

            if (numData > 0)
            {
                double longitud, latitud;

                MapPointMarker gPoint = clusterList[m];

                gPoint.getMarker3D().GetPosition(out longitud, out latitud);

                if (longitud >= tile.topLeft.x && longitud <= tile.bottomRight.x)
                    if (latitud <= tile.topLeft.y && latitud >= tile.bottomRight.y)
                    {
                        
                        //------------------
                        GameObject marker = Instantiate(clusterList[m].getMarker3D().prefab,
                            new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
                        marker.transform.SetPositionAndRotation(marker.transform.position +
                                               new Vector3((-(float)(longitud)) + centerMap.x,
                                                            desp,
                                                           (-(float)(latitud)) + centerMap.y), marker.transform.rotation);

                        marker.transform.parent = parent.transform;



                        marker.name = gPoint.getLabel();

                        /*
                        GameObject markerCluster = Instantiate(clusterList[m].markerInstance.gameObject,
                            new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
                        markerCluster.transform.SetPositionAndRotation(markerCluster.transform.position +
                                               new Vector3((-(float)(longitud)) + centerMap.x,
                                                            -1.0f,
                                                           (-(float)(latitud)) + centerMap.y), markerCluster.transform.rotation);
                        markerCluster.transform.parent = parent.transform;
                        */




                        
                        Canvas canvasSlice = parentCanvas.GetComponent<Canvas>();

                        GameObject markerCluster = GameObject.Instantiate(clusterList[m].markerInstance.gameObject, canvasSlice.transform) as GameObject;
                        markerCluster.transform.SetPositionAndRotation(marker.transform.localPosition, marker.transform.rotation);
                        //markerCluster.transform.parent = parent.transform;

                        RectTransform rectTransform = markerCluster.transform as RectTransform;
                       //markerCluster.transform.localScale = new Vector3(0.1f,0.1f,0.1f);
                        float factor = baseT.localScale.x / 1000.0f;
                        markerCluster.transform.localScale = new Vector3(factor, factor, factor);
                        markerCluster.transform.localRotation = Quaternion.Euler(new Vector3(-60, 180, 0));
                        markerCluster.transform.localPosition += Vector3.up * 0.6f;

                        string titleData = numData.ToString();
                        SetText(rectTransform, "Title", titleData);



                            //float scale = (clusterList[m].getMarker3D().scale / 15.0f) * numData;
                        //4.0f;
                        //marker.transform.localScale = new Vector3(scale, scale, scale);
                   

                        gPoint.setStackedGameObject(marker);
                        stackedPoints.Add(gPoint);

                        if (gPoint.isFiltered() || !gPoint.isVisible())
                            marker.SetActive(false);
                        else
                            marker.SetActive(true);



                        //------------------

                        /*
                        
                        gPoint.setStackedPosition(new Vector3((-(float)(gPoint.getX())) + centerMap.x,
                                                        -1.0f,
                                                    (-(float)(gPoint.getY())) + centerMap.y), parent.transform);
                        float scale = gPoint.getMarker3D().scale /6.0f;
                        gPoint.setStackedScale(new Vector3(scale, scale, scale));

                        stackedPoints.Add(gPoint);
                        */

                    }
            }

        }
    }

    private void SetText(RectTransform rt, string childName, string value)
    {

        var title = rt.GetComponentInChildren<Text>();
        if (title != null) title.text = value;
    }

    protected List<GridCluster> putPointMarkers(OnlineMapsTile tile, GameObject parent, Vector2 centerMap, int from, int to, float desp, Transform baseT)
    {
        int level = map.getLevel();
        int numPoint = 0;


        List<GridCluster> groupPointList = new List<GridCluster>();
        List<MapPoint> pointList = map.pointsInTime(from, to);

        Debug.Log("Los puntos de " + from + " hasta " + to + " son :");

        if (pointList == null)
            return null;

        foreach (MapPoint p in pointList)
        {
            MapPointMarker gPoint = (MapPointMarker)p;

            

            float longitud = gPoint.getX();
            float latitud = gPoint.getY();

            if (tile.topLeft.x<=longitud && tile.bottomRight.x>=longitud &&
                tile.topLeft.y>=latitud  && tile.bottomRight.y<=latitud) {

                if (!p.isGroupPoint())
                {
                    numPoint++;

                    if (numPoint == 2 && from==1701)
                        numPoint++;

                    Debug.Log(p.getLabel() + "(" + gPoint.getMarker3D().transform.name + ") - " + p.getFrom() + "-" + p.getTo());

                    //--------------

                    GameObject marker = Instantiate(gPoint.getMarker3D().prefab,
                        new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
                    marker.transform.SetPositionAndRotation(marker.transform.position +
                                                            new Vector3((-(float)(gPoint.getX())) + centerMap.x,
                                                                desp,
                                                                (-(float)(gPoint.getY())) + centerMap.y), marker.transform.rotation);


                    foreach (Transform child in marker.transform)
                    {
                        GameObject obj = child.gameObject;

                        if (child.name.Equals("picture"))
                        {
                            child.GetComponent<Renderer>().material.mainTexture = gPoint.getTexture();
                        }

                    }

                    marker.transform.parent = parent.transform;



                    //float scale = gPoint.getMarker3D().scale / 100.0f;
                    //marker.transform.localScale = new Vector3(scale, scale, scale);

                    float factor = baseT.localScale.x / 20.0f;
                    marker.transform.localScale = new Vector3(factor, factor, factor);


                    marker.AddComponent(typeof(EventTrigger));
                    EventTrigger trigger = marker.GetComponent<EventTrigger>();
                    EventTrigger.Entry entry = new EventTrigger.Entry();
                    entry.eventID = EventTriggerType.PointerClick;
                    entry.callback.AddListener((eventData) => { onClick(); });
                    trigger.triggers.Add(entry);





                    gPoint.setStackedGameObject(marker);
                    stackedPoints.Add(gPoint);
                }
                else
                    if (!groupPointList.Contains(p.getGroupPointCluster()))
                        groupPointList.Add(p.getGroupPointCluster());

                //--------------------

                /*
                gPoint.setStackedPosition(new Vector3((-(float)(gPoint.getX())) + centerMap.x,
                                                                -1.0f,
                                                            (-(float)(gPoint.getY())) + centerMap.y), parent.transform);
                float scale = gPoint.getMarker3D().scale / 100.0f;
                gPoint.setStackedScale(new Vector3(scale, scale, scale));

                stackedPoints.Add(gPoint);
                */

            }
        }

        return groupPointList;
    }

    public void onClick()
    {
        Debug.Log("Stacked click");
    }

    public void unStackPoints()
    {
        foreach (MapPointMarker p in stackedPoints)
            p.setStacked(false);
            //p.unStack();

        stackedPoints.Clear();
    }


    public GameObject cloneCurrent(int from, int to)
    {
        return this.take(SilkMap.instance.mapCamera.pixelHeight, SilkMap.instance.mapCamera.pixelWidth, from, to);
    }

    
    public void setValidPercentageTile(float percentage) => this.validPercentageTile = percentage;

    public float getValidPercentageTile() => this.validPercentageTile;

    public int getNumTiles() => listTiles.Count;

    public Vector2 getDesplazamiento()
    {
        Vector2 desplazamiento = new Vector2();
        //desplazamiento.x = (float) (mapTileResolution.x * (getTileWidth() - 1)) / 2.0f - getTileWidth() / 2.0f;
        //desplazamiento.x = (float) (mapTileResolution.x * (getTileWidth())) / 2.0f + 1.5f*getTileWidth();
        desplazamiento.x = (float)((mapTileResolution.x + 3.0f) * (getTileWidth() / 2.0f));
        desplazamiento.y = (float)((mapTileResolution.y + 3.0f) * (getTileHeight() / 2.0f));
        //desplazamiento.y = (float)(mapTileResolution.y * (getTileHeight())) / 2.0f + 3.0f * getTileHeight();

        desplazamiento.x = (float)(mapTileResolution.x * 10.0f) / 2.0f -10.0f/2.0f;
        desplazamiento.y = (float)(mapTileResolution.y * 10.0f) / 2.0f;
        Debug.Log("RES = " + mapTileResolution);
        Debug.Log("TILE =" + getTileWidth() + "," + getTileHeight());

        Debug.Log("DESP = " + desplazamiento);

        return desplazamiento;
    }

    public int getTileWidth()
    {
        return bottomRightCorner.x - topLeftCorner.x + 1;
    }

    public int getTileHeight()
    {
        return topLeftCorner.y - bottomRightCorner.y + 1;
    }

   

    public static Vector3 getTranslate(int division, int numDivisions)
    {
        Vector3 trans = new Vector3(0.0f, 0.0f, 0.0f);


        if (numDivisions==3)
        {
            if (division==0)
                trans.y = 14.0f;
            if (division == 1)
                trans.y = -2.0f;
            if (division == 2)
                trans.y = -20.0f;
        }

        if (numDivisions == 2)
        {
            if (division == 0)
                trans.y = 9.0f;
            if (division == 1)
                trans.y = -15.0f;
        }



        return trans;
    }

    public static Vector3 getRotate(int division, int numDivisions)
    {
        Vector3 rot = new Vector3(0.0f, 0.0f, 0.0f);


        if (numDivisions == 3)
        {
            if (division == 0)
                rot.x = 30.0f;
            if (division == 1)
                rot.x = 22.5f;
            if (division == 2)
                rot.x = 6.3f;
        }

        if (numDivisions == 2)
        {
            if (division == 0)
                rot.x = 26.0f;
            if (division == 1)
                rot.x = 10.5f;

        }


        return rot;
    }

    private Bounds CalculateLocalBounds(GameObject objeto)
    {
        Quaternion currentRotation = objeto.transform.rotation;
        objeto.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        Bounds bounds = new Bounds(objeto.transform.position, Vector3.zero);

        foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            bounds.Encapsulate(renderer.bounds);
        }

        Vector3 localCenter = bounds.center - objeto.transform.position;
        bounds.center = localCenter;

        objeto.transform.rotation = currentRotation;

        return bounds;
    }
}

