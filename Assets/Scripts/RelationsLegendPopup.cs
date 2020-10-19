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

    private List<Color> colors;
    private List<Property> _properties;
    private List<GameObject> instantiatedGameObjects;
    private void Awake()
    {
        
        colors = RelationshipVisualizer.instance.GetColors();
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
            image.color = colors[i];
            var text = go.GetComponentInChildren<Text>();
            int percent = (int)(selectedMarker.getNumObjectsWithProperty(_properties[i].GetName()) / totalNumOfPoints * 100);
            text.text = rel.GetName() + " " + percent + "%";
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
