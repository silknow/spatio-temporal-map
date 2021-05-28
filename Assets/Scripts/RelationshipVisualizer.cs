using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class RelationshipVisualizer : Singleton<RelationshipVisualizer>
{
    public GameObject relationshipItemVisualizer;
    private bool _init = false;
    private RectTransform _rectTransform;
    public bool VisualizingRelations
    {
        get => GetComponent<Canvas>().enabled;
    }

    public void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private List<RelationshipItemVisualizer> items;
    public void OnDataLoaded()
    {
        OnlineMaps.instance.OnChangePosition -= UpdateItemsPositions;
        var startTime = Stopwatch.StartNew();
        Clear();
        items = new List<RelationshipItemVisualizer>();
        if(relationshipItemVisualizer== null)
            return;
        var mps = SilkMap.instance.map.GetPoints();
        var mpsCount = mps.Count;
        for (var index = 0; index < mpsCount; index++)
        {
            var mp = mps[index];
            var mapPointMarker = mp as MapPointMarker;
            if (mapPointMarker != null && mapPointMarker.isGroupPoint())
                continue;
            var item = Instantiate(relationshipItemVisualizer, _rectTransform)
                .GetComponent<RelationshipItemVisualizer>();
            item.mapPointMarker = mapPointMarker;
            item.parentVisualizer = this;
            items.Add(item);
        }

        Debug.Log($"Numero de anillos creados { items.Count}");
        Debug.Log($"Crear Relaciones Anillos: {startTime.ElapsedMilliseconds * 0.001f} segundos");
        InitRelationsVisualizer();
        OnlineMaps.instance.OnChangePosition += UpdateItemsPositions;
       
       
    }
    private void InitRelationsVisualizer()
    {
        var ringsCount = items.Count;
        for (int i = 0; i < ringsCount; i++)
        {
            var item = items[i];
           
            item.gameObject.SetActive(VisualizingRelations);
        }
       
      
    }


    private void UpdateItemsPositions()
    {
      
        if(!VisualizingRelations || items == null)
            return;
        var ringsCount = items.Count;
        for (int i = 0; i < ringsCount; i++)
        {
            var item = items[i];
            var dim  = item.mapPointMarker.getDimension();
            var marker = dim == 2 ? item.mapPointMarker.getMarker2D() as OnlineMapsMarkerBase : item.mapPointMarker.getMarker3D() as OnlineMapsMarkerBase;
            var active = marker.InMapView()  && marker.enabled;
            item.gameObject.SetActive(active);
            if(active)
                item.UpdateSpritePosition();
            //marker.DestroyInstance();

        }
       
      
    }

    public void OnChangeCanvas(bool active)
    {
        if(active)
            UpdateItemsPositions();
    }

    private void Clear()
    {
        foreach (var rel in GetComponentsInChildren<RelationshipItemVisualizer>(true))
        {
            Destroy(rel.gameObject);
        }
    }

    public Dictionary<Property, float> GetRelationshipsPercentage()
    {
        var child = transform.GetChild(0);
        if (child == null)
            return null;
        return child.GetComponent<RelationshipItemVisualizer>() == null ? null : child.GetComponent<RelationshipItemVisualizer>().propertyPercentage;
    }
}
