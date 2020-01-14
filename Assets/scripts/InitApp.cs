using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Clustering;

public class InitApp : MonoBehaviour {

    /// <summary>
    /// Prefab of 3D marker
    /// </summary>

    public static int MAX_POINTS = 20; //5
    public static int MAX_HOT_POINTS = 10; // 4
    public static int MAX_CLUSTERS = MAX_POINTS * MAX_HOT_POINTS;
    public static float CLUSTER_DISTANCE = 10.0f;

    List<MapPoint> mapPoints = new List<MapPoint>();
    private MapMarker map;



    private void OnGUI()
    {
        //OnlineMaps.instance.zoom = 6;
        map.setViewerZoom(OnlineMaps.instance.zoom);
    }

    private void LogTypeList()
    {
        // Gets all providers
        OnlineMapsProvider[] providers = OnlineMapsProvider.GetProviders();
        foreach (OnlineMapsProvider provider in providers)
        {
            Debug.Log(provider.id);
            foreach (OnlineMapsProvider.MapType type in provider.types)
            {
                Debug.Log(type);
            }
        }
    }


    private void generateRandomMarkers()
    {
        int i = 0;
        int j = 0;
        int maxGen = 0;
        float longitud, latitud;
        float longitudP, latitudP;
        int cat;
        GameObject cube = GameObject.Find("Cube");
        string[] category = { "silknow.org/#pthing", "silknow.org/#man" };
        int numCat0 = 0, numCat1 = 0;


        for (i = 0; i < MAX_HOT_POINTS; i++)
        {
            longitud = UnityEngine.Random.Range(-8.0f, 36.0f);
            latitud = UnityEngine.Random.Range(35.0f, 56.0f);


            if (i == 0)
            {
                longitud = -0.389f;
                latitud = 39.416f;
                maxGen = 5;
            }
            else
                maxGen = UnityEngine.Random.Range(1, MAX_POINTS);

            for (j = 0; j < maxGen; j++)
            {
                longitudP = longitud + UnityEngine.Random.Range(-5, 5);
                latitudP = latitud + UnityEngine.Random.Range(-5, 5);
                cat = UnityEngine.Random.Range(-10, 10);

                if (cat <= 0)
                {
                    cat = 0;
                    numCat0++;
                }
                else
                {
                    cat = 1;
                    numCat1++;
                }


                mapPoints.Add(new MapPointMarker(OnlineMapsMarker3DManager.CreateItem(new Vector2(longitudP, latitudP), cube)));
                ((MapPointMarker)(mapPoints[mapPoints.Count - 1])).getMarker3D().altitude = 30.0f; // 0.0f; // 30.0f;
                ((MapPointMarker)(mapPoints[mapPoints.Count - 1])).getMarker3D().scale = 3.0f; // 1.0f; // 3.0f;
                BoxCollider box = ((MapPointMarker)(mapPoints[mapPoints.Count - 1])).getMarker3D().instance.GetComponent<BoxCollider>();
                box.size = new Vector3(0.5f, 0.5f, 0.5f);

                int posPoint = mapPoints.Count - 1;

                mapPoints[mapPoints.Count - 1].setURI("http://silknow.org/PhysicalManMade#" + posPoint.ToString());
                mapPoints[mapPoints.Count - 1].setCategory(category[cat]);
                mapPoints[mapPoints.Count - 1].setFrom(UnityEngine.Random.Range(1650, 1680));
                mapPoints[mapPoints.Count - 1].setTo(UnityEngine.Random.Range(1700, 1750));



                if (j > 10)
                {
                    int numConnections = UnityEngine.Random.Range(0, 5);

                    for (int c = 0; c < numConnections; c++)
                    {
                        int connectedWith = UnityEngine.Random.Range(0, j);
                        mapPoints[mapPoints.Count - 1].addRelationWith(mapPoints[connectedWith], "http://silknow.org/propName");
                    }
                }

            }

        }

        Debug.Log("Hay " + mapPoints.Count + " Puntos");
        Debug.Log("Hay " + numCat0 + " de " + category[0]);
        Debug.Log("Hay " + numCat1 + " de " + category[1]);
    }

    private void initMarkers()
    {

        generateRandomMarkers();

        map = new MapMarker();
        map.setGridingZoomData(3, 10, 4);
        map.setGridingQuadsHorizonal(64);
        map.fixZoomInterval(OnlineMaps.instance, 3, 14);
        map.fixPositionInterval(OnlineMaps.instance, 28.24f, -16.79f, 71.19f, 48.48f);
        map.setViewerPosition(23.42f, 46.2f);
        map.addPoints(mapPoints);
        map.update();
        map.setViewerZoom(OnlineMaps.instance.zoom);

        SilkMap.Instance.map = map;

        map.showClusters();

        /*
        map.reset();

        for (int i = 0; i < mapPoints.Count; i++)
            mapPoints[i].reset();
        mapPoints.RemoveRange(0, mapPoints.Count);

        generateRandomMarkers();

        map.addPoints(mapPoints);
        map.update();*/


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
    }

    private void OnMarkerClick(OnlineMapsMarkerBase marker)
    {
        // Show in console marker label.
        OnlineMapsMarker3D om = (OnlineMapsMarker3D)marker;

        Debug.Log(marker.label + " : " + marker.position.x + "," + marker.position.y);
    }

    private void Start()
    {
        // Get instance of OnlineMapsControlBase3D (Texture or Tileset)
        OnlineMapsControlBase3D control = OnlineMapsControlBase3D.instance;

        if (control == null)
        {
            Debug.LogError("You must use the 3D control (Texture or Tileset).");
            return;
        }

        SilkMap.Instance.init();
        OnlineMaps.instance.SetPosition(14.67986, 45.55115);
        OnlineMapsCameraOrbit.instance.lockPan = true;
        OnlineMapsCameraOrbit.instance.lockTilt = true;

        //LogTypeList();

        initMarkers();

    }
}








