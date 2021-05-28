using System;
using System.Collections;
using System.Collections.Generic;
using Honeti;
using UnityEngine;
using UnityEngine.UI;

public class RelationsLegendPopup : MonoBehaviour
{
    public GameObject prefabTitle;
    public GameObject rowPrefab;
    
    private List<Property> _properties;
    private List<GameObject> instantiatedGameObjects;
    private void Awake()
    {
        
        
        var propManager = SilkMap.instance.map.GetPropertyManager();
        _properties = propManager.GetRelatableProperties();
        instantiatedGameObjects = new List<GameObject>();
    }
    public void PopulateLegend()
    {

        var selectedMarker = MapUIManager.instance.GetSelectedMarker();
        var totalNumOfPoints = (SilkMap.instance.map.getNumPoints() * 1.0f);

        if (_properties.Count > 0)
        {
            instantiatedGameObjects.Add(Instantiate(prefabTitle, transform));
        }

        for (int i = 0; i < _properties.Count; i++)
        {
            var rel = _properties[i];
            
            var go = Instantiate(rowPrefab, this.transform);
            var image = go.GetComponentInChildren<Image>();
            image.color = _properties[i].GetRelationColor();
            var text = go.GetComponentInChildren<Text>();
            float percent = selectedMarker.getNumObjectsWithProperty(_properties[i].GetName()) / (totalNumOfPoints-1) * 100f;
            percent = Mathf.Max(percent, 0.0f);
            percent = Mathf.Min(percent, 100.0f);
            text.text = rel.GetName() + " " + percent.ToString("F1") + "%";
            instantiatedGameObjects.Add(go);
        }
        
      

    }

    public void ClearRows()
    {
        for (int i = instantiatedGameObjects.Count - 1; i >= 0; i--)
        {
            Destroy(instantiatedGameObjects[i]);
        }
        instantiatedGameObjects.Clear();
    }
    
}
