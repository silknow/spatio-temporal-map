using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map {

    protected List<MapPoint> points = new List<MapPoint>();
    protected ClusterManager clusterManager;
    protected float zoom;
    protected Vector3 viewerPosition;
    protected int visualizedClustersLevel;
    protected bool visualizedQuads;

    const int NO_LEVEL_VISUALIZED = -1; 
    
    public Map()
    {
        clusterManager = new ClusterManager();
        visualizedClustersLevel = NO_LEVEL_VISUALIZED;
        visualizedQuads = false;
        viewerPosition = new Vector3();
        zoom = -1;
    }

    public void reset()
    {
        clusterManager.reset();

        for (int i = 0; i < points.Count; i++)
            points[i].reset();

        points.RemoveRange(0, points.Count);
    }

    public void addPoint(MapPoint point)
    {
        points.Add(point);

    }

    public void addPoint(float x, float y)
    {
        MapPoint point = new MapPoint(x, y);
        points.Add(point);
    }

    public void addPoints(List<MapPoint> points)
    {
        this.points.AddRange(points);        
    }

    public void setGridingZoomData(float minZoom, float maxZoom, int levels)
    {
        clusterManager.setZoomData(minZoom, maxZoom, levels);
    }

    public void setGridingQuadsHorizonal(int numQuads)
    {
        clusterManager.setQuadsHorizontal(numQuads);
    }

    public void updateClustering()
    {
        clusterManager.addPoints(points);
        clusterManager.update();
    }

    public void showClusters()
    {
        Debug.Log("Map-->showClusters");
        showClustersAtZoom(this.zoom);
    }

    public void hideClusters()
    {
        hideClustersAtZoom(this.zoom);
    }

    public virtual void showClustersAtZoom(float zoom)
    {
        Debug.Log("Map-->showClustersAtZoom");
    }

    public virtual void hideClustersAtZoom(float zoom)
    {

    }

    public void setClustersPoints()
    {

    }

    public void hideClustersPoints()
    {

    }

    public virtual void setViewerPosition(float x, float y)
    {
        this.viewerPosition.x = x;
        this.viewerPosition.y = y;
    }

    public void setViewerZoom(float zoom)
    {
        //Debug.Log("El zoom es " + zoom);
        bool changeZoom = this.zoom!=zoom;
        bool init = (this.zoom == -1);

        if (!init && changeZoom)
        {
            hideClusters();
            this.zoom = zoom;
            showClusters();
        }
        else
        {
            this.zoom = zoom;
        }
    }

    public float getZoom()
    {
        return this.zoom;
    }

    public int getLevel()
    {
        return this.clusterManager.getLevel(this.zoom);
    }

}
