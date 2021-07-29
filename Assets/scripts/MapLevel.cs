using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLevel  {

    protected Rect zone;
    public int numQuadsH;
    public int numQuadsV;
    protected int level;
    protected float factorX = 0.0f;
    protected float factorY = 0.0f;
    public float quadHeight = 0.0f;
    public float quadWidth = 0.0f;

    protected List<GridCluster> clusters = new List<GridCluster>();
    //protected List<QuadCluster> quadClustersList = new List<QuadCluster>();

	public MapLevel(Rect zone, int numQuadsH, int numQuadsV, int level)
    {
        this.zone = zone;
        int times = pow(2, level);
        this.numQuadsH = numQuadsH * times;
        this.numQuadsV = numQuadsV * times;
        this.level = level;
        this.factorX = 1 / zone.width;
        this.factorY = 1 / zone.height;
        this.quadWidth = zone.width / this.numQuadsH;
        this.quadHeight = zone.height / this.numQuadsV;
    }

    public float getMinimum()
    {
        return this.quadWidth*2.0f;
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

    /*

    public GridCluster getClusterWithPointInQuad(MapPoint point)
    {
        int h = this.getHGrid(point.getX());
        int v = this.getVGrid(point.getY());

        List<GridCluster> clusterList = this.getGridClustersPerQuad(h, v);

        if (clusterList == null)
            clusterList = this.getGridClustersPerQuad(h - 1, v);
        else
            clusterList.AddRange(this.getGridClustersPerQuad(h - 1, v));
        
        if (clusterList == null)
            clusterList = this.getGridClustersPerQuad(h - 1, v + 1);
        else
            clusterList.AddRange(this.getGridClustersPerQuad(h - 1, v + 1));

        if (clusterList == null)
            clusterList = this.getGridClustersPerQuad(h - 1, v - 1);
        else
            clusterList.AddRange(this.getGridClustersPerQuad(h - 1, v - 1));

        if (clusterList == null)
            clusterList = this.getGridClustersPerQuad(h + 1, v);
        else
            clusterList.AddRange(this.getGridClustersPerQuad(h + 1, v));

        if (clusterList == null)
            clusterList = this.getGridClustersPerQuad(h + 1, v - 1);
        else
            clusterList.AddRange(this.getGridClustersPerQuad(h+1, v-1));

        if (clusterList == null)
            clusterList = this.getGridClustersPerQuad(h + 1, v + 1);
        else
            clusterList.AddRange(this.getGridClustersPerQuad(h + 1, v + 1));

        if (clusterList == null)
            clusterList = this.getGridClustersPerQuad(h, v - 1);
        else
            clusterList.AddRange(this.getGridClustersPerQuad(h, v-1));

        if (clusterList == null)
            clusterList = this.getGridClustersPerQuad(h, v + 1);
        else
            clusterList.AddRange(this.getGridClustersPerQuad(h, v + 1));
        }

        return null;
    }*/

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
        //return (int)((((pos - zone.xMin) * this.numQuadsH) * this.factorX)); /// zone.width) );
        return (int) ((pos - zone.xMin) / this.quadWidth)+1;
    }

    public int getVGrid(float pos)
    {
        //return (int)((((pos - zone.yMin) * this.numQuadsV) * this.factorY)); /// zone.height) );
        return (int)((pos - zone.yMin) / this.quadHeight)+1;
    }

    /*

    public QuadCluster getQuadCluster(int h, int v)
    {
        int i = 0;
        QuadCluster quadC = null;

        while (i<quadClustersList.Count && quadC==null)
        {
            if (quadClustersList[i].h == h && quadClustersList[i].v == v)
                quadC = quadClustersList[i];
            i++;
        }

        if (quadC == null)
        {
            quadC = new QuadCluster(h, v);
            quadClustersList.Add(quadC);
        }

        return quadC;
    }
    */

    public int getLevel()
    {
        return this.level;
    }

    public void addCluster(GridCluster cluster)
    {
        this.clusters.Add(cluster);
        cluster.setLevel(this.getLevel());

    }


    public void managePoints(List<MapPoint> points, float radius)
    {

        int numPoints = points.Count;
        bool pointKey = false;

        radius = radius * radius;

        int baseLevel = -100;

        //Debug.Log("el radio sin raiz es " + radius);

        for (int i = 0; i < numPoints; i++)
        {
            //Debug.Log("Analizando punto " + i + " = " + points[i].getX() + "," + points[i].getY());



            if (!points[i].clusteredLevel)
            {
                GridCluster radialCluster = points[i].isInGroupPoint;

                if (radialCluster==null)
                {
                    radialCluster = new GridCluster(0, 0, points[i], this);
                    //radialCluster.addPoint(points[i]);
                    //points[i].setGridCluster(radialCluster);
                    points[i].addCluster(radialCluster);
                    points[i].clusteredLevel = true;
                }
                else
                {
                    /*
                    for (int gP = 0; gP < radialCluster.getNumPoints(); gP++)
                    {
                        radialCluster.setLevel(this);
                        points[gP].addCluster(radialCluster);
                        points[gP].clusteredLevel = true;
                    }*/
                }

                //Debug.Log("Se crea el cluster con punto "+i);

                for (int j = 0; j < numPoints; j++)
                {
                    //Debug.Log("Analizando emparejamiento con punto " + j + " = " + points[j].getX() + "," + points[j].getY());
                    if (!points[j].clusteredLevel && j != i )
                    {
                        //Debug.Log("Distancia con punto " + j + " es de " + getDistance(points[i], points[j]));
                        if (getDistance(points[i], points[j]) <= radius)
                        {
                            radialCluster.addPoint(points[j]);
                            points[j].addCluster(radialCluster);
                            points[j].clusteredLevel = true;
                        }
                    }
                }

                if (pointKey)
                {
                    Debug.Log("El cluster radial tiene " + radialCluster.getNumPoints());
                    pointKey = false;
                }

                if (radialCluster.getNumPoints() > 0)
                {
                    radialCluster.update();
                    this.addCluster(radialCluster);
                }
                else
                {
                    // points[i].removeCluster(radialCluster);
                    //points[i].clusteredLevel = false;

                    //for (int auxC=0;auxC<auxList.Count;auxC++)
                    //{
                      //  auxList[auxC].removeCluster(radialCluster);
                    //    auxList[auxC].clusteredLevel = false;
                  //  }
                }
                //auxList.Clear();
            }
        }


        //Debug.Log("En el nivel base hay " + this.clusters.Count + " clusters mayores de 1 punto");

        int punto1 = 0;

        for (int i = 0; i < numPoints; i++)
        {
            if (!points[i].clusteredLevel)
            {
                GridCluster radialCluster = new GridCluster(0, 0, points[i], this);
                //radialCluster.addPoint(points[i]);
                //points[i].setGridCluster(radialCluster);
                points[i].clusteredLevel = true;
                points[i].addCluster(radialCluster);
                this.addCluster(radialCluster);
                punto1++;

            }
        }

        //Debug.Log("En el nivel base hay " + punto1 + " clusters de 1 punto");

    }

    protected float getDistance(MapPoint a, MapPoint b)
    {
        return (b.getX() - a.getX()) * (b.getX() - a.getX()) + (b.getY() - a.getY()) * (b.getY() - a.getY());
    }

    public void managePoints(List <MapPoint> points)
    {
        int i = 0;
        int h, v;
        int auxLevel;
        bool found;

        int numPoints = points.Count;
        
        for (i=0;i<numPoints;i++)
        {
            //h = getHGrid(points[i].getMarker().position.y);
            //v = getVGrid(points[i].getMarker().position.x);

            //h = getHGrid(points[i].getY());
            //v = getVGrid(points[i].getX());

            auxLevel = 0;
            found = false;

            int numClusters = this.clusters.Count;

            while (!found && auxLevel < numClusters) {
                found = this.clusters[auxLevel].checkPoint(points[i].getX(), points[i].getY()); //.check(h, v);//,points[i].getCategory());                
                auxLevel++;
            }
            if (found)
            {
                this.clusters[auxLevel - 1].addPoint(points[i]);
                points[i].addCluster(this.clusters[auxLevel - 1]);
            }
            else
            {

                h = getHGrid(points[i].getX());
                v = getVGrid(points[i].getY());
                GridCluster newGridCluster = new GridCluster(h, v, points[i],this);
                newGridCluster.setLevel(this.level);
                points[i].addCluster(newGridCluster);
                this.clusters.Add(newGridCluster);

                //QuadCluster quadC = this.getQuadCluster(h, v);
                //quadC.addCluster(newGridCluster);

                //if (level==0 || level== 1)
                //{
                  //  Debug.Log("Se crea quad en nivel " + level + " con h= " + h + " , v=" + v);
               // }

            }
            
        }
    }

    public void managePoints(GridCluster cluster)
    {
        int i = 0;
        int h, v;
        int auxLevel;
        bool found;
        List<MapPoint> points = cluster.getPoints();

        //Debug.Log("Managin cluster with " + points.Count);

        int numPoints = points.Count;

        for (i = 0; i < numPoints; i++)
        {
            //h = getHGrid(points[i].getMarker().position.y);
            //v = getVGrid(points[i].getMarker().position.x);

            //h = getHGrid(points[i].getY());
            //v = getVGrid(points[i].getX());

            auxLevel = 0;
            found = false;

            int numClusters = this.clusters.Count;

            while (!found && auxLevel < numClusters)
            {
                found = this.clusters[auxLevel].checkPoint(points[i].getX(), points[i].getY());// h, v);//,points[i].getCategory());                
                auxLevel++;
            }

            if (found)
            {
                this.clusters[auxLevel - 1].addPoint(points[i]);
                points[i].addCluster(this.clusters[auxLevel - 1]);
            }
            else
            {
                h = getHGrid(points[i].getX());
                v = getVGrid(points[i].getY());
                GridCluster newGridCluster = new GridCluster(h, v, points[i],this);
                newGridCluster.setLevel(this.level);
                points[i].addCluster(newGridCluster);

                newGridCluster.setParent(cluster);
                cluster.addChild(newGridCluster);

                this.clusters.Add(newGridCluster);

                //QuadCluster quadC = this.getQuadCluster(h, v);
                //quadC.addCluster(newGridCluster);

                //if (level==0 || level== 1)
                //{
                //  Debug.Log("Se crea quad en nivel " + level + " con h= " + h + " , v=" + v);
                // }

            }

        }
    }


    public Vector2 getRectCenter(Rect rect)
    {
        Vector2 center = new Vector2();
        center.x = (rect.xMax - rect.xMin) / 2.0f;
        center.y = (rect.yMax - rect.yMin) / 2.0f;

        return center;
    }

    public bool isInRect(float x, float y, Rect rect)
    {
        return x >= rect.xMin && x <= rect.xMax && y >= rect.yMin && y <= rect.yMax;
    }

    public void managePointsNew(GridCluster cluster)
    {
        int i = 0;
        List<MapPoint> points = cluster.getPoints();

        //Debug.Log("Managin cluster with " + points.Count);

        int numPoints = points.Count;

        List<GridCluster> newClusters = new List<GridCluster>();
        float width = (cluster.maxX - cluster.minX) / 2.5f;
        float heigth = cluster.maxY - cluster.minY;
        Vector2 size = new Vector2(width, heigth);

        if (size.x != 0.0f)
        {

            Rect area1 = new Rect(new Vector2(cluster.minX, cluster.minY), size);
            //Rect area2 = new Rect(new Vector2(cluster.minX + (cluster.maxX - cluster.minX) / 3.0f, cluster.minY), size);
            Rect area3 = new Rect(new Vector2(cluster.maxX - (cluster.maxX - cluster.minX) / 3.0f, cluster.minY), size);

            Vector2 center1 = getRectCenter(area1);
            //Vector2 center2 = getRectCenter(area2);
            Vector2 center3 = getRectCenter(area3);

            GridCluster cluster1 = new GridCluster(0,0);  // this.getHGrid(center1.x), this.getVGrid(center1.y));
                                                          // GridCluster cluster2 = new GridCluster(0,0); //;this.getHGrid(center2.x), this.getVGrid(center2.y));
            GridCluster cluster3 = new GridCluster(0, 0); // this.getHGrid(center3.x), this.getVGrid(center3.y));

            for (i = 0; i < numPoints; i++)
            {
                MapPoint p = cluster.getPoints()[i];

                if (isInRect(p.getX(), p.getY(), area1))
                {
                    p.addCluster(cluster1);
                    cluster1.addPoint(p);
                }
                else
                {
                    /*
                    if (isInRect(p.getX(), p.getY(), area2))
                    {
                        p.addCluster(cluster2);
                        cluster2.addPoint(p);
                    }
                    else
                    {*/
                        p.addCluster(cluster3);
                        cluster3.addPoint(p);
                    //}
                }
            }

            int numclusters = 0;

            if (cluster1.getNumPoints() > 0)
            {
                cluster1.setParent(cluster);
                cluster1.setLevel(this.level);
                cluster.addChild(cluster1);
                this.clusters.Add(cluster1);
                numclusters++;
            }

            /*
            if (cluster2.getNumPoints() > 0)
            {
                cluster2.setParent(cluster);
                cluster.addChild(cluster2);
                cluster2.setLevel(this.level);
                this.clusters.Add(cluster2);
                numclusters++;
            }*/

            if (cluster3.getNumPoints() > 0)
            {
                cluster3.setParent(cluster);
                cluster.addChild(cluster3);
                cluster3.setLevel(this.level);
                this.clusters.Add(cluster3);
                numclusters++;
            }
            //Debug.Log("Se crean "+ numclusters+ " clusters normales en nivel " + this.level);
        }
        else // grouppoint o clusters de 1 punto
        {
            //Debug.Log("Se crea un cluster de grouppoint/1 punto en nivel " + this.level);
            GridCluster cluster1 = new GridCluster(0, 0, cluster.getPoints()[0]);
            cluster1.setLevel(this.level);
            cluster1.setParent(cluster);
            cluster.addChild(cluster1);
            this.clusters.Add(cluster1);
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
        //Debug.Log("En este nivel .......");
        //Debug.Log("Hay una resolucion de " + numQuadsH + " x " + numQuadsV + " quads-");
        //Debug.Log("Hay un total de " + clusters.Count + " clusters.");
    }

    public List<GridCluster> getGridClusters()
    {
        return this.clusters;
    }

    /*
    public List<GridCluster> getGridClustersPerQuad(int h, int v)
    {
        int i = 0;
        List<GridCluster> clusterList = null;

        while (i<quadClustersList.Count && clusterList ==null)
        {
            if (quadClustersList[i].h==h && quadClustersList[i].v==v)
                clusterList = quadClustersList[i].getClusters();
            i++;
        }

        if (clusterList == null)
            return new List<GridCluster>();

        return clusterList;
    }*/
}
