using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

public class MapPoint 
{
    public const int TWO_DIMENSION = 2;
    public const int THREE_DIMENSION = 3;
    public static int lastId = 0;

    public Vector3 data;

    protected float distance = -1.0f;

    protected int dimension = TWO_DIMENSION;

    // x,y,z position in map coordinates
    protected float x;
    protected float y;
    protected float z;

    // Dsisplay or not display
    protected bool hidden = false;

    // Associated uri to the point (if exists)
    protected string uri = "";

    // Label of the point (if exists)
    protected string label = "";

    // Class/Category associated to the point
    protected string category = "";

    // The point exists in time from [from, to] years
    protected int from;
    protected int to;

    protected int id;

    // If filtered is active, the point must no be displayed
    protected bool filtered=false;

    // MapPoints related with this point
    protected List<RelationShip> relations = new List<RelationShip>();

    // Clusters where this point is related to
    protected List<GridCluster> clusterList = new List<GridCluster>();
    protected List<MapPoint> clusteredPointList;
    
    protected Dictionary<string, List<string>> propertyValues = new Dictionary<string, List<string>>();

    protected Dictionary<string, RelationShip> relationsPerProperty = new Dictionary<string, RelationShip>();

    // If the map point is a cluster or a data point.
    protected bool cluster = false;

    // cluster represented by this point
    GridCluster gridCluster;

    protected Map map;

   



    public string Model { get; set; }


    public MapPoint(float x, float y)
    {
        this.x = x;
        this.y = y;
        this.id = lastId;
        lastId++;
    }

    public int getId()
    {
        return id;
    }

    public void setMap(Map map)
    {
        this.map = map;
    }

    /**
     * returns true is same values of propertyName are in the values List
     * */
    public bool sameValuesAtProperty(string propertyName, List<string> values)
    {
        bool sameValue = false;

        List<string> valuesOfPoint = getPropertyValue(propertyName);

        if (valuesOfPoint.Any(x => values.Any(y => y.Equals(x))))
            sameValue = true;

        return sameValue;
    }

    public void SetPropertyValue(string propertyName, List<string> values)
    {
        
        if (!propertyValues.ContainsKey(propertyName))
            propertyValues.Add(propertyName, values);
        else
            propertyValues[propertyName] = values;
    }

    public List<string> getPropertyValue(string name)
    {
        if (propertyValues.ContainsKey(name))
            return propertyValues[name];
        else
            return new List<string>();
    }

    public void reset()
    {
        relations.Clear();
        clusterList.Clear();

        graphicReset();
        //relations.RemoveRange(0, relations.Count);
        //clusterList.RemoveRange(0, clusterList.Count);
    }

    public virtual void graphicReset()
    {

    }

    public Vector2 getVector2()
    {
        return new Vector2(this.x, this.y);
    }


    public int getDimension()
    {
        return dimension;
    } 


    public void setDimension(int dimension)
    {
        if (this.dimension!=dimension)
        {
            this.dimension = dimension;
            if (!hidden)
            {
                hide();
                show();
            }
        }
    }
    
    public string getLabel()
    {
        return label;
    }

    public void setLabel(string label)
    {
        this.label = label;
        updateGraphicLabel();
    }

    public string getCategory()
    {
        return this.category;
    }

    public void setCategory(string category)
    {
        this.category = category;
    }
 
    public string getURI()
    {
        return uri;
    }

    public void setURI(string URI)
    {
        this.uri = URI;
    }

    public List<MapPoint> getClusteredPoints()
    {
        if (isCluster())
        {
            return this.clusteredPointList;
        }
        else
            return null;
    }

    public void setClusteredPoints(List <MapPoint> clusteredPoints)
    {
        this.clusteredPointList = clusteredPoints;
    }

    /**
     * Function that returns if this point has a relation named <name> with the point <point>
     * @return true if the relation exist, false otherwise
     * */
    public bool hasRelationShipWith(MapPoint point, string name)
    {
        bool existsRelationship = false;
        int i = 0;
     
        while (!existsRelationship && i<relations.Count)
        {
            existsRelationship = (relations[i].isRelatedTo(point) && relations[i].getName().Equals(name));
            i++;
        }

        return existsRelationship;

    }    

    /**
     * Function that adds a <inverse> RelationShip of this point with another <point>
     * @returns the added Relationship instance
     * */
    public RelationShip addInverseRelationWith(MapPoint point, string inverse)
    {
        RelationShip relation = new RelationShip(this, point, "");
        relation.setInverse(inverse);       
        this.relations.Add(relation);

        return relation;
    }

    /**
     * Function that add a RelationShip named <name> fromt this point with <point>
     * @returns the added RelationShip instance
     * */
    public RelationShip addRelationWith(MapPoint point, string name)
    {
        RelationShip relation = null;

        if (!this.hasRelationShipWith(point,name))
        {
            relation = new RelationShip(this, point, name);
            this.relations.Add(relation);
            RelationShip inverseRelation = point.addInverseRelationWith(this, name);
            inverseRelation.hide();
        }

        return relation;
    }

    public List<RelationShip> getRelations()
    {
        return this.relations;
    }

    public void addCluster(GridCluster cluster)
    {
        this.clusterList.Add(cluster);
    }

    public List<GridCluster> getRelatedClusters()
    {
        return this.clusterList;
    }

    public void setXY(float x, float y)
    {
        this.x = x;
        this.y = y;

        updateGraphicsCoordinates();
    }

    public float getX()
    {
        return x;
    }

    public float getY()
    {
        return y;
    }

    public void show()
    {
        //if (getLabel().Equals("8824"))
         //   Debug.Log("localizado " + isFiltered());

        if (filtered)
        {
            hide();
            return;
        }

  
        if (hidden)
        {
            hidden = false;
            graphicShow();
        }
    }


    public void showAllRelations()
    {
        foreach (string property in relationsPerProperty.Keys)
            showRelations(property);
    }

    public void hideAllRelations()
    {
        foreach (string property in relationsPerProperty.Keys)       
            hideRelations(property);
    }

    public void removeAllRelations()
    {
        foreach (string property in relationsPerProperty.Keys)
            removeRelations(property);
    }

    public void removeRelations(string propertyName)
    {
        RelationShip relation;

        if (relationsPerProperty.ContainsKey(propertyName))
        {
            relation = this.relationsPerProperty[propertyName];
            relation.clear();
            relationsPerProperty.Remove(propertyName);

            removeGraphicRelations(propertyName);
        }

        if (relationsPerProperty.Keys.Count == 0)
            map.removePointWithRelation(this);
    }

    public void hideRelations(string propertyName)
    {
        RelationShip relation;

        if (relationsPerProperty.ContainsKey(propertyName))
        {
            relation = this.relationsPerProperty[propertyName];
            relation.hide();

            //relation.clear();
            //relationsPerProperty.Remove(propertyName);

            updateGraphicRelations(propertyName,false);
        }

        
        //if (relationsPerProperty.Keys.Count == 0)
        //    map.removePointWithRelation(this);
    }

    public bool propertyRelationsShown(string propertyName)
    {
        return relationsPerProperty.ContainsKey(propertyName);
    }

    public void showRelations(string propertyName)
    {
        RelationShip relation;

        //Debug.Log("Show relations for property " + propertyName);
        
        // Check if relations are previously processed
        if (!relationsPerProperty.ContainsKey(propertyName))
        {
            // Get the points with the same values in propertyName
            List<MapPoint> relatedPoints = map.poinstWithValuesAtProperty(propertyName, getPropertyValue(propertyName));

            relatedPoints.Remove(this);

            relation = new RelationShip(this, relatedPoints, propertyName);

            relationsPerProperty.Add(propertyName, relation);
        }

        relation = this.relationsPerProperty[propertyName];
        relation.update();

        updateGraphicRelations(propertyName,true);

        map.addPointWithRelation(this);
    }

    public void hide()
    {
        //if (getLabel().Equals("8824"))
          //  Debug.Log("localizado " + isFiltered());

        if (!hidden)
        {
            hidden = true;
            graphicHide();      
        }
    }

    public void setFiltered(bool filtered)
    {
        this.filtered = filtered;

        if (this.filtered)
            foreach (GridCluster gCluster in this.clusterList)
                gCluster.addFilteredPoint();
        else
            foreach (GridCluster gCluster in this.clusterList)
                gCluster.removeFilteredPoint();

    }

    public bool isFiltered()
    {
        return filtered;
    }


    protected virtual void graphicHide()
    {

    }

    protected virtual void graphicShow()
    {

    }

    protected virtual void updateGraphicLabel()
    {

    }

    protected virtual void updateGraphicsCoordinates()
    {

    }

    protected virtual void updateGraphicRelations(string propertyName, bool show)
    {
    }


    public virtual void removeGraphicRelations(string propertyName)
    {
    }



        public void setFrom(int from)
    {
        this.from = from;
    }

    public int getFrom()
    {
        return this.from;
    }

    public void setTo(int to)
    {
        this.to = to;
    }

    public int getTo()
    {
        return this.to;
    }

    public bool isCluster()
    {
        return this.cluster;
    }

    public void setCluster(bool cluster)
    {
        this.cluster = cluster;
    }

    public void setGridCluster(GridCluster gCluster)
    {
        this.gridCluster = gCluster;
        setCluster(true);
    }

    public GridCluster getGridCluster()
    {
        return this.gridCluster;
    }
}
