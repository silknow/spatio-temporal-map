using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLevel  {

    protected Rect zone;
    protected int numQuadsH;
    protected int numQuadsV;
    protected int level;

    protected List<GridCluster> clusters = new List<GridCluster>();

	public MapLevel(Rect zone, int numQuadsH, int numQuadsV, int level)
    {
        this.zone = zone;
        int times = pow(2, level);
        this.numQuadsH = numQuadsH * times;
        this.numQuadsV = numQuadsV * times;
        this.level = level;
    }

    public int getNumQuads(){
        return numQuadsH * numQuadsV;
    }

    public GridCluster getClusterWithPoint(MapPoint point)
    {
        foreach (GridCluster cluster in clusters)
        {
            if (cluster.getPoints().Contains(point))
                return cluster;
        }

        return null;
    }

    public void reset()
    {
        for (int i = 0; i < clusters.Count; i++)
            clusters[i].reset();

        clusters.Clear();

        //clusters.RemoveRange(0, clusters.Count);
    }

    public List<MapPoint> getPointsOfQuad(int numQuad)
    {
        List<MapPoint> quadPoints = new List<MapPoint>();

        float quadWidth = zone.width / numQuadsH;
        float quadHeight = zone.height / numQuadsV;

        int incQuadX = numQuad % numQuadsH;
        int incQuadY = (int)(numQuad / numQuadsH);

        float xIni = zone.xMin + (incQuadX)*quadWidth;
        float yIni = zone.yMin + (incQuadY)*quadHeight;

        quadPoints.Add(new MapPoint(xIni, yIni));
        quadPoints.Add(new MapPoint(xIni + quadWidth, yIni));
        quadPoints.Add(new MapPoint(xIni + quadWidth, yIni + quadHeight));
        quadPoints.Add(new MapPoint(xIni, yIni + quadHeight));

        return quadPoints;
    }

    public List<GridCluster>getClustersPerCategory(string category)
    {
        List<GridCluster> clusterCategoryList = new List<GridCluster>();

        for (int i = 0; i < clusters.Count; i++)
            if (clusters[i].getCategory().Equals(category))
                clusterCategoryList.Add(clusters[i]);

        return clusterCategoryList;

    }

    public int getHGrid(float pos)
    {
        return (int)( (((pos - zone.xMin) * this.numQuadsH) / zone.width) );
    }

    public int getVGrid(float pos)
    {
        return (int)( (((pos - zone.yMin) * this.numQuadsV) / zone.height) );
    }

    public void managePoints(List <MapPoint> points)
    {
        int i = 0;
        int h, v;
        int auxLevel;
        bool found;
        
        for (i=0;i<points.Count;i++)
        {
            //h = getHGrid(points[i].getMarker().position.y);
            //v = getVGrid(points[i].getMarker().position.x);

            h = getHGrid(points[i].getY());
            v = getVGrid(points[i].getX());

            auxLevel = 0;
            found = false;

            while (!found && auxLevel < this.clusters.Count)
            {
                found = this.clusters[auxLevel].check(h, v,points[i].getCategory());
                auxLevel++;
            }

            if (found)
            {
                this.clusters[auxLevel - 1].addPoint(points[i]);
                points[i].addCluster(this.clusters[auxLevel - 1]);
            }
            else
            {
                GridCluster newGridCluster = new GridCluster(h, v, points[i]);
                newGridCluster.setLevel(this.level);
                points[i].addCluster(newGridCluster);
                this.clusters.Add(newGridCluster);
            }
            
        }
    }

    public int pow(int a, int b)
    {
        int result = 1;
        int i = 1;

        for (i = 0; i < b; i++)
            result = a*result;

        return result;
    }

    public void showInfo()
    {
        Debug.Log("En este nivel .......");
        Debug.Log("Hay una resolucion de " + numQuadsH + " x " + numQuadsV + " quads-");
        Debug.Log("Hay un total de " + clusters.Count + " clusters.");
    }

    public List<GridCluster> getGridClusters()
    {
        return this.clusters;
    }

}
