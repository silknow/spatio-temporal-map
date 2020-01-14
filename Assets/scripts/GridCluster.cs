using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCluster
{
    int hQuad;
    int vQuad;
    public string category="";

    const int NO_VALUE = -500;
    List<MapPoint> points = new List<MapPoint>();
    // Conexiones
    List<int> connectionsList = new List<int>();
    MapPoint center = null;
    float maxX = NO_VALUE, maxY = NO_VALUE, minX=NO_VALUE, minY=NO_VALUE;

    int level = 0;

    public GridCluster(int h, int v, MapPoint point)
    {
        this.hQuad = h;
        this.vQuad = v;
        //this.points.Add(point);
        addPoint(point);
        this.category = point.getCategory();
    }

    public void reset()
    {
        connectionsList.RemoveRange(0, connectionsList.Count);
        points.RemoveRange(0,points.Count);

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

        update();
    }

    public int getNumPoints()
    {
        return points.Count;
    }

    public void update()
    {
        float x, y, xsum=0.0f, ysum=0.0f;
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
                            Debug.Log("Hay conexión con cluster " + (levelCluster - 1));
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

    public bool check(int h, int v, string category)
    {
        return (this.hQuad == h) && (this.vQuad == v) && (this.category.Equals(category));
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
}
