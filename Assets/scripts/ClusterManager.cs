using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class ClusterManager {

    List<MapPoint> points = new List<MapPoint>();
    List<MapLevel> levels = new List<MapLevel>();
    List<GridCluster> pointGroups = new List<GridCluster>();

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

        pointGroups.Clear();
        pointGroups = new List<GridCluster>();

        //points.RemoveRange(0, points.Count);

    }

    public void addPointGroup(GridCluster gCluster)
    {
        pointGroups.Add(gCluster);
    }

    public List<GridCluster> getPointGroupClusters()
    {
        return this.pointGroups;
    }

    public List<MapPoint> getPointGroupsPoints()
    {
        List<MapPoint> pointGroupsPoints = new List<MapPoint>();

        foreach (GridCluster gCluster in this.pointGroups)     
            pointGroupsPoints.Add(gCluster.getCenter());

        return pointGroupsPoints;        
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


        //List<MapPoint> levelPoints = this.points;

        //Debug.Log("Actualizando para " + points.Count + "");


        for (i=0;i<zoomIntervals;i++)
        {
            this.levels.Add(new MapLevel(this.zone, this.numQuadsHoriz, this.numQuadsVert, i));
        }
        
        
        for (i = zoomIntervals-2; i >=0; i--)
        {
            int numClos = 0;
            
            int punto1 = 0;
            //MapLevel level = new MapLevel(this.zone, this.numQuadsHoriz, this.numQuadsVert, i);
            MapLevel level = this.levels[i];


            if (lastLevel == null)
            {
                //Debug.Log("Hay "+ this.pointGroups.Count+" gouppoints");
                
                
                for (int pG = 0; pG<this.pointGroups.Count; pG++)
                {
                    this.pointGroups[pG].setLevel(level);
                    level.addCluster(this.pointGroups[pG]);
                    //this.pointGroups[pG].processed = true;
                }
                level.managePoints(this.points,0.5f);    
                
           
            }
            else 
            {
             
                
                List<GridCluster> clusterList = lastLevel.getGridClusters();// this.getGridClustersAtLevel(i+1);
                //Debug.Log("EN ZOOM INTERVAL " + i + "y nivel " + lastLevel.getLevel() + " hay ....");
                //Debug.Log("EN nivel " + lastLevel.getLevel() + " hay " + clusterList.Count + " clusters");

                //Debug.Log("El minimio del nivel " + lastLevel.getLevel() + " es " + level.getMinimum());
                if (clusterList != null) {

                    if (clusterList.Count == 1)
                    {
                        //Debug.Log("UNO Y SALE");
                        GridCluster newGridCluster = new GridCluster(clusterList[0]);
                        newGridCluster.associatePoints();
                        //newGridCluster.update();
                        level.addCluster(newGridCluster);
                        
  
                    }
                    else
                    {           
                        float minimum = level.getMinimum();

                        if (level.getLevel() <= 1)
                            minimum = minimum * 100.0f;
                        
                        //for (int o = 0;o<clusterList.Count;o++) 
                          //  clusterList[o].closest = getClosestTo(clusterList[o], clusterList, 0, minimum, false);
                        
                        int aux = 0;              
                        while (aux < clusterList.Count)
                        {
                            GridCluster currentCluster = clusterList[aux];
                            if (!clusterList[aux].processed)
                            {
                                    if (clusterList[aux].getPoints()[0].getURI().Equals(("http://data.silknow.org/object/5f7a48fe-d8ac-37d8-90e3-f4448e665a9b")))
                                        Debug.Log("Procesando");
                                    GridCluster closest = getClosestTo(clusterList[aux], clusterList, aux, minimum, false);

                                    if (closest != null)
                                    {
                                        GridCluster newGridCluster = new GridCluster(currentCluster, closest);
                                        newGridCluster.associatePoints();
                                        newGridCluster.update();
                                        level.addCluster(newGridCluster);
                                        currentCluster.processed = true;
                                        closest.processed = true;
                                        numClos++;
                                    }                              
                                    else
                                    {
                                        GridCluster newGridCluster = new GridCluster(currentCluster);
                                        newGridCluster.associatePoints();
                                        //newGridCluster.update();
                                        level.addCluster(newGridCluster);
                                        currentCluster.processed = true;
                                        punto1++;
                                    }
                                /*
                                }
                                else
                                {
                                    GridCluster newGridCluster = new GridCluster(currentCluster);
                                    newGridCluster.associatePoints();
                                    //newGridCluster.update();
                                    level.addCluster(newGridCluster);
                                    currentCluster.processed = true;
                                    punto1++;
                                    //closest.processed = true;
                                }*/
                            }
                            aux++;
                        }
                    }
                }
                //Debug.Log("EN el nivel " + i + " hay " + level.getGridClusters().Count);
                //Debug.Log("De los cuales " + punto1 + " son de 1 punto");
            }

            //Debug.Log(numClos + " clusters cercanos");
            lastLevel = level;

            //this.levels.Add(level);
        }

        for (int p = 0; p < points.Count; p++)
        {
            if (points[p].getURI().Equals("http://data.silknow.org/object/54e7582f-d37e-3e85-9cac-c26b52a7711b"))
            {
                MapPoint pn = points[p];
            }
            
            if (points[p].getURI().Equals("http://data.silknow.org/object/12eed6aa-578c-39c0-b0d8-ecf3cdfd68c8"))
            {
                MapPoint pn = points[p];
            }
            
            if (points[p].getURI().Equals("http://data.silknow.org/object/db5ed89d-fac0-3cf1-81ec-fc55a66b8252"))
            {
                MapPoint pn = points[p];
            }
                
        }
        
       
    }

    protected GridCluster getClosestTo(GridCluster from, List<GridCluster> clusterList, int since, float minimum, bool check)
    {
        GridCluster closest = null;
        GridCluster closest1 = null;
        string fromURI = from.getPoints()[0].getURI();
        
        float minDistance = 1000000.0f;
        float minDistance1 = 1000000.0f;
        //float minimum = 5.0f;

        int i = since;
        //i = 0;

        while (i < clusterList.Count)
        {
            if (!clusterList[i].processed && from != clusterList[i] ) //&& clusterList[i].getNumPoints()>1)
            {
                float currentDistance = getDistance(from.getCenter(), clusterList[i].getCenter());

                if (currentDistance <= minimum)
                {

                    if (check)
                    {
                        //GridCluster closest2 = getClosestTo(clusterList[i], clusterList, since, minimum, false);

                        GridCluster closest2 = clusterList[i].closest;
                        
                        if (closest2 != null)
                        {
                            if (closest2.getPoints()[0].getURI().Equals(fromURI))
                            {
                                if (currentDistance < minDistance && clusterList[i].getNumPoints() >= 1)
                                {
                                    minDistance = currentDistance;
                                    closest = clusterList[i];
                                }

                                /*
                                if (currentDistance < minDistance1 && clusterList[i].getNumPoints() == 1)
                                {
                                    minDistance1 = currentDistance;
                                    closest1 = clusterList[i];
                                }*/
                            }
                        }
                    }
                    else
                    {
                        if (currentDistance < minDistance && clusterList[i].getNumPoints() >= 1)
                        {
                            minDistance = currentDistance;
                            closest = clusterList[i];
                        }

                        /*
                        if (currentDistance < minDistance1 && clusterList[i].getNumPoints() == 1)
                        {
                            minDistance1 = currentDistance;
                            closest1 = clusterList[i];
                        }*/
                    }


                }

            }

            i++;
        }

        //if (minDistance1 < minDistance / 1.25f)
          //  closest = closest1;

        return closest;
    }

    protected float getDistance(MapPoint a, MapPoint b)
    {
        return (b.getX() - a.getX()) * (b.getX() - a.getX()) + (b.getY() - a.getY()) * (b.getY() - a.getY());
    }

    protected void arrangeRelatives(MapLevel level, MapLevel lastLevel)
    {
        List<GridCluster> levelClusterList = level.getGridClusters();

        foreach (GridCluster clusterChild in levelClusterList)
        {
            MapPoint childPoint = clusterChild.getPoint(0);

            //Debug.Log("Se llama desde cluster con h=" + clusterChild.hQuad + " y v=" + clusterChild.vQuad);

            int h = lastLevel.getHGrid(clusterChild.hQuad);
            int v = lastLevel.getVGrid(clusterChild.vQuad);

            //List<GridCluster> parentCandidates = lastLevel.getGridClustersPerQuad(h, v);

            GridCluster clusterParent = lastLevel.getClusterWithPoint(childPoint);
            //GridCluster clusterParent = lastLevel.getClusterWithPointInQuad(childPoint);

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
        //Debug.Log($"GetLevel {level}");

        if (level <= 0)
            level = 0;
        else
        {
            if (level >= this.zoomIntervals-1)
                level = this.zoomIntervals - 1;
            //else
              //  level = level + 1;
        }

        return level;
    }


    public void showInfo()
    {
        //Debug.Log("En este manager se gestiona la siguiente información............");
        //Debug.Log("Hay ....... " + points.Count + " puntos.");
        //Debug.Log("Hay ........" + levels.Count + " niveles.");
        //for (int i = 0; i < levels.Count; i++)
        //    levels[i].showInfo();
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
