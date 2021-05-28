using System;
using System.Collections;
using System.Collections.Generic;
using Honeti;
using UnityEngine;
using UnityEngine.UI;

public class FilteredPropertyRow : MonoBehaviour
{
    public Property property;
    public Text propertyName;

    public GameObject propertyToggle;

    public Transform toggleParent;
    // Start is called before the first frame update
    void Start()
    {
        propertyName.text = I18N.instance.getValue("^"+property.GetName()) ?? property.GetName();
        foreach (var value in property.getPossibleValues())
        {
            var toggleGameObject = Instantiate(propertyToggle, toggleParent);
            toggleGameObject.GetComponentInChildren<Text>().text = value;
            var toggle = toggleGameObject.GetComponent<Toggle>();
            toggle.onValueChanged.AddListener(delegate {
                ToggleValueChanged(toggle,property.GetName(),value);
            });

        }
    }

    private void OnEnable()
    {
        var s = "^"+property.GetName();
        propertyName.text = s;
    }

    void ToggleValueChanged(Toggle change, string property, string value)
    { 

        if (change.isOn)
        {
            //Filter property with FilterManager
            SilkMap.instance.map.addFilter(property, value);
            AnalyticsMonitor.instance.sendEvent("Apply_Filter", new Dictionary<string, object>
            {
                {"property", property},
                {"value", value}
            });
           
        }
        else
        {
            //Remove FilterValue;
            SilkMap.instance.map.removeFilter(property,value);
            AnalyticsMonitor.instance.sendEvent("Remove_Filter", new Dictionary<string, object>
            {
                {"property", property},
                {"value", value}
            });
        }

        //Mover a botón de Apply Filters
        SilkMap.instance.map.applyFilters();
        
    }
}
