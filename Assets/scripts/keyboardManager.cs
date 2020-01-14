using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class keyboardManager : MonoBehaviour {

    private static bool vis3d = true;
    private static bool first = true;

    GameObject initialMap;
    GameObject newMap;


    private static int firstKeyMin = 0;
    private static int firstKeyMax = 0;

    public  bool galleryMode = false;

    public float speed = 3.0f;

    // Use this for initialization
    void Start () {
        print("Hello");

        if (first)
        {
            first = false;
            initialMap = GameObject.Find("Map");

        }
        
	}
	
	// Update is called once per frame


    void updateGallery()
    {
        float translation = Input.GetAxis("Vertical") * speed;
        float straffe = Input.GetAxis("Horizontal") * speed;
        translation *= Time.deltaTime;
        straffe *= Time.deltaTime;
        GameObject capsule = GameObject.Find("Capsule");
        capsule.transform.Translate(0, 0, translation);

        capsule.transform.Rotate(0, straffe * 1.5f, 0);
    }

    void Update ()
    {
        float incPos = 0.25f;

        incPos = incPos - 0.025f * OnlineMaps.instance.zoom;

        if (incPos <= 0.0f)
            incPos = 0.001f;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            //print("up arrow key is held down");
            //OnlineMapsCameraOrbit.instance.rotation.x = 0.0f;
            //print("up arrow key is held down");
            if (SilkMap.Instance.getGalleryCamera().enabled)
                updateGallery();
            else
                OnlineMaps.instance.position = OnlineMaps.instance.position + new Vector2(0.0f, incPos);
        }

        if (Input.GetKey(KeyCode.KeypadDivide))
            OnlineMapsCameraOrbit.instance.rotation.x = 0.0f;


        if (Input.GetKey(KeyCode.KeypadMultiply))
            OnlineMapsCameraOrbit.instance.rotation.x = 35.0f;


        if (Input.GetKey(KeyCode.Escape)) {

            if (SilkMap.Instance.getGalleryCamera().enabled)
                SilkMap.Instance.changeCam();
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            print("up arrow key is held down");
            //OnlineMapsCameraOrbit.instance.rotation.x = 35.0f;
            if (SilkMap.Instance.getGalleryCamera().enabled)
                updateGallery();
            else
                OnlineMaps.instance.position = OnlineMaps.instance.position + new Vector2(0.0f, -incPos);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            print("up arrow key is held down");
            if (SilkMap.Instance.getGalleryCamera().enabled)
                updateGallery();
            else
                OnlineMaps.instance.position = OnlineMaps.instance.position + new Vector2(-incPos, 0.0f);
            //OnlineMaps.instance.SetPositionAndZoom(-0.389f, 39.416f,10.0f);
            //OnlineMapsMarkerManager.CreateItem(-0.389f, 39.416f, "Valencia");
            //OnlineMapsMarker3DManager.CreateItemFromExistGameObject(-0.389f, 39.416f, GameObject.Find("Cube"));
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            print("up arrow key is held down");
            if (SilkMap.Instance.getGalleryCamera().enabled)
                updateGallery();
            else
                OnlineMaps.instance.position = OnlineMaps.instance.position + new Vector2(incPos, 0.0f);
            //OnlineMaps.instance.SetPositionAndZoom(29.32f, 50.701f, 3.0f);
            //OnlineMapsMarker3DManager.instance.CreateFromExistGameObject(22f, 39.2f, GameObject.Find("Cube"));
        }

        if (Input.GetKey(KeyCode.KeypadPlus))
        {

            if (firstKeyMax==0)
            {
                OnlineMaps.instance.zoom = OnlineMaps.instance.zoom + 1;
                firstKeyMax = 8;
            }
            else
                firstKeyMax--;
            //OnlineMaps.instance.SetPositionAndZoom(29.32f, 50.701f, 3.0f);
            //OnlineMapsMarker3DManager.instance.CreateFromExistGameObject(22f, 39.2f, GameObject.Find("Cube"));
        }

        if (Input.GetKey(KeyCode.KeypadMinus))
        {
            if (firstKeyMin==0)
            {
                OnlineMaps.instance.zoom = OnlineMaps.instance.zoom - 1;
                firstKeyMin = 8;
            }
            else
                firstKeyMin--;
            //OnlineMaps.instance.SetPositionAndZoom(29.32f, 50.701f, 3.0f);
            //OnlineMapsMarker3DManager.instance.CreateFromExistGameObject(22f, 39.2f, GameObject.Find("Cube"));
        }

        if (Input.GetKey(KeyCode.F1))
            OnlineMaps.instance.mapType = "osm"; //.mapnik"; 

        if (Input.GetKey(KeyCode.F2))
           OnlineMaps.instance.mapType = "google.satellite";

        if (Input.GetKey(KeyCode.F3))
            OnlineMaps.instance.mapType = "arcgis.worldtopomap";

        if (Input.GetKey(KeyCode.F4))
            OnlineMaps.instance.mapType = "mapquest.satellite";

        if (Input.GetKey(KeyCode.F5))
            OnlineMaps.instance.mapType = "openmapsurfer";

        if (Input.GetKey(KeyCode.F6))
            OnlineMaps.instance.mapType = "opentopomap"; //.opentopomap";

        if (Input.GetKey(KeyCode.F7))
            OnlineMaps.instance.mapType = "stameny";

        if (Input.GetKey(KeyCode.F8))
            OnlineMaps.instance.mapType = "openmapsurfer"; 

        if (Input.GetKey(KeyCode.F9))
            OnlineMaps.instance.mapType = "openmapsurfer.adminbounds"; 

        if (Input.GetKey(KeyCode.F10))
            OnlineMaps.instance.mapType = "other.mtbmap";

        if (Input.GetKey(KeyCode.F11))
        {
            //SilkMap.Instance.changeCam();
            //PlayerPrefs.DeleteAll();
            //SilkMap.Instance.SaveState("sigloXX");
            //OnlineMapsMarker3DManager.instance.RemoveAll(true);


            SilkMap.Instance.createPlane();




            //OnlineMaps.instance.SetPosition(-10.67986, 25.55115);
        }
            
            // F11
            // OnlineMaps.instance.mapType = "virtualearth.aerial";

        if (Input.GetKey(KeyCode.F12))
        {
            //SilkMap.Instance.LoadState("sigloXX");


            SilkMap.Instance.reactivate();
            
            
            
            
            
            //SilkMap.Instance.map.update();
            //SilkMap.Instance.map.showClusters();
            /*
            initialMap.SetActive(false);
            newMap = OnlineMaps.Instantiate(initialMap);
            newMap.SetActive(true);
            initialMap.SetActive(true);*/


        }
        //OnlineMaps.instance.mapType = "yandex.normal";

    }



}
