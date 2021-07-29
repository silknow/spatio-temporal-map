using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Filter 
{
    protected string propertyName;
    protected List<string> values;
    public const bool AND_COMPARATION = false;
    public const bool OR_COMPARATION = true;

    public Filter(string propertyName, List<string>values)
    {
        this.propertyName = propertyName;
        this.values = values;
    }

    public void update(List<string> values)
    {
        this.values = this.values.Union(values).ToList<string>();
    }

    public List<string>getValues()
    {
        return this.values;
    }

    public void remove()
    {
        values.Clear();
    }


    public void RemoveValues(List<string> valuesToRemove )
    {
        this.values = this.values.Except(valuesToRemove).ToList<string>();
    }

    public bool checkFilterWithPoint(MapPoint point, bool comparation)
    {
        bool check = true;

        List<string> pointValues = point.getPropertyValue(propertyName);

        // If the point hasn't any value in the property --> must be filtered
        if (pointValues.Count == 0)
            return false;

        int numFails = 0;

        // In this lopp ---> 
        // Direct exit if fails and the comparation is AND
        // Direct exit if checks and the comparation is OR
        // Count the fails if the comparation is OR
        foreach (string value in values)
        {
            if (!pointValues.Contains(value))
            {
                if (comparation == AND_COMPARATION)
                    return false;
                else
                    numFails++;
            }
            else
                if (comparation == OR_COMPARATION)
                    return true;

        }

        // IF no direct exit in the last loop and there are fails --> no check
        if (numFails > 0)
            check = false;


        return check;
    }
}
