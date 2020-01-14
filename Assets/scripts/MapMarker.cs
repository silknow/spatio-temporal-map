using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapMarker : Map {

    protected List<List<MapPointMarker>> clusterMarkers = new List<List<MapPointMarker> >();
    protected List<List<OnlineMapsDrawingLine>> clusterLines = new List<List<OnlineMapsDrawingLine>> ();
    protected List<List<OnlineMapsDrawingLine>> connectionLines = new List<List<OnlineMapsDrawingLine>>();
    protected int selectedCluster = -1;
    protected List<OnlineMapsDrawingLine> quadsDrawed = new List<OnlineMapsDrawingLine>();

    protected Hashtable connectionsLinesPerLevel = new Hashtable();
    private bool drawTheClustersQuad = false;

    public MapMarker()
    {
        OnlineMapsControlBase.instance.OnMapClick += OnMapClick;
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
        return levelConnections[clusterPosition]; 
    }

    public new void reset()
    {
        for (int i=0;i<points.Count;i++)
        {
            MapPointMarker point = (MapPointMarker)points[i];
            UnityEngine.Object.Destroy(point.getMarker3D().prefab);
            point.getMarker3D().DestroyInstance();
        }

        for (int i=0;i<clusterMarkers.Count;i++)
        {
            List< MapPointMarker> clusterMarkersList = clusterMarkers[i];

            for (int j=0;j<clusterMarkersList.Count;j++)
            {
                UnityEngine.Object.Destroy(clusterMarkersList[j].getMarker3D().prefab);
                clusterMarkersList[j].reset();
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

    public void update()
    {
        int level;
        GameObject sphereModel = GameObject.Find("yarngroup");
        GameObject cylinderModel = GameObject.Find("Bobina2");
        cylinderModel.transform.position = cylinderModel.transform.position + new Vector3(0.0f, 50.0f, 0.0f);

        updateClustering();

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

            Debug.Log("En el nivel " + level + " hay " + clusters.Count + " cluster ");

            for (int i = 0; i < clusters.Count; i++)
            {

                GameObject cube;

                if (clusters[i].getCategory().Equals("silknow.org/#pthing"))
                    cube = sphereModel;
                else
                    cube = cylinderModel;

                // Creating cluster marker
                MapPointMarker mapPoint = new MapPointMarker(OnlineMapsMarker3DManager.CreateItem(
                        new Vector2(clusters[i].getCenter().getX(), clusters[i].getCenter().getY()), cube));
                mapPoint.getMarker3D().label = "Cluster " + i;
                mapPoint.getMarker3D().altitude = 30.0f;
                mapPoint.getMarker3D().altitudeType = OnlineMapsAltitudeType.absolute;
                mapPoint.getMarker3D().scale = getScale(clusters[i], this.points.Count);

                int id = level * 1000 + i;
                mapPoint.getMarker3D().instance.name = id.ToString();

                if (clusters[i].getCategory().Equals("silknow.org/#pthing"))
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
                clusterMarkers[level].Add(mapPoint);

                // Creating cluster lines
                /*
                List<Vector2> points = new List<Vector2>();
                Rect clusterZone = clusters[i].getZone();

                points.Add(new Vector2(clusterZone.xMin, clusterZone.yMax));
                points.Add(new Vector2(clusterZone.xMin + clusterZone.width, clusterZone.yMax));
                points.Add(new Vector2(clusterZone.xMin + clusterZone.width, clusterZone.yMin));
                points.Add(new Vector2(clusterZone.xMin, clusterZone.yMin));
                points.Add(new Vector2(clusterZone.xMin, clusterZone.yMax));

                OnlineMapsDrawingLine oLine = new OnlineMapsDrawingLine(points, Color.red);
                oLine.width = 2.0f;
                oLine.visible = true;
                OnlineMapsDrawingElementManager.AddItem(oLine);
                oLine.visible = false;
                clusterLines[level].Add(oLine);
                */


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
        double lng, lat;
        OnlineMapsControlBase.instance.GetCoords(out lng, out lat);
        Component omarker = OnlineMapsControlBase3D.instance.GetComponent("OnlineMapsMarker3D");
        
        GameObject selected=null;
                
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        // Casts the ray and get the first game object hit
      
        if (Physics.Raycast(ray, out hit, 1000))
            selected =  hit.transform.gameObject;
                
            /*
        RaycastHit[] hits;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        hits = Physics.RaycastAll(Input.mousePosition, ray.direction, 100.0F);

        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            Debug.Log("choca " + i + " con " + hit.transform.gameObject.name);
        }*/



        String name = "null";

        if (selected != null  && (!selected.name.Equals("Map")))
        {
            Debug.Log("Se selecciona " + selected.name);
            name = selected.name;
            if (isPoint(name))
            {
                int numPoint = Int32.Parse(name);
                numPoint = numPoint * (-1);
                MapPointMarker pointMarker = (MapPointMarker)points[numPoint];
                Debug.Log("Ha seleccionado el punto " + numPoint);

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
            

        //Debug.Log("LLAMA AL ONCLICK2 en " + lng + " , " + lat + " hay un obj con id "+name);


    }

    public bool isPoint(string name)
    {
        int numPoint = Int32.Parse(name);

        if (numPoint < 0)
            return true;
        else
            return false;

    }

    public void showLines(List<OnlineMapsDrawingLine> lineList)
    {
        for (int i = 0; i < lineList.Count; i++)        
            if (lineList[i]!=null)
                lineList[i].visible = true;
    }

    public void hideLines(List<OnlineMapsDrawingLine> lineList)
    {
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


        return scale;
    }


    public override void showClustersAtZoom(float zoom)
    {
        int level = clusterManager.getLevel(zoom);

        if (level < clusterManager.getNumLevels()-2)
        {

            Debug.Log("MapMarker-->showClustersAtZoom " + level);
            Debug.Log(clusterMarkers.Count);

            List<MapPointMarker> clusterList = this.clusterMarkers[level];
            List<OnlineMapsDrawingLine> clusterLineList = this.clusterLines[level];

            for (int i = 0; i < points.Count; i++)
                points[i].hide();

            List<GridCluster> clusters = clusterManager.getGridClustersAtLevel(level);


            for (int i = 0; i < clusterList.Count; i++)
            {
                clusterList[i].getMarker3D().scale = scaleCorrection(clusters[i], this.points.Count, zoom);
                if (!clusters[i].getCategory().Equals("silknow.org/#pthing"))
                    clusterList[i].getMarker3D().scale *= 2.0f;

                clusterList[i].show();
                //clusterLineList[i].visible = true;
            }


            if (drawTheClustersQuad && quadsDrawed.Count == 0)
            {
                for (int q = 0; q < clusterManager.getMapLevel(level).getNumQuads(); q++)
                {
                    drawQuad(clusterManager.getMapLevel(level).getPointsOfQuad(q), q);
                }
            }
        }
        else
        {
            for (int i = 0; i < points.Count; i++)
            {
                float corrector = 1.0f;

                if (level == 1) //zoom > 4 && zoom <= 6)
                    corrector = 1.25f;

                if (level == 2) //zoom > 6 && zoom <= 8)
                    corrector = 1.25f;

                if (level == 3) //zoom > 8)
                    corrector = 1.5f;
                ((MapPointMarker)(points[i])).getMarker3D().scale = 5* corrector;
                points[i].show();
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
            Debug.Log("MapMarker-->hideClustersAtZoom " + level);
            Debug.Log(clusterMarkers.Count);

            List<MapPointMarker> clusterList = this.clusterMarkers[level];
            List<OnlineMapsDrawingLine> clusterLineList = this.clusterLines[level];

            for (int i = 0; i < clusterList.Count; i++)
            {
                clusterList[i].hide();
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

    public List<MapPointMarker> getClusterMarkersAtLevel(int level)
    {
        return this.clusterMarkers[level];
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


}
