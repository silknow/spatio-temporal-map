using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSnapshot 
{

    Map map;
    protected List<OnlineMapsTile> listTiles = new List<OnlineMapsTile>();
    protected Dictionary<int, List<OnlineMapsTile>> tilesCandidates = new Dictionary<int, List<OnlineMapsTile>>();
    int height=0;
    int width=0;
    float validPercentageTile = 0.35f;
    int numColumns = 0;
    int numRows = 0;
    OnlineMapsTile topLeftCorner;
    OnlineMapsTile bottomRightCorner;
    GameObject tileBase;
    GameObject snapShotMesh;

    public MapSnapshot(Map map)
    {        
        this.map = map;
    }

    public GameObject take(int height, int width)
    {
        listTiles.Clear();
        this.height = height;
        this.width = width;
        loadTilesAtLevel((int)map.getZoom());
        createGameObject();
        return snapShotMesh;
    }

    public GameObject getSnapShot()
    {
        return snapShotMesh;
    }

    protected void createGameObject()
    {
        if (snapShotMesh != null)
            Object.DestroyImmediate(snapShotMesh);

        foreach (OnlineMapsTile tile in listTiles)
        {
            //OnlineMapsTile tile = listTiles[i];

            snapShotMesh = Object.Instantiate(tileBase, new Vector3(0, 0, 0), Quaternion.identity);
            snapShotMesh.transform.SetPositionAndRotation(snapShotMesh.transform.position +
              new Vector3(-10.0f * (tile.x - topLeftCorner.x),
                            -1.0f,
                            10.0f * (tile.y - bottomRightCorner.y) + 5.0f), snapShotMesh.transform.rotation);
            if (tile != null)
            {
                Texture2D tex = tile.texture;
                tex.Apply();
                MeshRenderer renderer = snapShotMesh.GetComponent<MeshRenderer>();
                renderer.material.mainTexture = tex;
            }
            else
                Debug.Log("TILE IS NULL");           
        }
    }

    protected void loadTilesAtLevel(int level)
    {
        int i = 0;
        int numTiles = OnlineMaps.instance.tileManager.tiles.Count;

        numRows = 0;
 
        // Load the candidate tiles and know the number of rows of the snapshot
        while (i < numTiles)
        {
            OnlineMapsTile tile = OnlineMaps.instance.tileManager.tiles[i];

            if (tile.zoom == level && inScreen(tile))
            {
                if (!tilesCandidates.ContainsKey(tile.x))
                {
                    List<OnlineMapsTile> valuesList = new List<OnlineMapsTile>();
                    tilesCandidates.Add(tile.x, valuesList);
                }

                tilesCandidates[tile.x].Add(tile);

                if (tilesCandidates[tile.x].Count > numRows)
                    numRows = tilesCandidates[tile.x].Count;
            }

            i++;
        }

        numColumns = 0;

        // Put in listTiles the definitive tile list, and get the numColumns property
        foreach (int key in tilesCandidates.Keys)
        {
            List<OnlineMapsTile> candidateList = tilesCandidates[key];
            if (candidateList.Count == numRows)
            {
                listTiles.AddRange(candidateList);
                numColumns++;
            }
        }

        tilesCandidates.Clear();

        updateCorners();
    }


    protected bool inScreen(OnlineMapsTile tile)
    {
        bool inside = true;

        // Reject all the tiles out of the screen (0,height),(width,0)
        if (tile.bottomRight.x < 0 || tile.bottomRight.y > this.height)
            return false;

        if (tile.topLeft.y < 0 || tile.topLeft.x > this.width)
            return false;

        // Reject all the tiles inside the screen, which its area inside is minor than valid percentage

        // Calculate the initial area of the tile
        double tileWidth = tile.bottomRight.x - tile.topLeft.x;
        double tileHeight = tile.topLeft.y - tile.bottomRight.y;
        double iniArea = tileWidth * tileHeight;

        // Get the width and height inside the  screen
        if (tile.topLeft.x < 0)
            tileWidth = tile.bottomRight.x;

        if (tile.bottomRight.x > this.width)
            tileWidth = this.width - tile.topLeft.x;

        if (tile.topLeft.y > this.height)
            tileHeight = this.height - tile.bottomRight.y;

        if (tile.bottomRight.y < 0)
            tileHeight = tile.topLeft.y;

        // Check if area is minor than valid percentage
        if (tileHeight * tileWidth <= this.validPercentageTile * iniArea)
            return false;


        return inside;
    }

    public void updateCorners()
    {

        if (listTiles.Count > 0)
        {
            topLeftCorner = listTiles[0];
            bottomRightCorner = listTiles[0];

            foreach (OnlineMapsTile tile in listTiles)
            {
                if (tile.x <= topLeftCorner.x && tile.y >= topLeftCorner.y)
                    topLeftCorner = tile;

                if (tile.x >= bottomRightCorner.x && tile.y <= bottomRightCorner.y)
                    bottomRightCorner = tile;
            }


        }
    }


    public void setValidPercentageTile(float percentage) => this.validPercentageTile = percentage;

    public float getValidPercentageTile() => this.validPercentageTile;
    
    public int getNumTiles() => listTiles.Count;

    public void setTileBase(GameObject tileBase) => this.tileBase = tileBase;
}
