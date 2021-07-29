using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCluster
{
    public int hQuad;
    public int vQuad;
    public string category="";
    protected GridCluster parent = null;
    protected List<GridCluster> childs = new List<GridCluster>();
    protected Dictionary<MapPoint, RelationShip> relationsPerPoint = new Dictionary<MapPoint, RelationShip>();
    public bool groupPoints = false;

    const int NO_VALUE = -500;
    List<MapPoint> points = new List<MapPoint>();
    // Conexiones
    List<int> connectionsList = new List<int>();
    MapPoint center = null;
    public float maxX = NO_VALUE, maxY = NO_VALUE, minX=NO_VALUE, minY=NO_VALUE;
    public Rect gridZone;
    public bool processed = false;
    public static int MIN_SIZE = 5;
    private GridCluster associatedTo=null;
    public GridCluster closest = null;

    int level = 0;

    int numFilteredPoints = 0;

    public GridCluster(int h, int v, MapPoint point)
    {
        this.hQuad = h;
        this.vQuad = v;
        //this.points.Add(point);
        addPoint(point);
        this.category = point.getCategory();
        this.numFilteredPoints = 0;        
    }


    public GridCluster(GridCluster a, GridCluster b)
    {
        this.numFilteredPoints = 0;
        this.points.AddRange(a.getPoints());
        this.points.AddRange(b.getPoints());
        a.setParent(this);
        b.setParent(this);
        this.addChild(a);
        this.addChild(b);
        this.setAverageCenter(a.getCenter(), b.getCenter());        
        
    }

    public void associatePoints()
    {
        for (int i = 0; i < this.points.Count; i++)
        {

            points[i].addCluster(this);
        }
    }

    public void associateTo(GridCluster gridCluster)
    {
        this.associatedTo = gridCluster;
    }

    public GridCluster getAssociatedTo()
    {
        return this.associatedTo;
    }

    public GridCluster(GridCluster a)
    {
        this.numFilteredPoints = 0;
        this.points.AddRange(a.getPoints());        
        a.setParent(this);        
        this.addChild(a);
        this.center = new MapPoint(a.getCenter().getX(), a.getCenter().getY());        
    }

    public void setAverageCenter(MapPoint a, MapPoint b)
    {
        this.setCenter(new MapPoint((a.getX() + b.getX()) / 2.0f, (a.getY() + b.getY()) / 2.0f));
    }

    public GridCluster(int h, int v, MapPoint point, MapLevel level)
    {
        this.hQuad = h;
        this.vQuad = v;
        //this.points.Add(point);
        addPoint(point);
        //point.setGridCluster(this);
        this.category = point.getCategory();
        this.numFilteredPoints = 0;        
        this.gridZone.xMin = level.quadWidth * (h - level.numQuadsH/2 - 1);
        this.gridZone.yMin = level.quadHeight * (v - level.numQuadsV/2 - 1);

        this.center = new MapPoint(point.getX(), point.getY());

        this.gridZone.width = level.quadWidth;
        this.gridZone.height = level.quadHeight;
        this.numFilteredPoints = 0;
    }

    public void setLevel(MapLevel level)
    {
        this.hQuad = 0;
        this.vQuad = 0;

        this.numFilteredPoints = 0;

        this.gridZone.xMin = level.quadWidth * (0 - level.numQuadsH / 2 - 1);
        this.gridZone.yMin = level.quadHeight * (0 - level.numQuadsV / 2 - 1);
        this.gridZone.width = level.quadWidth;
        this.gridZone.height = level.quadHeight;
    }

    public GridCluster(int h, int v)
    {
        this.hQuad = h;
        this.vQuad = v;

        this.numFilteredPoints = 0;
    }
    public int getNumVisiblePoints()
    {
        //Debug.Log("El cluster tiene " + getNumPoints() + " puntos");
        //Debug.Log("El cluster tiene " +numFilteredPoints + " puntos filtrados");

        //if (numFilteredPoints > getNumPoints())
        //    return 0;
        //else
            return getNumPoints() - numFilteredPoints;
    }


    public GridCluster(List <MapPoint> pointsGroup)
    {
        foreach (MapPoint p in pointsGroup)
        {
            addPoint(p);
            p.setGroupPoint(true);
        }

        this.update();

        center.setXY(pointsGroup[0].getX(), pointsGroup[0].getY());

        this.numFilteredPoints = 0;

        this.groupPoints = true;
    }

    public void addPointDirect(MapPoint point)
    {
        this.points.Add(point);
        point.setGridCluster(this);
    }

    public bool isGroupPoints()
    {
        return this.groupPoints;
    }

    public void setParent(GridCluster cluster)
    {
        this.parent = cluster;
    }

    public void addChild(GridCluster child)
    {
        if (!this.childs.Contains(child))
            this.childs.Add(child);
    }

    public List<GridCluster> getChilds()
    {
        return childs;
    }

    public RelationShip getRelationPerPoint(MapPoint point)
    {
        RelationShip relation = null;

        if (relationsPerPoint.ContainsKey(point))
            relation = relationsPerPoint[point];

        return relation;
    }

    public void addRelationsPerPoint(MapPoint point, List<GridCluster> relatedClusters)
    {
        if (!relationsPerPoint.ContainsKey(point))
        {
            List<MapPoint> clusterCenters = new List<MapPoint>();
            foreach (GridCluster cluster in relatedClusters)
                clusterCenters.Add(cluster.getCenter());
            RelationShip relation = new RelationShip(this.getCenter(), clusterCenters, point.getLabel());
            relationsPerPoint.Add(point, relation);

            propagateRelationWith(point,relatedClusters);
        }
    }

    public GridCluster getParent()
    {
        return parent;
    }

    protected void propagateRelationWith(MapPoint point, List<GridCluster> relatedClusters)
    {
        if (this.parent!=null)
        {
            //Debug.Log("El padre NO es NULL");
            /*if (parent.getCenter().isCluster())
                Debug.Log("El padre SI es un cluster");
            else
                Debug.Log("El padre NO es un cluster");
            */
            List <GridCluster> parentRelatedClusters = new List<GridCluster>();
            foreach (GridCluster cluster in relatedClusters)
                if (!parentRelatedClusters.Contains(cluster.parent) && cluster.parent != parent)
                    parentRelatedClusters.Add(cluster.parent);
            parent.addRelationsPerPoint(point, parentRelatedClusters);
        } 
        /*else
            Debug.Log("El padre SI es NULL");*/
    }


    public void showRelations()
    {
        foreach (MapPoint p in relationsPerPoint.Keys)        
            showRelationsPerPoint(p);        
    }

    public void hideRelations()
    {
        foreach (MapPoint p in relationsPerPoint.Keys)
            hideRelationsPerPoint(p);
    }

    public void showRelationsPerPoint(MapPoint point)
    {
        //Debug.Log("llamando a SHOW relations per point");
        if (relationsPerPoint.ContainsKey(point))
        {
            RelationShip relation = relationsPerPoint[point];

            relation.show();

            center.showClusterRelations(point);
        }
    }

    public void hideRelationsPerPoint(MapPoint point)
    {
        //Debug.Log("llamando a HIDE relations per point");
        if (relationsPerPoint.ContainsKey(point))
        {
            RelationShip relation = relationsPerPoint[point];
            relation.hide();
            center.hideClusterRelations(point);
        }

    }


    public void removeRelationsPerPoint(MapPoint point)
    {
        if (relationsPerPoint.ContainsKey(point))
        {
            RelationShip relation = relationsPerPoint[point];
            relation.remove();
            relationsPerPoint.Remove(point);

            center.removeGraphicRelations(point);

            if (this.parent != null)
                this.parent.removeRelationsPerPoint(point);
        }
    }

    public void addFilteredPoint()
    {
        //Debug.Log("Se esta filtrando ");
        if (numFilteredPoints < points.Count)
        {
            numFilteredPoints++;

            //if (center.getLabel().Trim().Equals("Cluster 2"))
            //    Debug.Log("En cluster " + center.getLabel() + " hay " + numFilteredPoints + " de " + points.Count);

            if (numFilteredPoints == points.Count)
                center.setFiltered(true);
        }
    }

    public void removeFilteredPoint()
    {
        if (numFilteredPoints > 0)
        {
            numFilteredPoints--;

            if (center.isFiltered())
                center.setFiltered(false);
        }
    }

    public void reset()
    {
        if (center!=null)
            center.reset();
        connectionsList.Clear();
        points.Clear();
        //connectionsList.RemoveRange(0, connectionsList.Count);
        //points.RemoveRange(0,points.Count);

    }

    public string getCategory()
    {
        return this.category;
    }

    public void initConnectionsList(int numClusters)
    {
        for (int c = 0; c < numClusters; c++)
            connectionsList.Add(0);
    }

    public int getLevel()
    {
        return level;
    }

    public List<int> getConnections()
    {
        return connectionsList;
    }



    public void setLevel(int level)
    {
        this.level = level;
    }

    public void addPoint(MapPoint point)
    {
        this.points.Add(point);

        if (minX == NO_VALUE || minX > point.getX())
            minX = point.getX();

        if (minY == NO_VALUE || minY > point.getY())
            minY = point.getY();

        if (maxX == NO_VALUE || maxX < point.getX())
            maxX = point.getX();

        if (maxY == NO_VALUE || maxY < point.getY())
            maxY = point.getY();

        //update();
    }

    public int getNumPoints()
    {
        return points.Count;
    }

    public void update()
    {
        float xsum=0.0f, ysum=0.0f;
        int i;

        for (i=0;i<points.Count;i++)
        {
            //xsum += points[i].getMarker().position.x;
            //ysum += points[i].getMarker().position.y;

            xsum += points[i].getX();
            ysum += points[i].getY();
        }

        if (center == null)
            center = new MapPoint(xsum / points.Count, ysum / points.Count);
        else
            center.setXY(xsum / points.Count, ysum / points.Count);
    }


    public void updateConnections(List<GridCluster> clusters)
    {
        for (int p = 0; p < this.getNumPoints(); p++)
        {
            MapPoint auxPoint = this.getPoint(p);
            for (int connect = 0; connect < auxPoint.getRelations().Count; connect++)
            {
                for (int clusterConnected = 0; clusterConnected < auxPoint.getRelations()[connect].getTo().getRelatedClusters().Count; clusterConnected++)
                {
                    GridCluster auxCluster = auxPoint.getRelations()[connect].getTo().getRelatedClusters()[clusterConnected];
                    if (auxCluster.getLevel() == 0 && auxCluster != this)
                    {
                        int levelCluster = 0;
                        bool clusterLevelFound = false;
                        while (!clusterLevelFound && levelCluster < clusters.Count)
                        {
                            clusterLevelFound = (clusters[levelCluster] == auxCluster);
                            levelCluster++;
                        }
                        if (clusterLevelFound)
                        {
                            connectionsList[levelCluster - 1]++;
                            //Debug.Log("Hay conexión con cluster " + (levelCluster - 1));
                        }
                    }
                }
            }
        }
    }


    public MapPoint getCenter()
    {
        return center;
    }

    public void setCenter(MapPoint center)
    {
        this.center = center;
        center.setGridCluster(this);
    }

    //, string category)
    public bool check(int h, int v) 
    {
        return (this.hQuad == h) && (this.vQuad == v);// && (this.category.Equals(category));
    }

    public bool checkPoint(float x, float y)
    {
        return gridZone.xMin <= x && gridZone.yMin <= y && gridZone.xMax >= x && gridZone.yMax >= y;
    }

    public MapPoint getPoint(int i)
    {
        if (i < points.Count)
            return points[i];
        else
            return null;
    }

    public Rect getZone()
    {
        return new Rect(minX, minY, maxX - minX, maxY - minY);
    }

    public int isPointWithURI(string URI)
    {
        int position = 0;
        bool found = false;
        int i = 0;

        while (!found && i<points.Count)
        {
            found = (points[i].getURI().Equals(URI));
            i++;
        }

        if (found)
            position = i - 1;

        return position;
    }

    public List<MapPoint> getPoints()
    {
        return this.points;
    }

    public List<MapPoint> getNoFilteredPoints()
    {
        List<MapPoint> pointsNoFiltered = new List<MapPoint>();

        foreach (MapPoint p in getPoints())
            if (!p.isFiltered())
                pointsNoFiltered.Add(p);

        return pointsNoFiltered;
    }
}
