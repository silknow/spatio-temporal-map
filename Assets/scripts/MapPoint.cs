using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPoint 
{

    public Vector3 data;

    protected float distance = -1.0f;


    // x,y,z position in map coordinates
    protected float x;
    protected float y;
    protected float z;

    // Dsisplay or not display
    protected bool hidden = false;

    // Associated uri to the point (if exists)
    protected string uri = "";

    // Class/Category associated to the point
    protected string category = "";

    // The point exists in time from [from, to] years
    protected int from;
    protected int to;

    // MapPoints related with this point
    protected List<RelationShip> relations = new List<RelationShip>();

    // Clusters where this point is related to
    protected List<GridCluster> clusterList = new List<GridCluster>(); 


    public string Model { get; set; }


    public MapPoint(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    public void reset()
    {
        relations.RemoveRange(0, relations.Count);
        clusterList.RemoveRange(0, clusterList.Count);
    }

    public Vector2 getVector2()
    {
        return new Vector2(this.x, this.y);
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
        if (hidden)
        {
            hidden = false;
            graphicShow();
        }
    }

    public void hide()
    {
        if (!hidden)
        {
            hidden = true;
            graphicHide();      
        }
    }

    protected virtual void graphicHide()
    {

    }

    protected virtual void graphicShow()
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

  
}
