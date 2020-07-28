using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClusterManager {

    List<MapPoint> points = new List<MapPoint>();
    List<MapLevel> levels = new List<MapLevel>();

    private Rect zone;
    private float zoomMin;
    private float zoomMax;
    private int zoomIntervals;
    private int numQuadsHoriz;
    private int numQuadsVert;

    public ClusterManager()
    {
        // By default zone is the whole world in longitud latitud
        zone.xMin = -180;
        zone.xMax = 180;
        zone.yMin = -90;
        zone.yMax = 90;
    }

    public void reset()
    {
        for (int i = 0; i < levels.Count; i++)
            levels[i].reset();

        //levels.RemoveRange(0, levels.Count);
        levels.Clear();

        levels = new List<MapLevel>();

        //for (int i = 0; i < points.Count; i++)
          //  points[i].reset();

        points.Clear();
        points = new List<MapPoint>();
        //points.RemoveRange(0, points.Count);

    }

    public int getNumLevels()
    {
        return zoomIntervals;
    }

    public bool hasData()
    {
        return points.Count > 0;
    }

    public void setZoomData(float min, float max, int intervals)
    {
        this.zoomMin = min;
        this.zoomMax = max;
        this.zoomIntervals = intervals;
    }

    public void setQuadsHorizontal(int numQuads)
    {
        this.numQuadsHoriz = numQuads;
        this.numQuadsVert = (int)(numQuads * (zone.height / zone.width));
    }

    public void addPoints(List <MapPoint> pointList)
    {
        this.points = pointList;
    }

    public void update()
    {
        int i = 0;
        MapLevel lastLevel=null;

        //Debug.Log("Actualizando para " + points.Count + "");
  
        for (i = 0; i < zoomIntervals; i++)
        {
            
            MapLevel level = new MapLevel(this.zone, this.numQuadsHoriz, this.numQuadsVert, i);            
            level.managePoints(this.points);
            //Debug.Log("Creando nivel " + i + " que tiene " +level.getGridClusters().Count);

            if (lastLevel!=null)
                arrangeRelatives(level, lastLevel);

            lastLevel = level;
            this.levels.Add(level);
        }
    }

    protected void arrangeRelatives(MapLevel level, MapLevel lastLevel)
    {
        foreach (GridCluster clusterChild in level.getGridClusters())
        {
            MapPoint childPoint = clusterChild.getPoint(0);

            GridCluster clusterParent = lastLevel.getClusterWithPoint(childPoint);

            if (clusterParent != null)
            {
                clusterChild.setParent(clusterParent);
                clusterParent.addChild(clusterChild);
            }
        }
    }

    public int getLevel(float zoomPos)
    {
        int level = (int)(((zoomPos - this.zoomMin) * this.zoomIntervals) / (this.zoomMax - this.zoomMin));

        if (level >= this.zoomIntervals)
            level = this.zoomIntervals-1;

        return level;
    }


    public void showInfo()
    {
        Debug.Log("En este manager se gestiona la siguiente información............");
        Debug.Log("Hay ....... " + points.Count + " puntos.");
        Debug.Log("Hay ........" + levels.Count + " niveles.");
        for (int i = 0; i < levels.Count; i++)
            levels[i].showInfo();
    }

    public List<GridCluster> getGridClustersAtLevel(int level)
    {
        if (levels.Count > 0 && level < levels.Count)
            return levels[level].getGridClusters();
        else
            return null;
    }

    public MapLevel getMapLevel(int level)
    {
        return levels[level];
    }

}
