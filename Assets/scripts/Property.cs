using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/**
 * This class represents a property of the objects which a going to be displayed on the map
 * This property has been managed in the PropertyManager class, and the MapPoint object has
 * a dictionary <Property name key><Property value>
 * 
 * The property could be filtered if filter attribute is true, in this case, the values List attribute
 * has all the possible values to filter.
 * 
 * The property could be visualized in the information object scree if visible attribute is true.
 * */

public class Property 
{
    // Property with a list of values
    protected bool multipleValues;
    // The property value is a image link
    protected bool image = false;
    // The property value is a link
    protected bool link = false;
    // The property must be displayed in the filters windows
    protected bool filter = false;
    // The property must be displayed in the visualization windows
    protected bool visible = true;
    // The position in the filters window
    protected int filterPosition = 0;
    // The positiion in the visualization windows
    protected int visiblePosition = 0;
    // Possible value list (only in filter=true) instances
    protected List<string> values = new List<string>();
    // Name of the property
    protected string name;

    protected int[,,,] matUnions = new int[4, 4, 4, 4];

    protected Dictionary<string, List<MapPoint>> valuePoints = new Dictionary<string,List<MapPoint>>();

    protected Dictionary<string, List<MapPoint>> packedUnion = new Dictionary<string, List<MapPoint>>();

    protected Color relationColor = Color.blue;

    protected bool relatable=false;

    protected bool unionsCreated = false;

    public class PointComparer : IEqualityComparer<MapPoint>
    {
        public bool Equals(MapPoint p1, MapPoint p2)
        {
            return p1.getURI().Equals(p2.getURI()); 
        }

        public int GetHashCode(MapPoint obj)
        {
            return obj.getURI().GetHashCode();
        }
    }

    /**
     * Creates an instance of Property class, setting in the attributes of the class the values of parameters
     * */
    public Property(string name, bool visible, bool filter, int filterPosition, int visiblePosition, bool image, bool link)
    {
        this.SetName(name);
        this.SetVisible(visible);
        this.SetFilter(filter);
        this.SetFilterPosition(filterPosition);
        this.SetVisiblePosition(visiblePosition);
        this.SetImage(image);
        this.SetLink(link);
    }

    public void reset()
    {
        foreach (string s in valuePoints.Keys)
            valuePoints[s].Clear();

        valuePoints.Clear();

        foreach (string s in packedUnion.Keys)
            packedUnion[s].Clear();

        packedUnion.Clear();
    }

    
    public string GetName()
    {
        return this.name;
    }

    public void SetName(string name)
    {
        this.name = name;
    }

    public bool IsMultipleValues()
    {
        return multipleValues;
    }

    public void SetMultipleValues(bool multiple)
    {
        this.multipleValues = multiple;
    }

    public bool IsImage()
    {
        return image;
    }

    public void SetImage(bool isImage)
    {
        this.image = isImage;
    }

    public bool IsLink()
    {
        return link;
    }

    public void SetLink(bool isLink)
    {
        this.link = isLink;
    }

    public bool IsFilter()
    {
        return filter;
    }

    public void SetFilter(bool isFilter)
    {
        this.filter = isFilter;
    }

    public bool IsVisible()
    {
        return visible;
    }

    public void SetVisible(bool isVisible)
    {
        this.visible = isVisible;
    }

    public bool IsRelatable()
    {
        return relatable;
    }

    public void SetRelatable(bool isRelatable)
    {
        this.relatable = isRelatable;
    }

    public int GetFilterPosition()
    {
        return filterPosition;
    }

    public void SetFilterPosition(int filterPosition)
    {
        this.filterPosition = filterPosition;
    }

    public int GetVisiblePosition()
    {
        return visiblePosition;
    }

    public void SetVisiblePosition(int position)
    {
        this.visiblePosition = position;
    }

    public Color GetRelationColor()
    {
        return relationColor;
    }

    public void setRelationColor(Color color)
    {
        this.relationColor = color;
    }

    /**
     * Clear the list of possible values of the property instance
     * */
    public void clearValues()
    {
        this.values.Clear();
        this.reset();
    }

    /**
     * Add the values of the propVals list (paremeter) to the values List of the property
     * If any value is registered is not entered again
     * */
    public void AddValue(List<string> propVals, MapPoint point)
    {

            if (this.IsFilter() || this.IsRelatable())
                foreach (string s in propVals)
                {
                    if (!values.Contains(s))
                    {
                        values.Add(s);
                        valuePoints.Add(s, new List<MapPoint>());
                    }
                    if (this.IsRelatable())
                        valuePoints[s].Add(point);

                }
    }

    public int getPointsWithValues(List<string> values)
    {
        List<string> decodedValues = new List<string>();

        for (int n = 0; n < this.values.Count; n++)
            decodedValues.Add("");

        foreach (string s in values)        
            decodedValues[this.values.IndexOf(s)] = s;

        string packedValues = "";

        foreach (string s in decodedValues)
            if (s.Length > 0)
                packedValues += s;

        if (packedUnion.ContainsKey(packedValues))
            return packedUnion[packedValues].Count-1;
        else
            return package(decodedValues);

    }


    protected int package(List <string> decodedValues)
    {       
        string currentPacked = "";
        bool first = true;

        List<MapPoint> auxUnion = new List<MapPoint>();

        foreach (string s in decodedValues)
        {
            if (s.Length > 0)
            {
                currentPacked += s;
                if (first)
                {
                    auxUnion.AddRange(valuePoints[s]);
                    first = false;
                }
                else
                {
                    if (packedUnion.ContainsKey(currentPacked))
                        auxUnion.AddRange(packedUnion[currentPacked]);
                    else
                    {
                        auxUnion = auxUnion.Union(valuePoints[s], new PointComparer()).ToList();
                        packedUnion.Add(currentPacked, auxUnion);
                    }
                }
            }
        }

        return auxUnion.Count-1;
    }

    public int getPointsWithValues3(List<string> values)
    {

        int[] valueCoords = { 0, 0, 0, 0 };

        if (!unionsCreated)
            updateMatUnions();

        if (values.Count < 4)
        {
            for (int i = 0; i < values.Count; i++)
            {
                int position = this.values.IndexOf(values[i]); // El problema que hay es que el punto tiene 4 valores, pero hay 10 posibles valores en la propiedad, no puede ser this.values.IndexOf (sino será un número mayor que 4 facilmente)

                valueCoords[position] = 1;
            }

            return matUnions[valueCoords[0], valueCoords[1], valueCoords[2], valueCoords[3]];
        }
        else
            return getPointsWithValues2(values);
    }

    public int getPointsWithValues1(List<string> values)
    {

        int maxNum = 1;

        foreach (string s in values)
        {
            int aux = valuePoints[s].Count;
            if (aux > maxNum)
                maxNum = aux;
        }
            
        return maxNum-1;
    }

    public int getPointsWithValues2(List<string> values)
    {

        List<MapPoint> union = new List<MapPoint>();

        foreach (string s in values)
        {
            int aux = valuePoints[s].Count;

            if (union.Count == 0)
                union.AddRange(valuePoints[s]);
            else
                union = union.Union(valuePoints[s], new PointComparer()).ToList();
        }

        return union.Count;
    }

    /**
     * Return all the possible values of the property
     * */
    public List<string> getPossibleValues()
    {
        if (this.IsFilter())
            return this.values;
        else
            return null;
    }

    protected void updateMatUnions()
    {
        resetMatUnions();

        if (values.Count > 0)
        {
            if (values.Count>=1)            
                matUnions[1, 0, 0, 0] = valuePoints[values[0]].Count;

            if (values.Count >= 2)
            {
                matUnions[0, 1, 0, 0] = valuePoints[values[1]].Count;
                matUnions[1, 1, 0, 0] = doUnion(values[0], values[1]).Count;
            }

            if (values.Count>=3)
            {
                matUnions[0, 0, 1, 0] = valuePoints[values[2]].Count;
                matUnions[1, 0, 1, 0] = doUnion(values[0], values[2]).Count;
                matUnions[0, 1, 1, 0] = doUnion(values[1], values[2]).Count;
                matUnions[1, 1, 1, 0] = doUnion(values[0], values[1], values[2]).Count;
            }

            if (values.Count>=4)
            {
                matUnions[0, 0, 0, 1] = valuePoints[values[3]].Count;

                matUnions[1, 0, 0, 1] = doUnion(values[0], values[3]).Count;
                matUnions[0, 1, 0, 1] = doUnion(values[1], values[3]).Count;
                matUnions[0, 0, 1, 1] = doUnion(values[2], values[3]).Count;
                matUnions[1, 1, 0, 1] = doUnion(values[0], values[1], values[3]).Count;
                matUnions[1, 0, 1, 1] = doUnion(values[0], values[2], values[3]).Count;

                matUnions[1, 1, 1, 1] = doUnion(values[0], values[1], values[2], values[3]).Count;
            }
        }
        
    }

    protected List<MapPoint> doUnion(string valueA, string valueB)
    {
        return valuePoints[valueA].Union(valuePoints[valueB], new PointComparer()).ToList();
    }

    protected List<MapPoint> doUnion(string valueA, string valueB, string valueC)
    {
        List<MapPoint> aux = doUnion(valueA, valueB);

        return valuePoints[valueC].Union(aux, new PointComparer()).ToList();
    }

    protected List<MapPoint> doUnion(string valueA, string valueB, string valueC, string valueD)
    {
        List<MapPoint> aux = doUnion(valueA, valueB, valueC);

        return valuePoints[valueD].Union(aux, new PointComparer()).ToList();
    }

    protected void resetMatUnions()
    {
        int a, b, c, d;

        for (a = 0; a <= 3; a++)
            for (b = 0; b <= 3; b++)
                for (c = 0; c <= 3; c++)
                    for (d = 0; d <= 3; d++)
                        matUnions[a, b, c, d] = 0;
    }
}
