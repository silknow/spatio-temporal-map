using System.Collections;
using System.Collections.Generic;
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

    protected Color relationColor = Color.blue;

    protected bool relatable=false;
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
    }

    /**
     * Add the values of the propVals list (paremeter) to the values List of the property
     * If any value is registered is not entered again
     * */
    public void AddValue(List<string> propVals)
    {
        if (this.IsFilter())
            foreach (string s in propVals)
                if (!values.Contains(s))
                    values.Add(s);
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
}
