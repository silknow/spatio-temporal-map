using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;

public class Map {

    protected List<MapPoint> points = new List<MapPoint>();
    protected ClusterManager clusterManager;
    protected float zoom;
    protected Vector3 viewerPosition;
    protected int visualizedClustersLevel;
    protected bool visualizedQuads;
    protected int dimension = MapPoint.TWO_DIMENSION;
    protected PropertyManager propertyManager;
    protected FilterManager filterManager;
    protected Dictionary<Vector2, List<MapPoint>> positionsGroup = new Dictionary<Vector2, List<MapPoint>>();
    protected List<MapPoint> pointsWithRelation = new List<MapPoint>();
    protected bool stacked = false;

    const int NO_LEVEL_VISUALIZED = -1; 
    
    public Map()
    {
        clusterManager = new ClusterManager();
        propertyManager = new PropertyManager();
        filterManager = new FilterManager();
        visualizedClustersLevel = NO_LEVEL_VISUALIZED;
        visualizedQuads = false;
        viewerPosition = new Vector3();
        zoom = -1;        
    }


    public bool hasData()
    {
        return points.Count > 0;
    }

    public void reset()
    {
        //Debug.Log("llamando reset");
        clusterManager.reset();
        //clusterManager = new ClusterManager();

        for (int i = 0; i < points.Count; i++)
            points[i].reset();

        points.Clear();
        //points.RemoveRange(0, points.Count);
        OnlineMapsMarker3DManager.RemoveAllItems();
        OnlineMapsMarkerManager.RemoveAllItems();

        positionsGroup.Clear();
        propertyManager.resetPropertyValues();
        resetFilters();
        removeAllRelations();
        //removeAllClusters();
    }


    public void setStacked(bool status)
    {
        this.stacked = status;
    }


    public bool getStacked()
    {
        return this.stacked;
    }

    public List<MapPoint> pointsInTime(int from, int to)
    {
        List<MapPoint> pointsResult = new List<MapPoint>();

        foreach (MapPoint p in points) {
            if (p.getFrom() >= from && p.getTo() <= to)
               pointsResult.Add(p);

            if (p.getLabel().Equals("7236"))
                Debug.Log("El punto es de " + p.getFrom() + " hasta " + p.getTo());
        }

        return pointsResult;
    }

    public void removeAllClusters()
    {
        removeAllGraphicClusters();
    }

    public virtual void removeAllGraphicClusters() { 
    }

    public PropertyManager GetPropertyManager()
    {
        return propertyManager;
    }

    public bool pointsViewing()
    {
        int level = clusterManager.getLevel(zoom);

        if (level < clusterManager.getNumLevels() - 2)
            return false;
        else
            return true;
    }

    public void SetDimension(int dimension)
    {
        if (dimension != this.dimension)
        {
            this.dimension = dimension;

            changeProjection(dimension);

            foreach (MapPoint mPoint in points)
            {
                mPoint.setDimension(dimension);
            }
            updateClustersDimension(dimension);           

        }
    }

    public MapPoint getPointPerLabel(String label)
    {
        foreach (MapPoint p in points)
            if (label.Equals(p.getLabel()))
                return p;

        return null;
    }
    public MapPoint getPointPerUri(String uri)
    {
        foreach (MapPoint p in points)
            if (uri.Equals(p.getURI()))
                return p;

        return null;
    }


    public void addFilter(String propertyName, string value)
    {
        List<string> values = new List<string>();
        values.Add(value);
        this.addFilter(propertyName, values);
    }

    public void addFilter(string propertyName, List<string>values)
    {
        if (filterManager != null)
            filterManager.addFilter(propertyName, values);
    }

    /*
    public void displayRelations(string propertyName, List<string> values)
    {

    }

    public void hideRelations(MapPoint point, string propertyName)
    {
        Debug.LogFormat("Hiding relationship for property {0}",propertyName);
    }


    public void hideRelations(MapPoint point)
    {

    }

    public void hideRelations()
    {

    }

    public void displayRelations(MapPoint point, string propertyName, List<string> values)
    {
        Debug.LogFormat("Displaying relationship for property {0} with value {1}",propertyName,values[0]);
    }
    */

    public void addPointWithRelation(MapPoint point)
    {
        if (!pointsWithRelation.Contains(point))
            pointsWithRelation.Add(point);
    }

    public void removePointWithRelation(MapPoint point)
    {
        pointsWithRelation.Remove(point);
    }

    public void updateFilter(string propertyName, List<string>values)
    {
        if (filterManager != null)
            filterManager.updateFilter(propertyName, values);

        applyFilters();
    }

    public void removeFilter(string propertyName)
    {
        Debug.Log("llama a remove 1 "+propertyName);
        if (filterManager != null)
            filterManager.removeFilter(propertyName);
        
    }
   

    public void removeAllRelations()
    {
        foreach (MapPoint p in pointsWithRelation)
        {
            p.removeAllRelations();
        }

        pointsWithRelation.Clear();
    }

    public void removeFilter(string propertyName, List<string> valuesToRemove)
    {
        Debug.Log("llama a remove 2 "+propertyName);
        if (filterManager != null)
            filterManager.removeFilter(propertyName, valuesToRemove);

        applyFilters();
    }

    public void removeFilter(string propertyName, string value)
    {
        List<string> valuesToRemove;

        //Debug.Log("llama a remove 3 " + propertyName);

        if (filterManager != null)
        {
            valuesToRemove = new List<string>();
            valuesToRemove.Add(value);
            filterManager.removeFilter(propertyName, valuesToRemove);
        }        
    }

    public void applyFilters()
    {
        if (filterManager != null)
            filterManager.applyFilters(points);

        showClusters();
    }

    /*
    * Returns a List<string> with the name of the filtered properties 
    * */
    public List<string> getFilteredPropertiesName()
    {
        if (filterManager != null)
            return filterManager.getFilteredPropertiesName();
        else
            return null;
    }

    /**
     * Returns a List<string> with the values of a filtered property, which name is specified in the parameter
     * */
    public List<string> getFilteredPropertyValues(string propertyName)
    {
        if (filterManager != null)
            return filterManager.getFilteredPropertyValues(propertyName);
        else
            return null;
    }

    public void resetFilters()
    {
        if (filterManager != null)
            filterManager.resetFilters(points);

        showClusters();
    }

    /**
     * Distribute the points that are located in the same longitud,latitud in a circle
     * arround the original location.
     * */
    public void distributeGroupsOnCircle()
    {
        float incR = 0.1f;
        double incAng=6.28f;
        int level;
        double pointsPerLevel;
        int pointAtLevel;

        foreach (Vector2 v in positionsGroup.Keys)
        {
            List<MapPoint> pointList = positionsGroup[v];

            level = 0;
            pointAtLevel = 1;
            pointsPerLevel = Math.Pow(2, level);

            for (int i= 0;i < pointList.Count;i++) 
            {
                if (pointAtLevel > pointsPerLevel)
                {
                    pointAtLevel = 1;
                    level++;
                    pointsPerLevel = Math.Pow(2, level);
                    incAng = 6.28f / pointsPerLevel;
                }
                
                double x = v.x + Math.Cos(pointAtLevel * incAng) * incR * level;
                double y = v.y + Math.Sin(pointAtLevel * incAng) * incR * level;
           
                pointList[i].setXY(Convert.ToSingle(x), Convert.ToSingle(y));

                pointAtLevel++;                
            }            
        }

    }


    public int GetDimension()
    {
        return this.dimension;
    }

    public bool putInExistingGroup(MapPoint point)
    {
        float dis = 0.1f;
        bool put = false;

        foreach (Vector2 v in positionsGroup.Keys)
        {
            put = (Math.Abs(v.x - point.getX()) < dis && Math.Abs(v.y - point.getY()) < dis);
            if (put)
            {
                positionsGroup[v].Add(point);
                return true;
            }
        }

        return put;
    }

    public void checkPosition(MapPoint point)
    {
        List<MapPoint> points;        
       
        if (positionsGroup.Keys.Count == 0 || !putInExistingGroup(point)) {
            points = new List<MapPoint>();
            points.Add(point);
            positionsGroup.Add(new Vector2(point.getX(), point.getY()), points);
        }
    }

    public void addPoint(MapPoint point)
    {
        checkPosition(point);
        points.Add(point);
        point.setMap(this);

    }

    public Dictionary<Vector2, List<MapPoint>> getGroups()
    {
        return this.positionsGroup;
    }

    public void addPoint(float x, float y)
    {
        MapPoint point = new MapPoint(x, y);
        addPoint(point);
        //points.Add(point);
    }

    public void addPoints(List<MapPoint> points)
    {
        //this.points.AddRange(points);        
        foreach (MapPoint p in points)
            addPoint(p);
    }

    public List<MapPoint> poinstWithValuesAtProperty(string propertyName, List<string> values)
    {
        List<MapPoint> pointsWithSameValue = new List<MapPoint>();

        foreach (MapPoint p in this.points) {
            if (p.sameValuesAtProperty(propertyName, values))
                pointsWithSameValue.Add(p);
        }

        return pointsWithSameValue;
    }

    public void setGridingZoomData(float minZoom, float maxZoom, int levels)
    {
        clusterManager.setZoomData(minZoom, maxZoom, levels);
    }

    public void setGridingQuadsHorizonal(int numQuads)
    {
        clusterManager.setQuadsHorizontal(numQuads);
    }

    public void separatePoints()
    {

    }

    public void updateClustering()
    {
        clusterManager.addPoints(points);
        clusterManager.update();
    }

    public void activateTimeFrame(int from, int to)
    {
        filterManager.activateTimeFrame(from, to);
        filterManager.applyFilters(points);
        showClusters();
    }

    public void removeTimeFrame()
    {
        filterManager.removeTimeFrame();
        filterManager.applyFilters(points);
        showClusters();
    }

    public void showClusters()
    {
        //Debug.Log("Map-->showClusters");
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

    public virtual void updateClustersDimension(int dimension)
    {
   
    }

    public virtual void changeProjection(int dimension)
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
