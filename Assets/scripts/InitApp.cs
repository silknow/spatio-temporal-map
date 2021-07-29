using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Clustering;
using Honeti;
using UnityEngine.Events;
using SilknowMap;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.Timeline;
using Debug = UnityEngine.Debug;

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
    [SerializeField] private Sprite customGroupPointMarker;
    public Canvas customMarkerCanvas;

    // Event to be triggered when data is loaded to the map
    [Header("Event to be triggered when data is loaded to the map")]
    public UnityEvent loadedDataEvent;

    public Dictionary<string, Texture> iconsTexture = new Dictionary<string, Texture>();

    public List<Color> relationsColors;

    private Dictionary<string, ScriptableCategory> categoriesDictionary;
    private List<ScriptableCategory> categoriesGroups;
    private int numObject=0;
   

    private void OnGUI()
    {
        //OnlineMaps.instance.zoom = 6;
        // Reajusta el mapa de acorde al zoom del sistema.
        //map.setViewerZoom(OnlineMaps.instance.zoom);
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
        string[] category = {"silknow.org/#pthing", "silknow.org/#man"};
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

                mapPoints.Add(new MapPointMarker(
                    OnlineMapsMarker3DManager.CreateItem(new Vector2(longitudP, latitudP), prefabObject)));
                // Altitud del mar = 30.0f
                ((MapPointMarker) (mapPoints[mapPoints.Count - 1])).getMarker3D().altitude = 30.0f; // 0.0f; // 30.0f;
                // Scale =3.0f, luego se cambia
                ((MapPointMarker) (mapPoints[mapPoints.Count - 1])).getMarker3D().scale = 3.0f; // 1.0f; // 3.0f;
                // Se le asigna un collider
                BoxCollider box = ((MapPointMarker) (mapPoints[mapPoints.Count - 1])).getMarker3D().instance
                    .GetComponent<BoxCollider>();
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
                        mapPoints[mapPoints.Count - 1]
                            .addRelationWith(mapPoints[connectedWith], "http://silknow.org/propName");
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
        map = new MapMarker();
        map.clusterPrefab = prefabCluster;
       
        //map.fixPositionInterval(OnlineMaps.instance, 28.24f, -16.79f, 71.19f, 48.48f);
        map.setViewerPosition(10.23985f, 21.83287f);
        OnlineMaps.instance.zoom = 3;
        map.setViewerZoom(3);
        
        map.setGridingZoomData(3, 12, 8);// CAmbio 2,10,6 por 2,14,4
        map.setGridingQuadsHorizonal(8); // cambio 16 por 8
        map.fixZoomInterval(OnlineMaps.instance, 3, 12);
      
        SilkMap.Instance.map = map;
        //ClonedMap.instance.setMap(map);
        if (clonedMapGroup != null)
            SilkMap.instance.clonedGroupMap = clonedMapGroup;

        //map.showClusters();

        map.SetDimension(2);

        UpdatePropertyManager(SilkMap.instance.map);

        OnlineMapsMarkerManager.instance.defaultTexture = getDefaultTexture();

        /*
        GameObject lightGameObject = new GameObject("The Light");
        Light lightComp = lightGameObject.AddComponent<Light>();
        lightComp.type = LightType.Directional;
        lightComp.color = Color.white;
        lightGameObject.transform.position = new Vector3(0, 5, 0);
        lightGameObject.transform.Rotate(new Vector3(45, 0, 0), Space.Self);
        */
        Resources.UnloadUnusedAssets();
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
        OnlineMapsMarker3D om = (OnlineMapsMarker3D) marker;

        Debug.Log(marker.label + " : " + marker.position.x + "," + marker.position.y);
    }

    private void Awake()
    {
        UnityEngine.Analytics.Analytics.enabled = false;
        UnityEngine.Analytics.Analytics.deviceStatsEnabled = false;
        UnityEngine.Analytics.Analytics.initializeOnStartup = false;
        UnityEngine.Analytics.Analytics.limitUserTracking = false;
        UnityEngine.Analytics.PerformanceReporting.enabled = false;
    }

    // Función principal inicia las instancias de OnlineMas, y carga los datos
    private void Start()
    {
        //Debug.Log("start method 1..... IN " + Time.time * 1000);
        //Update map labels;
        OnlineMaps.instance.language = I18N.instance.gameLang.ToString().ToLower(CultureInfo.InvariantCulture);


        // Get instance of OnlineMapsControlBase3D (Texture or Tileset)
        OnlineMapsControlBase3D control = OnlineMapsControlBase3D.instance;

        if (control == null)
        {
            Debug.LogError("You must use the 3D control (Texture or Tileset).");
            return;
        }


        SilkMap.Instance.init();
        Camera.main.orthographicSize = 287;
        OnlineMapsCameraOrbit.instance.lockPan = true;
        OnlineMapsCameraOrbit.instance.lockTilt = true;

        
        initMarkers();

        map.customMarkerCanvas = customMarkerCanvas;
        map.customMarkerGameObject = customMarkerGameObject;
        map.customGroupPointMarker = customGroupPointMarker;
        categoriesDictionary = new Dictionary<string, ScriptableCategory>();
        categoriesGroups = Resources.LoadAll<ScriptableCategory>("ScriptableCategories").ToList();

        FillCategoriesDictionary();
        Resources.UnloadUnusedAssets();
        GC.Collect();
        //Debug.Log("start method 4..... IN " + Time.time * 1000);
    }

    public void FillCategoriesDictionary()
    {
        //costumes
        categoriesDictionary.Add("costume", categoriesGroups[2]);
        categoriesDictionary.Add("civil attire", categoriesGroups[2]);
        categoriesDictionary.Add("women's attire", categoriesGroups[2]);
        categoriesDictionary.Add("men's attire", categoriesGroups[2]);
        categoriesDictionary.Add("children's attire", categoriesGroups[2]);
        categoriesDictionary.Add("festive attire", categoriesGroups[2]);
        categoriesDictionary.Add("religious attire", categoriesGroups[2]);
        categoriesDictionary.Add("regional attire", categoriesGroups[2]);
        categoriesDictionary.Add("costume accessories", categoriesGroups[2]);
        //drawing
        categoriesDictionary.Add("drawings", categoriesGroups[3]);
        categoriesDictionary.Add("sketch", categoriesGroups[3]);
        categoriesDictionary.Add("other typesofobjects", categoriesGroups[3]);
        categoriesDictionary.Add("point paper plan", categoriesGroups[3]);
        //fabrics
        categoriesDictionary.Add("fabrics", categoriesGroups[4]);
        categoriesDictionary.Add("fabric passementerie", categoriesGroups[4]);
        categoriesDictionary.Add("ornamental fabrics", categoriesGroups[4]);
        //household objects
        categoriesDictionary.Add("household objects", categoriesGroups[5]);
        //furniture
        categoriesDictionary.Add("furniture", categoriesGroups[6]);
        //religious accessories
        categoriesDictionary.Add("religious accessories", categoriesGroups[7]);
    }

    public void UpdatePropertyManager(Map map)
    {
        //map.GetPropertyManager().AddProperty("description.language", false, false, 0, 0, false, false,false);
        map.GetPropertyManager().AddProperty("description", true, false, 0, 1, false, false);
        map.GetPropertyManager().AddProperty("technique", true, true, 1, 2, false, false, true, relationsColors[0]);
        map.GetPropertyManager().AddProperty("material", true, true, 2, 3, false, false, true, relationsColors[1]);
        map.GetPropertyManager().AddProperty("time", true, true, 3, 4, false, false, true, relationsColors[2]);
        map.GetPropertyManager().AddProperty("place", true, false, 0, 5, false, true);
        map.GetPropertyManager().AddProperty("img", true, false, 0, 6, true, false);
        map.GetPropertyManager().AddProperty("category", true, true, 4, 7, false, false, true, relationsColors[3]);
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
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(iconsPath + textureFileName);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Texture texture = ((DownloadHandlerTexture) www.downloadHandler).texture;
                iconsTexture.Add(textureFileName, texture);
            }
        }
    }

    private void AppendObjectsToMap(ManMadeObject[] objectList)
    {
        var objCount = objectList.Length;
        var listOfPoints = new List<MapPoint>();
        for (var index = 0; index < objCount; index++)
        {
            var obj = objectList[index];
            if (obj.production?.location == null)
                continue;
/*
            if (numObject == 300)
                Debug.Log("EL 300 es " + obj.id);

            if (numObject == 3000)
                Debug.Log("EL 3000 es " + obj.id);

            if (numObject == 15000)
                Debug.Log("EL 15000 es " + obj.id);

            numObject++;
*/

            /* PRUEBA RENDIMIENTO */
            var listOfCenturies = new List<TimeElement>();
            if (obj.production?.time.Length > 0)
            {
             
                foreach (var timeElement in obj.production.time)
                {
                    if (APIManager.instance.timeValues.TryGetValue(timeElement, out var time))
                    {
                        listOfCenturies.Add(time);
                    }

                       
                }

                listOfCenturies = listOfCenturies.OrderBy(t => t.century).ToList();
     
            }
            var objCategory = categoriesGroups[0];

            var listOfCategories = new List<string>();
            if (obj.production?.category != null && obj.production.category?.Length > 0 &&
                obj.production.category[0] != null && 
                obj.production.category[0].label != null)
            {
                listOfCategories = obj.production.category.Select(s=>s.label).ToList();
                switch (listOfCategories.Count)
                {
                    case 0:
                        objCategory = categoriesGroups[0];
                        break;
                    case 1:
                    {
                        objCategory = categoriesDictionary.TryGetValue(listOfCategories[0], out var cat) ? cat : categoriesGroups[0];
                        break;
                    }
                    default:
                    {
                        var selectedCategories = new List<ScriptableCategory>();
                        foreach (var categoryName in listOfCategories)
                        {
                            if (!categoriesDictionary.TryGetValue(categoryName, out var cat)) continue;
                            if (!selectedCategories.Contains(cat))
                                selectedCategories.Add(cat);
                        }

                        objCategory = selectedCategories.Count > 1 ? categoriesGroups[1] : selectedCategories[0];

                        selectedCategories.Clear();
                        break;
                    }
                }
            }
            /* PRUEBA RENDIMIENTO */
            foreach (var currentLocation in obj.production?.location)
            {
                var latitude = currentLocation.lat;
                var longitude = currentLocation.@long;

                if (float.IsNaN(latitude) || float.IsNaN(longitude))
                    continue;

                // Cada MapPoint tiene una serie de propiedades, la posición, escala y altitud vienen dentro de un objeto
                // MapPointMarker de OnlineMaps 
                // Se añade la posicion (lat,long) y se asocia a un objeto 3d Cube, luego puede cambiarse
                var mapPoint = new MapPointMarker(longitude, latitude, prefabObject, false);
                //mapPoint.addPositionValue("Production", new Vector2(longitude, latitude));
                if (mapPoint.getMarker3D() != null)
                {
                    mapPoint.getMarker3D().altitude = 30.0f;
                    mapPoint.getMarker3D().scale = 3.0f;
                }

                mapPoint.setMap(map);

                /* PRUEBA RENDIMIENTO */
                // Se introducen el resto de propiedades (URI, categpry (clase), from y to (intervalo de tiempo)
                mapPoint.setURI(obj.id);
                mapPoint.setLabel(obj.label != null && obj.label.Length > 0 ? obj.label[0] : obj.identifier);
 
                if (obj.production?.time?.Length > 0)
                {
                    map.GetPropertyManager().SetPropertyValue("time", mapPoint, obj.production.time.ToList());
                    if (listOfCenturies.Count > 0)
                    {
                        mapPoint.setFrom(listOfCenturies.First().@from);
                        mapPoint.setTo(listOfCenturies.Last().to);
                    }
                }
 
                //Set Properties values
                map.GetPropertyManager().SetPropertyValue("technique", mapPoint, obj.production.technique.ToList());
                map.GetPropertyManager().SetPropertyValue("material", mapPoint, obj.production.material.ToList());
 
                
                var listOfPlaces = obj.production.location.Select(s=>s.label).ToList();
                map.GetPropertyManager().SetPropertyValue("place", mapPoint, listOfPlaces);
                mapPoint.assignTexture(listOfPlaces.Count == 1 ? objCategory.defaultIcon: objCategory.multipleLocationIcon);
                mapPoint.setKnownLocation(listOfPlaces.Count ==1);
                
                map.GetPropertyManager().SetPropertyValue("category", mapPoint, listOfCategories);
 
               
                 mapPoint.setDimension(map.GetDimension());
                 mapPoint.setMultipleLocations(obj.production.location.Length > 1);
                  /* PRUEBA RENDIMIENTO  */
                listOfPoints.Add(mapPoint);
            }
        }
        map.addPoints(listOfPoints);
        
    }

    public IEnumerator LoadRestData(ManMadeObject[] objectList)
    {
        EvaluationConsole.instance.AddLine($"El Dataset tiene: {objectList.Length} objetos");
        var totalTime = Stopwatch.StartNew();
        var startTime = Stopwatch.StartNew();
        
        map.reset();
        map.addPositionName("Production");
        
        //AppendOBJECTS
        foreach (var smallerList in  objectList.Split(10))
        {
            AppendObjectsToMap(smallerList.ToArray());
            yield return null;
        }
        EvaluationConsole.instance.AddLine($"Se han generado: {map.getNumPoints()} marcadores");
        EvaluationConsole.instance.AddLine($"Tiempo Generación Marcadores: {startTime.ElapsedMilliseconds * 0.001f} s");
        yield return new WaitForSeconds(0.1f);
        ResetMapParameters();
        map.update();

        //print("loadedDataEvent triggered");
        loadedDataEvent.Invoke();
        map.setViewerZoom(OnlineMaps.instance.zoom);
        map.UpdateMarkers();

        objectList = null;
        GC.Collect();
        Resources.UnloadUnusedAssets();
    }
    /*public IEnumerator LoadRestData(ManMadeObject[] objectList)
    {
     
        Debug.Log("Empezando a cargar datos");
        var startTime = Stopwatch.StartNew();

        mapPoints.Clear();
        map.reset();
        map.addPositionName("Production");
        //map.addPositionName("Actual");


        // Para cambiar mapa con posicion name.........................
        // map.setMaintainPoints(true);
        // map.activePosition(positionName);
        // map.reset();
        // ResetMapParameters();
        // map.update();

        var objCount = objectList.Length;
        var coroutineYieldFactor = objCount / 5;
        for (var index = 0; index < objCount; index++)
        {
            if (index%coroutineYieldFactor == 0)
                yield return null;
            var obj = objectList[index];
            if (obj.production == null || obj.production.location == null)
                continue;

            for (int locationIndex = 0; locationIndex < obj.production.location.Length; locationIndex++)
            {
                var latitude = obj.production.location[locationIndex].lat;
                var longitude = obj.production.location[locationIndex].@long;

                if (float.IsNaN(latitude) || float.IsNaN(longitude))
                    continue;


                // Cada MapPoint tiene una serie de propiedades, la posición, escala y altitud vienen dentro de un objeto
                // MapPointMarker de OnlineMaps 
                // Se añade la posicion (lat,long) y se asocia a un objeto 3d Cube, luego puede cambiarse
                var mapPoint = new MapPointMarker(longitude, latitude, prefabObject, false);

                // If the object has a correct location, it is added to he sameLocationPoints list.
                //sameLocationPoints.Add(mapPoint);

                mapPoint.addPositionValue("Production", new Vector2(longitude, latitude));
                //mapPoint.addPositionValue("Actual", new Vector2(longitud + 1, latitud + 1));

                // Altitud del mar = 30.0f               
                mapPoint.getMarker3D().altitude = 30.0f;
                // Scale =3.0f, luego se cambia
                mapPoint.getMarker3D().scale = 3.0f;
                mapPoint.setMap(map);

                /* PRUEBA RENDIMIENTO #1#
                // Se introducen el resto de propiedades (URI, categpry (clase), from y to (intervalo de tiempo)
                mapPoint.setURI(obj.id);
                //mapPoint.setCategory(category[1]);
                mapPoint.setLabel(obj.label != null && obj.label.Length > 0 ? obj.label[0] : obj.identifier);

                if (obj.production.time.Length > 0)
                {
                    var listOfCenturies = new List<TimeElement>();
                    foreach (var timeElement in obj.production.time)
                    {
                        if (APIManager.instance.timeValues.TryGetValue(timeElement, out var time))
                        {
                            listOfCenturies.Add(time);
                        }

                        map.GetPropertyManager().SetPropertyValue("time", mapPoint, timeElement);
                    }

                    var orderedList = listOfCenturies.OrderBy(t => t.century).ToList();

                    if (orderedList.Count > 0)
                    {
                        mapPoint.setFrom(orderedList.First().@from);
                        mapPoint.setTo(orderedList.Last().to);
                    }
                }

                //Set Properties values
                map.GetPropertyManager().SetPropertyValue("technique", mapPoint, obj.production.technique.ToList());
                map.GetPropertyManager().SetPropertyValue("material", mapPoint, obj.production.material.ToList());

                ScriptableCategory objCategory = categoriesGroups[0];

                if (obj.production.category != null && obj.production.category.Length > 0 &&
                    obj.production.category[0] != null)
                {
                    var listOfCategories = new List<string>();
                    foreach (var categoryLabel in obj.production.category)
                    {
                        if (categoryLabel.label != null)
                            listOfCategories.Add(categoryLabel.label);
                    }

                    map.GetPropertyManager().SetPropertyValue("category", mapPoint, listOfCategories);
                    if (listOfCategories.Count == 0)
                        objCategory = categoriesGroups[0];
                    else if (listOfCategories.Count == 1)
                    {
                        if (categoriesDictionary.TryGetValue(listOfCategories[0], out var cat))
                        {
                            objCategory = cat;
                        }
                        else
                        {
                            objCategory = categoriesGroups[0];
                        }
                    }
                    else
                    {
                        List<ScriptableCategory> selectedCategories = new List<ScriptableCategory>();
                        foreach (var categoryName in listOfCategories)
                        {
                            if (categoriesDictionary.TryGetValue(categoryName, out var cat))
                            {
                                if (!selectedCategories.Contains(cat))
                                    selectedCategories.Add(cat);
                            }
                        }

                        if (selectedCategories.Count > 1)
                            objCategory = categoriesGroups[1];
                        else
                        {
                            objCategory = selectedCategories[0];
                        }

                        selectedCategories.Clear();
                    }
                }

                var listOfPlaces = new List<String>();
                foreach (var loc in obj.production.location)
                {
                    listOfPlaces.Add(loc.label);
                }

                map.GetPropertyManager().SetPropertyValue("place", mapPoint, listOfPlaces);

                if (listOfPlaces.Count == 1)
                {
                    mapPoint.setKnownLocation(true);
                    mapPoint.assignTexture(objCategory.defaultIcon);
                }
                else
                {
                    mapPoint.setKnownLocation(false);
                    mapPoint.assignTexture(objCategory.multipleLocationIcon);
                }

                /* PRUEBA RENDIMIENTO  #1#
                mapPoint.setDimension(map.GetDimension());
                mapPoint.setMultipleLocations(obj.production.location.Length>1);
                mapPoints.Add(mapPoint);
            }

            // If there are more that one object with the same locations, 
            // All the objects are multipleLocation marked
           /* if (sameLocationPoints.Count > 1)
            {
                //Debug.Log("DUPLICADOS " + sameLocationPoints.Count);
                foreach (MapPoint p in sameLocationPoints)
                {
                    p.setMultipleLocations(true);
                    //Debug.Log(p.getLabel() + "    " + p.getX() + "," + p.getY());
                }
            }

            sameLocationPoints.Clear();#1#
        }


        map.addPoints(mapPoints);
        mapPoints.Clear();
        EvaluationConsole.instance.AddLine($"CARGAR OBJETOS: {startTime.ElapsedMilliseconds * 0.001f} s");
      
        //Debug.Log("HAY " + mapPoints.Count + " PUNTOS");
        
        
        var resetTime = Stopwatch.StartNew();
        ResetMapParameters();
        EvaluationConsole.instance.AddLine($"Resetear parametros Mapa: {resetTime.ElapsedMilliseconds * 0.001f} s");
        var updateTime = Stopwatch.StartNew();
        map.update();
        EvaluationConsole.instance.AddLine($"Actualizar Mapa: {updateTime.ElapsedMilliseconds * 0.001f} s");
        //StartCoroutine(map.updateRelationData());

        //StartCoroutine(map.createGraphicRelationData());

        //print("loadedDataEvent triggered");
        loadedDataEvent.Invoke();
        map.setViewerZoom(OnlineMaps.instance.zoom);
        map.UpdateMarkers();

        objectList = null;
        GC.Collect();
        Resources.UnloadUnusedAssets();
    }*/
 

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

    public void ResetMapParameters()
    {
        /*
        map.setGridingZoomData(3, 10, 4);
        map.setGridingQuadsHorizonal(64);
        map.fixZoomInterval(OnlineMaps.instance, 3, 14);
        map.fixPositionInterval(OnlineMaps.instance, 28.24f, -16.79f, 71.19f, 48.48f);
 
        */
        SilkMap.Instance.map = map;

        //CenterMapOnData();


        //map.showClusters();
    }

    public void CenterMapOnData()
    {
        Vector2 center;
        int zoom;

        var listOfMarkers = mapPoints.Select(t => (t as MapPointMarker)?.getMarker2D() as OnlineMapsMarkerBase);

        //Debug.LogFormat("OnlineMapsMarkerManager count : {0}",OnlineMapsMarkerManager.instance.items.Count);
        // Get the center point and zoom the best for all markers.
        OnlineMapsUtils.GetCenterPointAndZoom(OnlineMapsMarkerManager.instance.ToArray(), out center, out zoom);

        //Debug.LogFormat("Zoom Level: {0}",zoom);
        //Debug.LogFormat("Center: {0} ; {1}",center.x,center.y);
        // Change the position and zoom of the map.


        map.fixZoomInterval(OnlineMaps.instance, 2, Mathf.Max(zoom, 14));
        map.setViewerZoom(zoom);
        //map.setViewerPosition(center.x, center.y);
    }

    public void ToggleDimension()
    {
        short dim = map.GetDimension() == (short)2 ? (short)3 : (short)2;
        map.SetDimension(dim);
        if (dim == 2)
        {
            GetComponent<OnlineMapsAdjustToScreen>();
        }
          
        map.showClusters();
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