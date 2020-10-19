using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapPointMarkerClicked : UnityEvent<MapPointMarker> { }
public class MapMarker : Map {

    protected List<List<MapPointMarker>> clusterMarkers = new List<List<MapPointMarker> >();
    protected List<List<OnlineMapsDrawingLine>> clusterLines = new List<List<OnlineMapsDrawingLine>> ();
    protected List<List<OnlineMapsDrawingLine>> connectionLines = new List<List<OnlineMapsDrawingLine>>();
    protected int selectedCluster = -1;
    protected List<OnlineMapsDrawingLine> quadsDrawed = new List<OnlineMapsDrawingLine>();
    List<Vector2> pointsBase=null;

    protected Hashtable connectionsLinesPerLevel = new Hashtable();
    private bool drawTheClustersQuad = false;

    public GameObject clusterPrefab;

    public GameObject customMarkerGameObject;
    public Canvas customMarkerCanvas;
    


    

    public RectTransform container;
    private List<MarkerInstance> markers = new List<MarkerInstance>();
    private Canvas canvas;
    private OnlineMapsTileSetControl control;

    public MapMarker()
    {
        OnlineMapsControlBase.instance.OnMapClick += OnMapClick;
        OnlineMaps.instance.OnMapUpdated += OnMapUpdated;

        control = OnlineMapsTileSetControl.instance;
        //canvas = container.GetComponentInParent<Canvas>();
    }

    private Camera worldCamera
    {
        get
        {
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay) return null;
            return canvas.worldCamera;
        }
    }

    public void OnMapUpdated()
    {
        SilkMap.instance.refreshStack();
        if (markers!=null)
            UpdateMarkers();
    }

    private void UpdateMarkers()
    {

        foreach (MarkerInstance marker in markers) UpdateMarker(marker);
    }

    private void UpdateMarker(MarkerInstance marker)
    {
        double px = marker.data.longitude;
        double py = marker.data.latitude;

        Vector2 screenPosition = control.GetScreenPosition(px, py);

        if (screenPosition.x < 0 || screenPosition.x > Screen.width ||
            screenPosition.y < 0 || screenPosition.y > Screen.height)
        {
            marker.gameObject.SetActive(false);
            return;
        }

        if (marker.mapMarker != null && marker.mapMarker.isVisible())
        {
            string newTitle = marker.mapMarker.getGridCluster().getNumVisiblePoints().ToString();
            if (!newTitle.Equals(marker.data.title)) {
                marker.data.title = newTitle;
                SetText(marker.transform, "Title", newTitle);
            }

            if (SilkMap.instance.map.GetDimension() == 3)
            {
                /*
                marker.transform.localPosition = marker.mapMarker.getMarker3D().transform.localPosition;
                if (marker.transform.localRotation.eulerAngles.x == 90)
                    marker.transform.Rotate(new Vector3(-45, 0, 0));
                    */
                var position = OnlineMapsTileSetControl.instance.GetWorldPosition(marker.mapMarker.getVector2());
                marker.transform.localPosition = new Vector3(position.x, -position.z,3f);
                marker.transform.localRotation = Quaternion.Euler(new Vector3(-45, 180, 0));
            }
            else
            {
                /*
                Vector2 newPosition = OnlineMapsControlBase.instance.GetPosition(new Vector2(marker.mapMarker.getX(), marker.mapMarker.getY()));
                marker.transform.localPosition = new Vector3(-newPosition.x, marker.transform.localPosition.y+0.5f, newPosition.y);
                if (marker.transform.localRotation.eulerAngles.x == 45)
                    marker.transform.Rotate(new Vector3(45, 0, 0));
                    */


                var position = OnlineMapsTileSetControl.instance.GetWorldPosition(marker.mapMarker.getVector2());
                marker.transform.localPosition = new Vector3(position.x, -position.z,0f);
                marker.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
            }

            if (!marker.gameObject.activeSelf)
                marker.gameObject.SetActive(true);

            


            /*
            Vector2 screenPosition = control.GetScreenPosition(px, py);

            if (screenPosition.x < 0 || screenPosition.x > Screen.width ||
                screenPosition.y < 0 || screenPosition.y > Screen.height)
            {
                marker.gameObject.SetActive(false);
                return;
            }

            RectTransform markerRectTransform = marker.transform;

            if (!marker.gameObject.activeSelf) marker.gameObject.SetActive(true);

            Camera worldCamera = null;
            if (customMarkerCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
                worldCamera = canvas.worldCamera;

            Vector2 point;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(markerRectTransform as RectTransform, screenPosition, worldCamera, out point);
            markerRectTransform.localPosition = point;*/
        }
        else
            marker.gameObject.SetActive(false);
    }

    public void fixZoomInterval(OnlineMaps oMaps, float min, float max)
    {
        OnlineMaps.instance.zoomRange = new OnlineMapsRange(min, max);
    }

    public void fixPositionInterval(OnlineMaps oMaps, float latMin, float longMin, float latMax, float longMax)
    {
        oMaps.positionRange = new OnlineMapsPositionRange(latMin, longMin, latMax, longMax);
    }

    public List<OnlineMapsDrawingLine> getConnectionLinesOfCluster(int clusterPosition, int level)
    {
        List<List<OnlineMapsDrawingLine>> levelConnections = (List<List<OnlineMapsDrawingLine>>)connectionsLinesPerLevel[level];

        if (levelConnections != null && levelConnections.Count>=clusterPosition)
            return levelConnections[clusterPosition];
        else
            return null;
    }

    public new void reset()
    {
        if (markers != null)
        {
            foreach (MarkerInstance markerI in markers)
                GameObject.DestroyImmediate(markerI.gameObject);

            markers.Clear();
        }

        for (int i=0;i<points.Count;i++)
        {
            MapPointMarker point = (MapPointMarker)points[i];

            if (point.isCluster() || !maintainPoints)
            {
                point.getMarker3D().DestroyInstance();
                point.getMarker2D().DestroyInstance();
            }
        }

        for (int i=0;i<clusterMarkers.Count;i++)
        {
            List< MapPointMarker> clusterMarkersList = clusterMarkers[i];

            for (int j=0;j<clusterMarkersList.Count;j++)
            {
                //UnityEngine.Object.Destroy(clusterMarkersList[j].getMarker3D().prefab);
                clusterMarkersList[j].reset();
                if(clusterMarkersList[j].getMarker3D()!=null)
                    clusterMarkersList[j].getMarker3D().Dispose();
                if (clusterMarkersList[j].getMarker2D() != null)
                    clusterMarkersList[j].getMarker2D().Dispose();
            }

            clusterMarkersList.RemoveRange(0, clusterMarkersList.Count);
        }

        clusterMarkers.RemoveRange(0, clusterMarkers.Count);


        for (int i = 0; i < clusterLines.Count; i++)
        {
            List<OnlineMapsDrawingLine> clusterLinesList = clusterLines[i];

            for (int j = 0; j < clusterLinesList.Count; j++)
            {
                clusterLinesList[j].Dispose();                
            }

            clusterLinesList.RemoveRange(0, clusterLinesList.Count);
        }

        base.reset();

        clusterLines.RemoveRange(0, clusterLines.Count);

        List<int> keyList = new List<int>();

        foreach (int el in connectionsLinesPerLevel.Keys)
        {
            keyList.Add(el);
        }

        for (int i=0;i<keyList.Count;i++)
        {
            List<List<OnlineMapsDrawingLine>> levelConnections = (List < List < OnlineMapsDrawingLine >>)connectionsLinesPerLevel[keyList[i]];
            for (int j=0;j<levelConnections.Count;j++)
            {
                List<OnlineMapsDrawingLine> listLine = levelConnections[j];

                for (int u = 0; u < listLine.Count; u++)
                {
                    if (listLine[u]!=null)
                        listLine[u].Dispose();
                }

                listLine.RemoveRange(0, listLine.Count);
            }

            connectionsLinesPerLevel.Remove(keyList[i]);
        }
    }

    public override void updateClustersDimension(int dimension)
    {
        for (int level = 0; level < clusterMarkers.Count; level++) // clusterManager.getNumLevels(); level++)
            if (clusterMarkers[level]!=null)
                foreach (MapPointMarker mapPoint in this.clusterMarkers[level])
                    mapPoint.setDimension(dimension);
    }


    public void update()
    {
        int level;
        int c = 1;
        /*
        GameObject sphereModel = GameObject.Find("yarngroup");
        GameObject cylinderModel = GameObject.Find("Bobina2");
        cylinderModel.transform.position = cylinderModel.transform.position + new Vector3(0.0f, 50.0f, 0.0f);
        */

        if (clusterManager.hasData())
            return;

        updateClustering();


        distributeGroupsOnCircle();

        foreach (GridCluster gCluster in clusterManager.getPointGroupClusters())
        {
            MapPointMarker point = getClusterMarker(gCluster, -c);
            c++;
        }

        
        for (level = 0; level < clusterManager.getNumLevels(); level++)
        {
            this.clusterMarkers.Add(new List<MapPointMarker>());
            this.clusterLines.Add(new List<OnlineMapsDrawingLine>());
            // Each level , a list lines per cluster

            
            List<List<OnlineMapsDrawingLine>> levelConnections = new List<List<OnlineMapsDrawingLine>>();
            for (int a = 0; a < clusterManager.getGridClustersAtLevel(level).Count; a++)
                levelConnections.Add(new List<OnlineMapsDrawingLine>());

            connectionsLinesPerLevel.Add(level, levelConnections);  
        }

        for (level = 0; level < clusterManager.getNumLevels(); level++)
        {
            List<GridCluster> clusters = clusterManager.getGridClustersAtLevel(level);

            //            Debug.Log("En el nivel " + level + " hay " + clusters.Count + " cluster ");

            if (clusters.Count > 0 && !clusters[0].getCenter().isCluster())
            {

                for (int i = 0; i < clusters.Count; i++)
                {
                    // Creating cluster marker

                    MapPointMarker point = getClusterMarker(clusters[i], level * 1000 + i);

                    clusterMarkers[level].Add(point);
                    
                    
                    
                    if (level == 0)
                    {
                        clusters[i].initConnectionsList(clusters.Count);
                        clusters[i].updateConnections(clusters);

                        List<List<OnlineMapsDrawingLine>> levelConnections = (List<List<OnlineMapsDrawingLine>>)connectionsLinesPerLevel[level];

                        for (int clusterCon = 0; clusterCon < clusters.Count; clusterCon++)
                            if (clusters[i].getConnections()[clusterCon] == 0)
                                levelConnections[i].Add(null);
                            else
                                levelConnections[i].Add(addConnection(clusters[i], clusters[clusterCon], clusters[i].getConnections()[clusterCon]));


                    }

                }
            }

        }


        
    }

    protected void addMarkerInstance(MapPointMarker mapPointMarker) {


        mapPointMarker.assignTexture(null);
        GameObject markerGameObject = GameObject.Instantiate(customMarkerGameObject, customMarkerCanvas.transform) as GameObject;

        //gObject.transform.parent = mapPointMarker.getMarker3D().instance.transform;

        RectTransform rectTransform = markerGameObject.transform as RectTransform;
        //rectTransform.SetParent(customMarkerContainer);
        markerGameObject.transform.localScale = Vector3.one;

        MarkerInstance marker = new MarkerInstance();

        MarkerData data = new MarkerData();
        data.title = mapPointMarker.getGridCluster().getNumPoints().ToString();
        data.longitude = mapPointMarker.getX();
        data.latitude = mapPointMarker.getY();


        marker.data = data;
       
        marker.gameObject = markerGameObject;
        marker.transform = rectTransform;
        //marker.transform.Rotate(new Vector3(90, 180, 0));
        /*
        Quaternion quatRotation = Quaternion.identity;
        quatRotation.x = 45;
        quatRotation.y = 180;

        marker.transform.localRotation = quatRotation;*/

        marker.mapMarker = mapPointMarker;
        mapPointMarker.markerInstance = marker;
        mapPointMarker.getMarker2D().texture = null;
        mapPointMarker.getMarker2D().enabled = false;

        SetText(rectTransform, "Title", data.title);

        markers.Add(marker);
    }

    private void SetText(RectTransform rt, string childName, string value)
    {

        var title =  rt.GetComponentInChildren<Text>();
        if (title != null) title.text = value;
    }


    protected MapPointMarker getClusterMarker(GridCluster gCluster, int id)
    {
        MapPointMarker mapPoint = new MapPointMarker(gCluster.getCenter().getX(), gCluster.getCenter().getY(), clusterPrefab, true);
        mapPoint.setGridCluster(gCluster);

        

        //MapPointMarker mapPoint = new MapPointMarker(OnlineMapsMarker3DManager.CreateItem(
        // new Vector2(clusters[i].getCenter().getX(), clusters[i].getCenter().getY()), clusterPrefab));
        //mapPoint.getMarker3D().label = "Cluster " + i;

        mapPoint.getMarker3D().instance.name = id.ToString();
        mapPoint.setLabel("Cluster " + id);
            
        mapPoint.setClusteredPoints(gCluster.getPoints());

        mapPoint.setCluster(true);
        mapPoint.getMarker3D().altitude = 30.0f;
        mapPoint.getMarker3D().altitudeType = OnlineMapsAltitudeType.absolute;
        mapPoint.getMarker3D().scale = getScale(gCluster, this.points.Count);
        mapPoint.setMap(this);
        addMarkerInstance(mapPoint);


        gCluster.setCenter(mapPoint);

        //if (gCluster.isGroupPoints())
        //    Debug.Log("AQUI");

        if (gCluster.getCategory().Equals("silknow.org/#pthing"))
        {
            SphereCollider sphereCollider = mapPoint.getMarker3D().instance.GetComponent<SphereCollider>();
            sphereCollider.radius = 1;
            //mapPoint.getMarker3D().scale = mapPoint.getMarker3D().scale * 100.0f;
        }
        else
        {
            CapsuleCollider capsuleCollider = mapPoint.getMarker3D().instance.GetComponent<CapsuleCollider>();
            capsuleCollider.radius = 0.5f;
            capsuleCollider.height = 1.5f;
            capsuleCollider.direction = 1;
            mapPoint.getMarker3D().altitude = 70.0f;
            //mapPoint.getMarker3D().transform.position = mapPoint.getMarker3D().transform.position + new Vector3(0.0f, 60.0f, 0.0f);
            //mapPoint.getMarker3D().al

        }


        

        mapPoint.hide();

        return mapPoint;
    }


    public new void setViewerPosition(float x, float y)
    {
        base.setViewerPosition(x, y);
        OnlineMaps.instance.position = new Vector2(x, y);
    }

    private OnlineMapsDrawingLine addConnection(GridCluster clusterFrom, GridCluster clusterTo, int numConnections)
    {
        MapPoint from = clusterFrom.getCenter();
        MapPoint to = clusterTo.getCenter();


        List<Vector2> points = new List<Vector2>();

        points.Add(from.getVector2());
        points.Add(to.getVector2());

        

        OnlineMapsDrawingLine oLine = new OnlineMapsDrawingLine(points, Color.blue);
        oLine.width = 1.0f;
        oLine.visible = true;
        OnlineMapsDrawingElementManager.AddItem(oLine);
        oLine.visible = false;
        oLine.name = "connection";



        return oLine;



    }

    private void getRectEcuationParameters(MapPoint from, MapPoint to, ref float m, ref float b)
    {
        m = (from.getY() - to.getY()) / (from.getX() - to.getX());

        b = from.getY() - m * from.getX();

    }

    private void OnClusterClick(int id)
    {
        Debug.Log("Click ON " + id);
    }

    private void OnMapClick()
    {

        //Vector3 mousePosition = Input.mousePosition;

        // Converts the screen coordinates to geographic.
        //Vector3 mouseGeoLocation = OnlineMapsControlBase.instance.GetCoords(mousePosition);
        //double lng, lat;

        /*
        if (true)
            return;

        OnlineMapsControlBase.instance.GetCoords(out lng, out lat);
        Component omarker = OnlineMapsControlBase3D.instance.GetComponent("OnlineMapsMarker3D");
        
        GameObject selected=null;
                
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        // Casts the ray and get the first game object hit
      
        if (Physics.Raycast(ray, out hit, 1000))
            selected =  hit.transform.gameObject;
                




        String name = "null";

        if (selected != null  && (!selected.name.Equals("Map")))
        {

            Debug.Log("Se selecciona " + selected.name);

            name = selected.name;

           

            if (isPoint(name))
            {
                if (name.Contains("Cube"))
                    name = selected.transform.parent.name;

                if (name.Contains("(Clone)"))
                    name = name.Substring(0, name.IndexOf("("));

                Debug.Log("Ahora name vale " + name);
                //nt numPoint = Int32.Parse(name);
                //mPoint = numPoint * (-1);
                //MapPointMarker pointMarker = (MapPointMarker)points[numPoint];
                MapPointMarker pointMarker = (MapPointMarker)getPointPerLabel(name);
                Debug.Log("Ha seleccionado el punto " + pointMarker.getLabel());
                Debug.Log("EL marker3D esta " + pointMarker.getMarker3D().enabled);
                List<string> values = pointMarker.getPropertyValue("technique");
                foreach (string s in values)
                    Debug.Log("Valor de technique = " + s);

                
                MapItemPopup.instance.OnMarkerClick(pointMarker);
                SilkMap.Instance.changeCam();
            }
            else
            {
                GridCluster cluster = getClusterByName(name);
                if (cluster != null)
                {
                    Debug.Log("Seleccionado el cluster " + name + " con " + cluster.getNumPoints() + " datos");
                    int clusterClicked = Int32.Parse(name);
                    showLines(getConnectionLinesOfCluster(clusterClicked, 0));
                    if (selectedCluster != -1)
                        hideLines(getConnectionLinesOfCluster(selectedCluster, 0));
                    selectedCluster = clusterClicked;
                }
            }

        }
            
        */
        //Debug.Log("LLAMA AL ONCLICK2 en " + lng + " , " + lat + " hay un obj con id "+name);


    }

    public bool isPoint(string name)
    {
        //int numPoint = Int32.Parse(name);

        //if (numPoint < 0)
        if (name.Contains("Cube") || name.Contains("Clone"))
            return true;
        else
            return false;

    }

    public void showLines(List<OnlineMapsDrawingLine> lineList)
    {
        if (lineList!=null)
            for (int i = 0; i < lineList.Count; i++)        
                if (lineList[i]!=null)
                    lineList[i].visible = true;
    }

    public void hideLines(List<OnlineMapsDrawingLine> lineList)
    {
        if (lineList!=null)
            for (int i = 0; i < lineList.Count; i++)
                if (lineList[i] != null)
                    lineList[i].visible = false;
    }

    public GridCluster getClusterByName(String name)
    {
        GridCluster cluster = null;

        int numCluster = Int32.Parse(name);

        int level = (int)(numCluster / 1000);
        int position = numCluster % 1000;

        List<GridCluster> clusters = clusterManager.getGridClustersAtLevel(level);

        cluster = clusters[position];

        return cluster;
    }

    

    public int getScale(GridCluster cluster, int totalNumPoints)
    {
        int scaleMin = 5;
        int scaleMax = 40;
        int scale;

        int clusterPoints = cluster.getNumPoints();

        scale = scaleMin + (int) ((3*scaleMax*clusterPoints)/totalNumPoints);

        if (!cluster.isGroupPoints())            
            scale = scale / 4;

        return 20; // scale;
    }


    public override void showClustersAtZoom(float zoom)
    {
        int level = clusterManager.getLevel(zoom);

        if (level < clusterManager.getNumLevels()-2)
        {

            //Debug.Log("MapMarker-->showClustersAtZoom " + level);
            //Debug.Log(clusterMarkers.Count);

            if (this.clusterMarkers.Count > 0)
            {
                List<MapPointMarker> clusterList = this.clusterMarkers[level];
                List<OnlineMapsDrawingLine> clusterLineList = this.clusterLines[level];

                for (int i = 0; i < points.Count; i++)
                    points[i].hide();

                List<GridCluster> clusters = clusterManager.getGridClustersAtLevel(level);

                //Debug.Log("Se muestran los clusters "+clusterList.Count);
                for (int i = 0; i < clusterList.Count; i++)
                {
                    clusterList[i].getMarker3D().scale = scaleCorrection(clusters[i], this.points.Count, zoom);
                    if (!clusters[i].getCategory().Equals("silknow.org/#pthing"))
                        clusterList[i].getMarker3D().scale *= 2.0f;

                    if (clusterList[i].getGridCluster()!=null)
                        clusterList[i].getGridCluster().getCenter().show();
                    clusterList[i].show();
                    
                    //clusterList[i].getGridCluster().showRelations();
                    //clusterLineList[i].visible = true;
                }


                if (drawTheClustersQuad && quadsDrawed.Count == 0)
                {
                    for (int q = 0; q < clusterManager.getMapLevel(level).getNumQuads(); q++)
                    {
                        drawQuad(clusterManager.getMapLevel(level).getPointsOfQuad(q), q);
                    }
                }

                foreach (MapPoint p in pointsWithRelation)
                {
                    //p.hideAllRelations();
                }
            }
        }
        else
        {
            for (int i = 0; i < points.Count; i++)
            {
                float corrector = 1.0f;

                if (level == 1) //zoom > 4 && zoom <= 6)
                    corrector = 3.25f;

                if (level == 2) //zoom > 6 && zoom <= 8)
                    corrector = 3.25f;

                if (level == 3) //zoom > 8)
                    corrector = 3.5f;

                if (((MapPointMarker)(points[i])).getMarker3D()!=null)
                    ((MapPointMarker)(points[i])).getMarker3D().scale = 6* corrector;               
                points[i].show();
            }

            foreach (MapPoint p in clusterManager.getPointGroupsPoints())
                p.show();

            foreach (MapPoint p in pointsWithRelation)
            {
                //p.showAllRelations();
            }
        }
    }

    public override void hideClustersAtZoom(float zoom)
    {
        int level = clusterManager.getLevel(zoom);

        if (level < clusterManager.getNumLevels()-2)
        {
            for (int i = 0; i < points.Count; i++)
                points[i].hide();

            foreach (MapPoint p in clusterManager.getPointGroupsPoints())
                p.hide();

            //Debug.Log("MapMarker-->hideClustersAtZoom " + level);
            //Debug.Log(clusterMarkers.Count);

            if (this.clusterMarkers.Count > 0)
            {
                List<MapPointMarker> clusterList = this.clusterMarkers[level];
                List<OnlineMapsDrawingLine> clusterLineList = this.clusterLines[level];

                for (int i = 0; i < clusterList.Count; i++)
                {
                    if (clusterList[i].getGridCluster() != null)
                        clusterList[i].getGridCluster().getCenter().hide();
                    clusterList[i].hide();
                    
                    //clusterList[i].getGridCluster().hideRelations();
                    //clusterLineList[i].visible = false;
                }

                for (int q = 0; q < quadsDrawed.Count; q++)
                {
                    OnlineMapsDrawingLine oLineQuad = quadsDrawed[q];
                    oLineQuad.visible = false;
                    OnlineMapsDrawingElementManager.RemoveItem(oLineQuad);
                }

                quadsDrawed.RemoveRange(0, quadsDrawed.Count);
            }
        }


    }

    public List<MapPointMarker> getClusterMarkersAtLevel(int level)
    {
        if (this.clusterMarkers.Count > 0)
            return this.clusterMarkers[level];
        else
            return null;
    }

    private void drawQuad(List<MapPoint> quadPoints, int q)
    {
        List<Vector2> points = new List<Vector2>();
        points.Add(new Vector2(quadPoints[0].getX(), quadPoints[0].getY()));
        points.Add(new Vector2(quadPoints[1].getX(), quadPoints[1].getY()));
        points.Add(new Vector2(quadPoints[2].getX(), quadPoints[2].getY()));
        points.Add(new Vector2(quadPoints[3].getX(), quadPoints[3].getY()));


        OnlineMapsDrawingLine oLine = new OnlineMapsDrawingLine(points, Color.green);

        oLine.width = 2.0f;
        oLine.visible = true;
        OnlineMapsDrawingElementManager.AddItem(oLine);

        quadsDrawed.Add(oLine);
    }

    public int scaleCorrection(GridCluster cluster, int numPoints, float zoom)
    {
        int scale = getScale(cluster, this.points.Count);

       // Debug.Log(zoom);

        float corrector = 1.0f;

        int level = clusterManager.getLevel(zoom);
        
        if (level==1) //zoom > 4 && zoom <= 6)
            corrector = 2.0f;

        if (level==2) //zoom > 6 && zoom <= 8)
            corrector = 2.5f;

        if (level==3) //zoom > 8)
            corrector = 3.0f;

        scale = (int)(scale * corrector);

        //if (!cluster.getCategory().Equals("silknow.org/#pthing"))
          //  scale = scale * 2;


        return scale;
    }

    public override void removeAllGraphicClusters()
    {
        if (this.clusterMarkers.Count > 0)
        {
            for (int level = 0; level < clusterManager.getNumLevels(); level++)
            {
                List<MapPointMarker> clusterList = this.clusterMarkers[level];
                //List<OnlineMapsDrawingLine> clusterLineList = this.clusterLines[level];


                //List<GridCluster> clusters = clusterManager.getGridClustersAtLevel(level);


                for (int i = 0; i < clusterList.Count; i++)
                    clusterList[i].reset();

                clusterList.Clear();
            }
        }

    }

    public override void changeProjection(int dimension)
    {
        if (dimension == MapPoint.TWO_DIMENSION)
        {
            Camera.main.orthographic = true;
            Camera.main.orthographicSize = 287.5f;
            OnlineMapsCameraOrbit.instance.rotation = new Vector2(0, 0);
        }
        else
        {
            Camera.main.orthographic = false;
            OnlineMapsCameraOrbit.instance.rotation = new Vector2(35, 0);
        }

        update();

    }

    [Serializable]
    public class MarkerData
    {
        public string title;
        public float longitude;
        public float latitude;


    }

    public class MarkerInstance
    {
        public MarkerData data;
        public GameObject gameObject;
        public RectTransform transform;
        public MapPointMarker mapMarker;
    }

}
