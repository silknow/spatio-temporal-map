using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SilkMap : MonoBehaviour {

    public static SilkMap instance;

    public Camera galleryCamera=null;
    public Camera mapCamera=null;
    public Hashtable marker3DHash = new Hashtable();
    public MapMarker map = null;
    public bool changing = false;

    GameObject initialMap;
    GameObject newMap;

    public static SilkMap Instance
    {
        // Here we use the ?? operator, to return 'instance' if 'instance' does not equal null
        // otherwise we assign instance to a new component and return that
        get { return instance ?? (instance = new GameObject("SilkMap").AddComponent<SilkMap>()); }
    }

    // Instance method, this method can be accesed through the singleton instance
    public void setGalleryCamera(Camera camera)
    {
        this.galleryCamera = camera;
    }

    public void setMapCamera(Camera camera)
    {
        this.mapCamera = camera;
    }

    public Camera getMapCamera()
    {
        return this.mapCamera;
    }

    public Camera getGalleryCamera()
    {
        return this.galleryCamera;
    }

    public void init()
    {
       Debug.Log("SE LLAMA A SILKMAP..............");
       this.mapCamera = Camera.allCameras[0];
       this.galleryCamera = Camera.allCameras[1];
       this.galleryCamera.enabled = false;
       this.mapCamera.enabled = true;

        //OnlineMapsTile.OnAllTilesLoaded += createPlane;
        
        //GameObject.Find("SilkMap").transform.SetPositionAndRotation(new Vector3(0.0f, 50.0f, 0.0f), GameObject.Find("SilkMap").transform.rotation);
    }

    public void changeCam()
    {
        if (this.mapCamera.enabled)
        {
            this.galleryCamera.enabled = true;
            this.mapCamera.enabled = false;
        }
        else
        {
            this.mapCamera.enabled = true;
            this.galleryCamera.enabled = false;
        }

    }

    public void LoadState(string key)
    {
        GameObject sphereModel = GameObject.Find("yarngroup");
        GameObject cylinderModel = GameObject.Find("Bobina2");

        Debug.Log("Recuperando ...."+key+"General");

        if (changing)
        {
            Debug.Log("No hay recuperación");
            return;

        }

        changing = true;

        Debug.Log("Comienza recuperación");

        List<GameObject> markers = (List < GameObject > ) marker3DHash[key];

        for (int i=0;i<markers.Count;i++)
        {
            GameObject marker = markers[i];
            marker.SetActive(true);
            marker.transform.SetParent(instance.transform);

        }

        //instance.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

        changing = false;


    }


    public void SaveState(string key)
    {

        Debug.Log("Comienza salvado... ");
        if (changing || marker3DHash.ContainsKey(key))
        {
            Debug.Log("No hay salvado");
            return;
        }

        changing = true;
         
        Debug.Log("Commienza salvado");

        OnlineMaps map = OnlineMaps.instance;


        OnlineMapsXML prefs = new OnlineMapsXML(key+"Map");


        // Save position and zoom
        OnlineMapsXML generalSettings = prefs.Create(key+"General");
        generalSettings.Create("Coordinates", map.position);
        generalSettings.Create("Zoom", map.zoom);

        List<GameObject> markers = new List<GameObject>();

        List <OnlineMapsMarker3D> mapMarkerList = OnlineMapsMarker3DManager.instance.items;

        for (int i = 0; i < mapMarkerList.Count;i++)
        {
            if (mapMarkerList[i].enabled)
            {
                GameObject cloned = Instantiate(mapMarkerList[i].prefab);
                cloned.transform.SetPositionAndRotation(mapMarkerList[i].instance.transform.position, mapMarkerList[i].instance.transform.rotation);
                cloned.transform.localScale = mapMarkerList[i].instance.transform.localScale;
                cloned.transform.localScale.Scale(new Vector3(1.5f, 1.5f, 1.5f));
                Debug.Log("La position es " + mapMarkerList[i].instance.transform.position.x + "," + mapMarkerList[i].instance.transform.position.y + "," + mapMarkerList[i].instance.transform.position.z);
                Debug.Log("La escala es " + mapMarkerList[i].instance.transform.localScale.x + "," + mapMarkerList[i].instance.transform.localScale.y + "," + mapMarkerList[i].instance.transform.localScale.z);
                cloned.SetActive(false);
                markers.Add(cloned);
            }
        }

        if (!marker3DHash.ContainsKey(key))
            marker3DHash.Add(key, markers);

        changing = false;


    }

    public void createPlane()
    {

               
        GameObject firstChild=null;
        OnlineMapsTile firstTile=null;
        OnlineMapsTile lastTile=null;

        Debug.Log("Hay tiles ..."+OnlineMaps.instance.tileManager.tiles.Count);
        int fila = 0;
        int i = 0;
        int filaReal = 0;
        int columnReal = 0;

        while (fila < 6 && i < OnlineMaps.instance.tileManager.tiles.Count)
        {
            for (int j = 0; j < 6; j++)
            {
                //Debug.Log("Fila = " + fila + "------- j = " + j);
                if (fila > 0 && j > 0 && j < 5)
                {
                    OnlineMapsTile tile = OnlineMaps.instance.tileManager.tiles[i];
                    GameObject cube = Instantiate(GameObject.Find("Cube2"));
                    cube.transform.SetPositionAndRotation(cube.transform.position + new Vector3(-10.0f * filaReal, 30.0f, 10.0f * columnReal), cube.transform.rotation);
                    columnReal++;

                    if (fila == 1 && j == 1)
                    {
                        firstChild = cube;
                        firstTile = tile;
                    }

                    if (fila == 5 && j == 4)
                    {
                        lastTile = tile;
                    }

                    if (tile != null)
                    {
                        Texture2D tex = tile.texture;
                        tex.Apply();
                        MeshRenderer renderer = cube.GetComponent<MeshRenderer>();
                        renderer.material.mainTexture = tex;
                    }
                    else
                        Debug.Log("TILE IS NULL");

                    cube.transform.parent = GameObject.Find("SilkMap").transform;
                }
                i++;
                
            }

            if (fila > 0)
                filaReal++;
            fila++;

            columnReal = 0;



        }


        Debug.Log("FIRST TILE ES " + firstTile.GetRect());
        Debug.Log("LAST TILE ES " + lastTile.GetRect());

        float xMin = firstTile.GetRect().xMin;
        float yMin = firstTile.GetRect().yMin;
        float xMax = lastTile.GetRect().xMin + lastTile.GetRect().width;
        float yMax = lastTile.GetRect().yMin + lastTile.GetRect().height;


        Rect rect = new Rect(xMin, yMin, xMax - xMin, yMax - yMin);

        GameObject.Find("SilkMap").transform.localScale = new Vector3(40.0f, 1.0f, 40.0f);

        int level = map.getLevel();
        List<MapPointMarker> clusterList = map.getClusterMarkersAtLevel(level);

        for (int m=0;m<clusterList.Count;m++) { 

            GameObject marker1 = Instantiate(clusterList[m].getMarker3D().prefab);
            
            float positionX = (-50.0f * (clusterList[m].getMarker3D().position.x + 90.0f)) / 225.0f;
            float positionY = -20.0f;
            float positionZ = (40.0f * (100.83f - (clusterList[m].getMarker3D().position.y+0.0f))) / 120.15f;

            Vector3 position = new Vector3(positionX, positionY, positionZ);

            marker1.transform.parent = GameObject.Find("SilkMap").transform;
            marker1.transform.localPosition = new Vector3(positionX, positionY, positionZ);

            float scaleCorrector = clusterList[m].getMarker3D().scale;
            if (!marker1.name.Contains("Bobina"))
            {
                scaleCorrector = scaleCorrector / 20.0f;
                marker1.transform.localScale = new Vector3(scaleCorrector, scaleCorrector, scaleCorrector);

            }
            else
            {
                scaleCorrector = marker1.transform.localScale.x;
                marker1.transform.localScale = new Vector3(scaleCorrector*2.0f, scaleCorrector*50.0f, scaleCorrector*2.0f);
                marker1.transform.localPosition = marker1.transform.localPosition + new Vector3(0.0f, 2.0f,0.0f);
            }

            

        }






    }

    public void reactivate()
    {
        newMap.SetActive(false);
        initialMap.SetActive(true);
    }

    public GameObject getPlane(OnlineMapsTile tile)
    {

        GameObject plane = new GameObject("Plane");
        MeshFilter meshFilter = (MeshFilter)plane.AddComponent(typeof(MeshFilter));


        Rect rect = tile.GetRect();
        Debug.Log("El rectangulo es " + rect);

        Mesh mesh = new Mesh();
        Vector3[] verts = new Vector3[4]
        {
            new Vector3(rect.min.x,0,rect.max.y),
            new Vector3(rect.min.x+rect.width,0,rect.max.y),
            new Vector3(rect.min.x+rect.width,0,rect.min.y),
            new Vector3(rect.min.x,0,rect.min.y)
        };

        mesh.vertices = verts;
        mesh.uv = new Vector2[]
        {
            new Vector2(0,0),
            new Vector2(0,1),
            new Vector2(1,1),
            new Vector2(1,0)
        };

        mesh.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;

        MeshRenderer renderer = plane.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        renderer.material.shader = Shader.Find("Particles/Additive");


        Texture2D tex = tile.texture;
        tex.Apply();

        renderer.material.mainTexture = tex;
        //renderer.material.color = Color.green;

        return plane;
    }
}