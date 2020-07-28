using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Clustering;
using UnityEngine.Events;
using SilknowMap;

public class InitApp : MonoBehaviour {

    /// <summary>
    /// Prefab of 3D marker
    /// </summary>

    // Constantes de datos genericos
    // Número máximo de puntos por cluster
    public static int MAX_POINTS = 20; //5

    // Número máximo de puntos, que sirven de base para generar los puntos aleatorios
    public static int MAX_HOT_POINTS = 10; // 4

    //public static int MAX_CLUSTERS = MAX_POINTS * MAX_HOT_POINTS;
    //public static float CLUSTER_DISTANCE = 10.0f;

    // Lista de puntos en el mapa
    List<MapPoint> mapPoints = new List<MapPoint>();

    // Instancia de MapMarker que gestiona el mapa
    private MapMarker map;

    public GameObject prefabObject;
    public GameObject prefabCluster;
    
    public GameObject clonedMapGroup;
    
    // Event to be triggered when data is loaded to the map
    [Header("Event to be triggered when data is loaded to the map")]
    public UnityEvent loadedDataEvent;

    private void OnGUI()
    {
        //OnlineMaps.instance.zoom = 6;
        // Reajusta el mapa de acorde al zoom del sistema.
        map.setViewerZoom(OnlineMaps.instance.zoom);
    }

    // Método que muestra por consola los proveedores de mapas admitidos
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


    // Método que genera los puntos aleatorios en la lista mapPoints
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


                // Cada MapPoint tiene una serie de propiedades, la posición, escala y altitud vienen dentro de un objeto
                // MapPointMarker de OnlineMaps 
                // Se añade la posicion (lat,long) y se asocia a un objeto 3d Cube, luego puede cambiarse
         
                mapPoints.Add(new MapPointMarker(OnlineMapsMarker3DManager.CreateItem(new Vector2(longitudP, latitudP), prefabObject)));
                // Altitud del mar = 30.0f
                ((MapPointMarker)(mapPoints[mapPoints.Count - 1])).getMarker3D().altitude = 30.0f; // 0.0f; // 30.0f;
                // Scale =3.0f, luego se cambia
                ((MapPointMarker)(mapPoints[mapPoints.Count - 1])).getMarker3D().scale = 3.0f; // 1.0f; // 3.0f;
                // Se le asigna un collider
                BoxCollider box = ((MapPointMarker)(mapPoints[mapPoints.Count - 1])).getMarker3D().instance.GetComponent<BoxCollider>();
                box.size = new Vector3(0.5f, 0.5f, 0.5f);

                int posPoint = mapPoints.Count - 1;

                // Se introducen el resto de propiedades (URI, categpry (clase), from y to (intervalo de tiempo)
                mapPoints[mapPoints.Count - 1].setURI("http://silknow.org/PhysicalManMade#" + posPoint.ToString());
                mapPoints[mapPoints.Count - 1].setCategory(category[cat]);
                // Si sólo sabemos un año from=to=ese año
                mapPoints[mapPoints.Count - 1].setFrom(UnityEngine.Random.Range(1650, 1680));
                mapPoints[mapPoints.Count - 1].setTo(UnityEngine.Random.Range(1700, 1750));


                // Conexiones con otros mappoints
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

    // Método que instancia MapMarker, establece los parametros de inicio y
    // añade los puntos a representar desde la lista mapPoints
    private void initMarkers()
    {

        //generateRandomMarkers();

        map = new MapMarker();
        map.clusterPrefab = prefabCluster;
        map.setGridingZoomData(3, 10, 4);
        map.setGridingQuadsHorizonal(64);
        map.fixZoomInterval(OnlineMaps.instance, 3, 14);
        map.fixPositionInterval(OnlineMaps.instance, 28.24f, -16.79f, 71.19f, 48.48f);
        map.setViewerPosition(23.42f, 46.2f);
        Vector2 br = OnlineMaps.instance.bottomRightPosition;
        Vector2 tl = OnlineMaps.instance.topLeftPosition;
        int zoomi = OnlineMaps.instance.zoom;
        
        //map.addPoints(mapPoints);
        //map.update();
        //APIManager.instance.OnButtonTestClick();
        map.setViewerZoom(OnlineMaps.instance.zoom);

        SilkMap.Instance.map = map;
        //ClonedMap.instance.setMap(map);
        if (clonedMapGroup != null) 
            SilkMap.instance.clonedGroupMap = clonedMapGroup;
        
        //map.showClusters();

        map.SetDimension(2);

        UpdatePropertyManager(SilkMap.instance.map);


        /*map.reset();

        for (int i = 0; i < mapPoints.Count; i++)
            mapPoints[i].reset();
        mapPoints.RemoveRange(0, mapPoints.Count);

        generateRandomMarkers();

        map.addPoints(mapPoints);
        map.update();*/


        ///StartCoroutine("RemoveAllMarkers");
    }

    // Método que dibuja Quad
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

    // Método callback llamado cada vez que se click un punto del mapa
    private void OnMarkerClick(OnlineMapsMarkerBase marker)
    {
        // Show in console marker label.
        OnlineMapsMarker3D om = (OnlineMapsMarker3D)marker;

        Debug.Log(marker.label + " : " + marker.position.x + "," + marker.position.y);
    }

    // Función principal inicia las instancias de OnlineMas, y carga los datos
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
        //OnlineMaps.instance.SetPosition(3.05, 46.21);
        OnlineMaps.instance.SetPositionAndZoom(13.896, 47.527,4.5f);
        Camera.main.orthographicSize = 287;
        OnlineMapsCameraOrbit.instance.lockPan = true;
        OnlineMapsCameraOrbit.instance.lockTilt = true;

        //LogTypeList();

        initMarkers();

    }

    public void UpdatePropertyManager(Map map)
    {
        //map.GetPropertyManager().AddProperty("description.language", false, false, 0, 0, false, false,false);
        map.GetPropertyManager().AddProperty("description", true, false, 0, 1, false, false);
        map.GetPropertyManager().AddProperty("technique", true, true, 1, 2, false, false, true, Color.blue);
        map.GetPropertyManager().AddProperty("material", true, true, 2, 3, false, false, true, Color.red);
        map.GetPropertyManager().AddProperty("time", true, true, 3, 4, false, false,false,Color.green);
        map.GetPropertyManager().AddProperty("place", true, false, 0, 5, false, true, true, Color.grey);
        map.GetPropertyManager().AddProperty("img", true, false, 0, 6, true, false);
     }

    public void LoadRestData(List<ManMadeObject> objectList)
    {
        float longitud, latitud;
        string[] category = { "silknow.org/#pthing", "silknow.org/#man" };
        mapPoints.Clear();
        map.reset();
        foreach (var obj in objectList)
        {
            if (obj.production == null || obj.production.location == null)
                continue;
            string lat = obj.production.location[0].lat;
            string lg = obj.production.location[0].@long;
            if(string.IsNullOrEmpty(lat) || string.IsNullOrEmpty(lg))
                continue;
            longitud = float.Parse(lg, CultureInfo.InvariantCulture);
            latitud = float.Parse(lat, CultureInfo.InvariantCulture);
            
            // Cada MapPoint tiene una serie de propiedades, la posición, escala y altitud vienen dentro de un objeto
            // MapPointMarker de OnlineMaps 
            // Se añade la posicion (lat,long) y se asocia a un objeto 3d Cube, luego puede cambiarse
            var mapPoint = new MapPointMarker(longitud, latitud, prefabObject,false);
            // Altitud del mar = 30.0f
            mapPoint.getMarker3D().altitude = 30.0f;
            // Scale =3.0f, luego se cambia
            mapPoint.getMarker3D().scale = 3.0f;
            
            // Se introducen el resto de propiedades (URI, categpry (clase), from y to (intervalo de tiempo)
            mapPoint.setURI(obj.id);
            mapPoint.setCategory(category[1]);
            mapPoint.setLabel(obj.label ?? obj.identifier);
;
            // Check for century Dictionary
            if (APIManager.instance.timeValues.TryGetValue(obj.production.time[0], out var time))
            {
                mapPoint.setFrom(time.@from);
                mapPoint.setTo(time.to);
            }

            //Set Properties values
            map.GetPropertyManager().SetPropertyValue("technique", mapPoint, obj.production.technique);
            map.GetPropertyManager().SetPropertyValue("material", mapPoint, obj.production.material);
            map.GetPropertyManager().SetPropertyValue("place", mapPoint, obj.production.location[0].country);
            map.GetPropertyManager().SetPropertyValue("time", mapPoint, obj.production.time[0]);

            mapPoint.setDimension(map.GetDimension());
            mapPoints.Add(mapPoint);
        }
        map.addPoints(mapPoints);

        ResetMapParameters();
        map.update();

        loadedDataEvent.Invoke();
    }

    IEnumerator RemoveAllMarkers()
    {
        yield return new WaitForSeconds(.1f);
        print("Borrando todos los markers");
        map.reset();

        // Este for y el clear ya lo hace el map.reset()
        //for (int i = 0; i < mapPoints.Count; i++)
        //   mapPoints[i].reset();
        //mapPoints.Clear();
        //mapPoints.RemoveRange(0, mapPoints.Count);

        //map.update();

    }
    public void RemoveAllMarkersOnClick()
    {
        print("Borrando todos los markers");
        map.reset();

        for (int i = 0; i < mapPoints.Count; i++)
            mapPoints[i].reset();
        mapPoints.RemoveRange(0, mapPoints.Count);
        map.update();
      
    }

    public void ResetMapParameters (){
        /*
        map.setGridingZoomData(3, 10, 4);
        map.setGridingQuadsHorizonal(64);
        map.fixZoomInterval(OnlineMaps.instance, 3, 14);
        map.fixPositionInterval(OnlineMaps.instance, 28.24f, -16.79f, 71.19f, 48.48f);
      
        */
        SilkMap.Instance.map = map;
        CenterMapOnData();
        //map.showClusters();
    }
    public void CenterMapOnData(){
        Vector2 center;
        int zoom;

        var listOfMarkers = mapPoints.Select(t => (t as MapPointMarker).getMarker2D());

        //Debug.LogFormat("OnlineMapsMarkerManager count : {0}",OnlineMapsMarkerManager.instance.items.Count);
        // Get the center point and zoom the best for all markers.
        OnlineMapsUtils.GetCenterPointAndZoom(listOfMarkers.ToArray(), out center, out zoom);

        //Debug.LogFormat("Zoom Level: {0}",zoom);
        //Debug.LogFormat("Center: {0} ; {1}",center.x,center.y);
        // Change the position and zoom of the map.
        
        
        map.fixZoomInterval(OnlineMaps.instance, 3, Mathf.Max(zoom,14));
        map.setViewerZoom(zoom);
        map.setViewerPosition(center.x,center.y);
    }
    public void ToggleDimension ()
    {
        var dim = map.GetDimension() == 2 ? 3 : 2;
        map.SetDimension(dim);
    }
}








