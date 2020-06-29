using System.Collections;
using System.Collections.Generic;
using Honeti;
using UnityEngine;
using UnityEngine.UI;

public class NormalPropertyItem : MonoBehaviour
{
    public Text propertyName;
    public Text propertyValue;

    public virtual void SetPropertyData(Property prop)
    {
        propertyName.text = I18N.instance.getValue("^"+prop.GetName()).ToUpperInvariant() ?? prop.GetName().ToUpperInvariant();//prop.GetName().ToUpperInvariant();
        
        var possibleValues = MapUIManager.instance.GetSelectedMarker().getPropertyValue(prop.GetName());
        if (possibleValues == null)
        {
            propertyValue.text = "NO DATA ";
            return;
        }
        var listOfValues= ""; 
        foreach (var value in possibleValues)
        {
            listOfValues += value + "; ";
        }
        propertyValue.text = listOfValues;
    }
}
