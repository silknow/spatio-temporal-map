using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    protected Vector2 mapTileResolution = new Vector2();

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
        }
        //Debug.Log("object map vale " + objectMap);
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

    public void updateCorners(List<OnlineMapsTile> listTiles)
    {
        //OnlineMapsTile tileTopLeft;
        //OnlineMapsTile tileBottomRight;

        if (listTiles.Count > 0)
        {
            tileTopLeft = listTiles[0];
            tileBottomRight = listTiles[0];

            foreach (OnlineMapsTile tile in listTiles)
            {
                if (tile.x <= tileTopLeft.x && tile.y >= tileTopLeft.y)
                    tileTopLeft = tile;

                if (tile.x >= tileBottomRight.x && tile.y <= tileBottomRight.y)
                    tileBottomRight = tile;
            }

            Debug.Log("tileTopLeft " + tileTopLeft.x + "," + tileTopLeft.y);
            Debug.Log("tileBottomRight " + tileBottomRight.x + "," + tileBottomRight.y);
        }
    }

    public void cloneCurrent()
    {

        int i = 0;
        int numTiles = OnlineMaps.instance.tileManager.tiles.Count;

        Debug.Log("num tiles = " + numTiles);

        //this.objectMap = Instantiate(clonedMap, new Vector3(0, 0, 0), Quaternion.identity);

        Vector2 origin = new Vector2(0, Screen.height);
        Vector2 end = new Vector2(Screen.width, 0);

        int maxRows = 0;
        Dictionary<int, List<OnlineMapsTile>> tilesCandidates = new Dictionary<int, List<OnlineMapsTile>>();

        while (i < numTiles)
        {
            OnlineMapsTile tile = OnlineMaps.instance.tileManager.tiles[i];

            //if (tile.zoom == OnlineMaps.instance.zoom && inScreen(tile))
            if (inScreen(tile))
            {
                if (!tilesCandidates.ContainsKey(tile.x))
                {
                    List<OnlineMapsTile> valuesList = new List<OnlineMapsTile>();
                    tilesCandidates.Add(tile.x, valuesList);
                }

                tilesCandidates[tile.x].Add(tile);

                if (tilesCandidates[tile.x].Count > maxRows)
                    maxRows = tilesCandidates[tile.x].Count;
            }

            i++;
        }

        mapTileResolution.x = 0;
        mapTileResolution.y = maxRows;

        Debug.Log("resolution = " + maxRows);


        List<OnlineMapsTile> listTiles = new List<OnlineMapsTile>();

        foreach (int key in tilesCandidates.Keys)
        {
            List<OnlineMapsTile> candidateList = tilesCandidates[key];
            if (candidateList.Count == maxRows)
            {
                listTiles.AddRange(candidateList);
                mapTileResolution.x++;
            }
        }

        Debug.Log("num tiles = " + listTiles.Count);


        //this.objectMap = Instantiate(clonedMap) as GameObject;

        updateCorners(listTiles);

        i = 0;

        
        //objectMap.transform.SetPositionAndRotation(
          //  new Vector3(desplazamiento, 0.0f, 0.0f), 
            //new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));

        while (i < listTiles.Count)
        {
            OnlineMapsTile tile = listTiles[i];

            GameObject cube = Instantiate(tileBase, new Vector3(0, 0, 0), Quaternion.identity);
            cube.transform.SetPositionAndRotation(cube.transform.position +
              new Vector3(-10.0f * (tile.x - tileTopLeft.x) , 
                            -1.0f, 
                            10.0f * (tile.y - tileBottomRight.y)+5.0f), cube.transform.rotation);
            if (tile != null)
            {
                Texture2D tex = tile.texture;
                tex.Apply();
                MeshRenderer renderer = cube.GetComponent<MeshRenderer>();
                renderer.material.mainTexture = tex;
            }
            else
                Debug.Log("TILE IS NULL");
            
            cube.transform.parent = objectMap.transform;
            i++;
        }

        //objectMap.transform.Translate(new Vector3(desplazamiento, 0.0f, 0.0f));

        

    }

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
        return tileBottomRight.x - tileTopLeft.x + 1;
    }

    public int getTileHeight()
    {
        return tileTopLeft.y - tileBottomRight.y + 1;
    }

    public bool inScreen(OnlineMapsTile tile)
    {
        bool inside = true;

        Vector2 topLeft = OnlineMapsControlBase3D.instance.GetScreenPosition(tile.topLeft);
        Vector2 bottomRight = OnlineMapsControlBase3D.instance.GetScreenPosition(tile.bottomRight);

        if (bottomRight.x < 0)
            return false;

        if (topLeft.y < 0)
            return false;

        if (topLeft.x > Screen.width)
            return false;

        if (bottomRight.y > Screen.height)
            return false;


        return inside;
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
}
