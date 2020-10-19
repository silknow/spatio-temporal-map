using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Clustering;
using Honeti;
using UnityEngine.Events;
using SilknowMap;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class InitApp : MonoBehaviour
{

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
    public GameObject customMarkerGameObject;
    public Canvas customMarkerCanvas;

    // Event to be triggered when data is loaded to the map
    [Header("Event to be triggered when data is loaded to the map")]
    public UnityEvent loadedDataEvent;

    public Dictionary<string, Texture> iconsTexture = new Dictionary<string, Texture>();


    public List<Texture2D> customIcons;
    
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
        //map.fixPositionInterval(OnlineMaps.instance, 28.24f, -16.79f, 71.19f, 48.48f);
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

        OnlineMapsMarkerManager.instance.defaultTexture = getDefaultTexture();




        /*map.reset();

        for (int i = 0; i < mapPoints.Count; i++)
            mapPoints[i].reset();
        mapPoints.RemoveRange(0, mapPoints.Count);

        generateRandomMarkers();

        map.addPoints(mapPoints);
        map.update();*/


        ///StartCoroutine("RemoveAllMarkers");
    }



    public void onClick()
    {
        Debug.Log("STACKED CLICK");
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
        //Update map labels;
        print(I18N.instance.gameLang.ToString());
        OnlineMaps.instance.language = I18N.instance.gameLang.ToString().ToLower(CultureInfo.InvariantCulture);
        
        
        // Get instance of OnlineMapsControlBase3D (Texture or Tileset)
        OnlineMapsControlBase3D control = OnlineMapsControlBase3D.instance;

        if (control == null)
        {
            Debug.LogError("You must use the 3D control (Texture or Tileset).");
            return;
        }

        List<string> textureFileNames = new List<string>();
        textureFileNames.Add("vestidop.png");
        textureFileNames.Add("trajep.png");
        textureFileNames.Add("trajepi.png");

        StartCoroutine(GetTexture(textureFileNames));

        SilkMap.Instance.init();
        //OnlineMaps.instance.SetPosition(3.05, 46.21);
        OnlineMaps.instance.SetPositionAndZoom(13.896, 47.527,4.5f);
        Camera.main.orthographicSize = 287;
        OnlineMapsCameraOrbit.instance.lockPan = true;
        OnlineMapsCameraOrbit.instance.lockTilt = true;

        //LogTypeList();

        initMarkers();

        map.customMarkerCanvas = customMarkerCanvas;
        map.customMarkerGameObject = customMarkerGameObject;

        customIcons = new List<Texture2D>();
        customIcons = Resources.LoadAll<Texture2D>("object_groups").ToList();
    }

    public void UpdatePropertyManager(Map map)
    {
        //map.GetPropertyManager().AddProperty("description.language", false, false, 0, 0, false, false,false);
        map.GetPropertyManager().AddProperty("description", true, false, 0, 1, false, false);
        map.GetPropertyManager().AddProperty("technique", true, true, 1, 2, false, false, true, Color.blue);
        map.GetPropertyManager().AddProperty("material", true, true, 2, 3, false, false, true, Color.red);
        map.GetPropertyManager().AddProperty("time", true, true, 3, 4, false, false,true,Color.green);
        map.GetPropertyManager().AddProperty("place", true, false, 0, 5, false, true, false, Color.grey);
        map.GetPropertyManager().AddProperty("img", true, false, 0, 6, true, false);
     }


    public void changeMapDataPosition(string positionName)
    {
        map.setMaintainPoints(true);
        map.activePosition(positionName);
        map.reset();
        ResetMapParameters();
        map.update();
    }


        //https://silknow.org/silknow/media/icons/vestido.png


    IEnumerator GetTexture(List<string> textureFileNames)
    {
        

        string iconsPath = "https://silknow.org/silknow/media/icons/";
        foreach (string textureFileName in textureFileNames)
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(iconsPath+ textureFileName);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Texture texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                iconsTexture.Add(textureFileName, texture);
            }
        }
        
    }

    public void LoadRestData(List<ManMadeObject> objectList)
    {
        float longitud, latitud;
        string[] category = { "silknow.org/#pthing", "silknow.org/#man" };
        mapPoints.Clear();
        map.reset();
        map.addPositionName("Production");
        map.addPositionName("Actual");
        int iconRandom = 0;


        // Para cambiar mapa con posicion name.........................
        // map.setMaintainPoints(true);
        // map.activePosition(positionName);
        // map.reset();
        // ResetMapParameters();
        // map.update();

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

            mapPoint.addPositionValue("Production", new Vector2(longitud, latitud));
            mapPoint.addPositionValue("Actual", new Vector2(longitud + 1, latitud + 1));

            // Altitud del mar = 30.0f
            mapPoint.getMarker3D().altitude = 30.0f;
            // Scale =3.0f, luego se cambia
            mapPoint.getMarker3D().scale = 3.0f;
            mapPoint.setMap(map);
            
            /* JAVI - Textura desde web *
            
            if (iconRandom % 7 == 0)
                mapPoint.setKnownLocation(false);


            if (iconRandom % 2 == 0)
                mapPoint.assignTexture(iconsTexture["vestidop.png"]);
            else
            {
                if (mapPoint.isKnownLocation())
                    mapPoint.assignTexture(iconsTexture["trajep.png"]);
                else
                    mapPoint.assignTexture(iconsTexture["trajepi.png"]);
            }

            iconRandom++;
            
            /* JAVI - Textura desde web */

            /* PABLO - Texturas desde editor */
             if (iconRandom == (customIcons.Count-1)) 
                 iconRandom = 0;
             mapPoint.setKnownLocation(true);
             mapPoint.assignTexture(customIcons[iconRandom]);
             iconRandom++;
             
             if (obj.production.location.Count > 1)
             {
                 mapPoint.setKnownLocation(false);
                 mapPoint.assignTexture(customIcons[customIcons.Count -1]);
             }
             /*  PABLO - Texturas desde editor */

            // Se introducen el resto de propiedades (URI, categpry (clase), from y to (intervalo de tiempo)
            mapPoint.setURI(obj.id);
            mapPoint.setCategory(category[1]);
            mapPoint.setLabel(obj.label ?? obj.identifier);
            if (obj.production.time.Count >0)
            {
                // Check for century Dictionary
                if (APIManager.instance.timeValues.TryGetValue(obj.production.time[0], out var time))
                {
                    mapPoint.setFrom(time.@from);
                    mapPoint.setTo(time.to);
                }
                map.GetPropertyManager().SetPropertyValue("time", mapPoint, obj.production.time[0]);
            }

            //Set Properties values
            map.GetPropertyManager().SetPropertyValue("technique", mapPoint, obj.production.technique);
            map.GetPropertyManager().SetPropertyValue("material", mapPoint, obj.production.material);
            if(obj.production.location.Count<1)
                continue;
            map.GetPropertyManager().SetPropertyValue("place", mapPoint, obj.production.location[0].country);
           

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
        if(dim == 2)
            GetComponent<OnlineMapsAdjustToScreen>().ResizeMap();
    }

    private Texture2D getDefaultTexture()
    {
        Texture2D tex = new Texture2D(64, 64, TextureFormat.ARGB32, false);

        Color fillColor = Color.clear;
        Color[] fillPixels = new Color[tex.width * tex.height];

        for (int i = 0; i < fillPixels.Length; i++)
        {
            fillPixels[i] = fillColor;
        }

        tex.SetPixels(fillPixels);
        /*
        for (int i = 0; i < 100; i++)
        {
            for (int j = 0; j < 15; j++)
            {
                tex.SetPixel(i, j, Color.white);
            }
        }*/

        tex.Apply();

        return tex;
    }
}








