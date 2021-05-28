using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeFrame 
{
    private int from;
    private int to;
    private List<MapPoint> filteredPoints = new List<MapPoint>();

    public TimeFrame(int from, int to)
    {
        this.from = from;
        this.to = to;
    }

    public int getFrom()
    {
        return from;
    }

    public void setFrom(int from)
    {
        this.from = from;
    }

    public int getTo()
    {
        return to;
    }

    public void setTo(int to)
    {
        this.to = to;
    }

    public bool checkInFrame(MapPoint point)
    {
        bool inFrame = false;
        if (point.getFrom() >= from)
        {
            if (point.getTo() <= to)
                inFrame = true;
            if (point.getFrom() <= to)
                inFrame = true;
        }else
        {
            if (point.getTo() >= from)
                inFrame = true;
        }
            //bool inFrame = point.getFrom()>= from && point.getTo()<=to;

        if (!inFrame) {
            if (!filteredPoints.Contains(point))
            {
                filteredPoints.Add(point);
                if (!point.isFiltered())
                    point.setFiltered(true);
            }
        }

        return inFrame;
    }

    public void Reset()
    {
        this.from = -100000;
        this.to = 100000;

        foreach (MapPoint p in filteredPoints)
            p.setFiltered(false);

        filteredPoints.Clear();
    }
}
