using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * PropertyManager class will manage of the properties related with the objects displayed on the map
 * 
 * Once the manager is instantited, the first action is to enter the definition of the properties, through the AddProperty method.
 * 
 * The manager instance will be used to assign property values to each displayed MapPoint through the method setPropertyValue
 * This method assign the property value to the MapPoint instance and also manage general issues, like the possible values of 
 * a property.
 * 
 * The manager could be used to get the name of all the filtered properties and their values.
 * The manager could be used to get the all the possible values of the filtered properties.
 * 
 * The manager could be used to get the name of all the properties which will be displayed on the object information screen.
 * */
public class PropertyManager : MonoBehaviour
{
    List<Property> propertyList = new List<Property>();

    public const string PROPERTY_EXISTS = "The property is registered in the manager : ";
    public const string PROPERTY_NOT_EXISTS = "The property is not registered in the manager : ";

    /**
     * Add the specification of a new property in the property manager.
     * Once this specification is added, the property could be used to assign values to MapPoint instances.
     * */
    public Property AddProperty(string name,bool visible, bool filter, int positionFilter,int visiblePosition, bool image, bool link, bool relatable)
    {

        Property property = this.GetPropertyByName(name);

        if (property != null)
            Debug.LogError(PROPERTY_EXISTS + name);
        else
        {
            property = new Property(name, visible, filter, positionFilter, visiblePosition, image, link);
            property.SetRelatable(relatable);
            propertyList.Add(property);
        }

        return property;
    }

    /**
     * Set the value <value> to the property <name> of the <point>
     * */
    public void SetPropertyValue(string name,  MapPoint point, string value)
    {
        List<string> valueList = new List<string>();
        valueList.Add(value);
        SetPropertyValue(name, point, valueList);   
    }

    /**
     * Clear the possible values of the managed properties
     * */
    public void resetPropertyValues()
    {
        foreach (Property p in this.propertyList)
            p.clearValues();
    }

    /** 
     * Sets the values of <valueList> to hthe property <name> of the <point>
     * */
    public void SetPropertyValue(string name, MapPoint point, List<string> valueList)
    {
        if (valueList == null) {
            //Debug.Log("El valor de la instancia es NULL");
            return;
        }

        Property property = GetPropertyByName(name);

        if (property == null)
            return;

        if (valueList.Count > 1)
            property.SetMultipleValues(true);

        if (property != null)
        {
            if (property.IsFilter())
                property.AddValue(valueList);

            point.SetPropertyValue(name, valueList);
        }
        else
            Debug.LogError(PROPERTY_NOT_EXISTS + name);


    }

    /**
     * Returns a list with all the property instances which could be used to filter the objects on the map
     * */
    public List<Property> GetFilteredProperties() {

        List<Property> filteredList = new List<Property>();

        foreach (Property prop in propertyList)
            if (prop.IsFilter())
                filteredList.Add(prop);

        return filteredList;
    }

    /**
     * Returns a list with the names of the properties which could be used to filter the objects on the map
     * */
    public List<string> GetFilteredPropertiesName()
    {
        List<string> filteredList = new List<string>();
        Debug.Log("Hay " + propertyList.Count + " propiedades");

        foreach (Property prop in propertyList)
            if (prop.IsFilter())
                filteredList.Add(prop.GetName());

        return filteredList;
    }

    /**
     * Gets the property instance with the name <name>
     * */
    public Property GetPropertyByName(string name)
    {
        foreach (Property p in propertyList)        
            if (p.GetName().Equals(name))
                return p;

        //Debug.Log(PROPERTY_NOT_EXISTS + name);

        return null;
    }

    /**
     * Returns a list with the properties used to display information in the object information window
     * */
    public List<Property> GetVisibleProperties()
    {
        List<Property> visiblePropList = new List<Property>();

        foreach (Property prop in propertyList)
            if (prop.IsVisible())
                visiblePropList.Add(prop);

        return visiblePropList;
    }

    /**
     * Returns a list with the names of the properties used to display information in the object information window
     * */
    public List<string> GetVisiblePropertiesName()
    {
        List<string> visiblePropList = new List<string>();

        foreach (Property prop in propertyList)
            if (prop.IsVisible())
                visiblePropList.Add(prop.GetName());

        return visiblePropList;
    }

    /**
 * Returns a list with the names of the properties which user can ask for relationships in the object information window
 * */
    public List<string> GetRelatablePropertiesName()
    {
        List<string> relatablePropList = new List<string>();

        foreach (Property prop in propertyList)
            if (prop.IsRelatable())
                relatablePropList.Add(prop.GetName());

        return relatablePropList;
    }

    /**
 * Returns a list with the properties used to ask for relationships in the object information window
 * */
    public List<Property> GetRelatableProperties()
    {
        List<Property> relatablePropList = new List<Property>();

        foreach (Property prop in propertyList)
            if (prop.IsRelatable())
                relatablePropList.Add(prop);

        return relatablePropList;
    }
}
