using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SilkMap : MonoBehaviour {

    public static SilkMap instance;

    //public Camera galleryCamera=null;
    public Camera mapCamera=null;
    public Hashtable marker3DHash = new Hashtable();
    public MapMarker map = null;
    public bool changing = false;
    public bool timeSliceDisplay = false;


    GameObject initialMap;
    GameObject newMap;
    public GameObject clonedGroupMap;
    List<GameObject> mapList = new List<GameObject>();

    public static SilkMap Instance
    {
        // Here we use the ?? operator, to return 'instance' if 'instance' does not equal null
        // otherwise we assign instance to a new component and return that
        get { return instance ?? (instance = new GameObject("SilkMap").AddComponent<SilkMap>()); }
    }

   

    public void setMapCamera(Camera camera)
    {
        this.mapCamera = camera;
    }

    public Camera getMapCamera()
    {
        return this.mapCamera;
    }

    /*
    // Instance method, this method can be accesed through the singleton instance
    public void setGalleryCamera(Camera camera)
    {
        this.galleryCamera = camera;
    }

    public Camera getGalleryCamera()
    {
        return this.galleryCamera;
    }
    */

    public void init()
    {
       //Debug.Log("SE LLAMA A SILKMAP..............");
        this.mapCamera = Camera.allCameras[0];
       //this.galleryCamera = Camera.allCameras[1];
       //this.galleryCamera.enabled = false;
       this.mapCamera.enabled = true;
        //this.map.SetDimension(2);

        //OnlineMapsTile.OnAllTilesLoaded += createPlane;
        
        //GameObject.Find("SilkMap").transform.SetPositionAndRotation(new Vector3(0.0f, 50.0f, 0.0f), GameObject.Find("SilkMap").transform.rotation);
    }

    public void changeCam()
    {
        /*
        if (this.mapCamera.enabled)
        {
            this.galleryCamera.enabled = true;
            this.mapCamera.enabled = false;
        }
        else
        {
            this.mapCamera.enabled = true;
            this.galleryCamera.enabled = false;
        }*/

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

    public void clearData()
    {
        this.mapList.Clear();
    }


    public void refreshStack()
    {
        ClonedMap.Instance.updateVisualization();
    }
    public GameObject createSnapShot(int from, int to)
    {


       // if (clonedGroupMap == null)
        //clonedGroupMap = ClonedMap.instance.getObjectMap();

        /*
        if (to == 1700)
            timeSliceDisplay = true;
        else
        {
            if (timeSliceDisplay)
            {
                removeSnapShots();
                timeSliceDisplay = false;                
            }
        }*/

        GameObject snapShot = ClonedMap.Instance.cloneCurrent(from, to);

        return snapShot;
    }

    public void removeSnapShots()
    {
        //if (clonedGroupMap == null)
        //clonedGroupMap = ClonedMap.instance.getObjectMap();

        foreach (Transform child in ClonedMap.instance.getObjectMap().transform)
        {
            GameObject.DestroyImmediate(child.gameObject);
        }

        Resources.UnloadUnusedAssets();

        ClonedMap.instance.unStackPoints();
    }


    public GameObject createPlane(int from, int to)
    {
        return createSnapShot(from, to);
    }


    public void resetMap()
    {
        //map.reset();
        // Resetea el valor de las propiedades
        //map.GetPropertyManager().resetPropertyValues();

        // Reset all filters
        //map.resetFilters();

        // Remove all relations
        //map.removeAllRelations();
    }

    public void reactivate()
    {
        //newMap.SetActive(false);
        //initialMap.SetActive(true);
        if (clonedGroupMap != null)
        {
           //clonedGroupMap.SetActive(false);
           foreach (Transform children in clonedGroupMap.transform)
            {
                Destroy(children.gameObject);
            }
            foreach (Transform childrenG in ClonedMap.Instance.getObjectMap().transform)
            {
                Destroy(childrenG.gameObject);
            }

            clearData();  

        }

        //initialMap.SetActive(true);
        ClonedMap.instance.unStackPoints();
        timeSliceDisplay = false;
        ClonedMap.Instance.getObjectMap().SetActive(true);


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