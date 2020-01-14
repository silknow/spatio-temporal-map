using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelationShip {

    MapPoint from;
    List<MapPoint> relatedWith = new List<MapPoint>();

    string name;
    string inverse;

    List<bool> visible = new List<bool>();

    bool multiple;

    public RelationShip(MapPoint from, MapPoint to, string name)
    {
        this.from = from;
        this.relatedWith.Add(to);
        this.visible.Add(true);
        this.multiple = false;
        this.name = name;
    }

    public bool isMultiple()
    {
        return this.multiple;
    }

    public void addPoint(MapPoint point)
    {
        this.relatedWith.Add(point);
        this.visible.Add(true);
        this.multiple = true;
    }

    public RelationShip(MapPoint from, List<MapPoint> pointList, string name)
    {
        this.from = from;
        this.relatedWith = pointList;
        this.name = name;
        this.multiple = true;
        for (int i = 0; i < pointList.Count; i++)
            this.visible.Add(true);
    }

    public MapPoint getFrom()
    {
        return this.from;
    }


    public MapPoint getTo()
    {
        return this.relatedWith[0];
    }

    public List<MapPoint> getRelatedWith()
    {
        return this.relatedWith;
    }
    
    public string getName()
    {
        return this.name;
    }

    public void setName(string name)
    {
        this.name = name;
    }

    public bool isVisible()
    {
        return this.visible[0];
    }


    public void setInverse(string inverse)
    {
        this.inverse = inverse;
    }

    public string getInverse() {
        return this.inverse;
    }

    private int getPositionOfPoint(MapPoint point)
    {
        int position = -1;

        bool found = false;
        int i = 0;

        while (!found && i < relatedWith.Count)
        {
            found = (relatedWith[i].getURI().Equals(point.getURI()));
            i++;
        }

        if (found)
            position = i - 1;

        return position;
    }

    public bool isVisibleRelationWith(MapPoint point)
    {
        bool isVisible = false;

        int position = getPositionOfPoint(point);


        if (position>=0)
            isVisible = this.visible[position];

        return isVisible;
    }

    public void hide()
    {
        for (int i = 0; i < relatedWith.Count; i++)
            visible[i] = false;
    }

    public void hideRelatationWithPoint(MapPoint point)
    {
        int position = getPositionOfPoint(point);

        if (position >= 0)
            this.visible[position] = false;
    }

    public void showRelatationWithPoint(MapPoint point)
    {
        int position = getPositionOfPoint(point);

        if (position >= 0)
            this.visible[position] = true;
    }


    public bool isRelatedTo(MapPoint point)
    {
        bool found = false;
        int i = 0;

        while (!found && i<relatedWith.Count)
        {
            found = relatedWith[i].getURI().Equals(point.getURI());
            i++;
        }

        return found;
    }
}
