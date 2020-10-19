using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelationshipVisualizer : Singleton<RelationshipVisualizer>
{
    public GameObject relationshipItemVisualizer;

    public void OnDataLoaded()
    {
        Clear();
        if(relationshipItemVisualizer== null)
            return;
        foreach (var mp in SilkMap.instance.map.GetPoints())
        {
            var mapPointMarker = mp as MapPointMarker;
            var item = Instantiate(relationshipItemVisualizer, this.GetComponent<RectTransform>()).GetComponent<RelationshipItemVisualizer>();
            item.mapPointMarker = mapPointMarker;
        }
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
    public List<Color> GetColors()
    {
        var child = transform.GetChild(0);
        if (child == null)
            return null;
        return child.GetComponent<RelationshipItemVisualizer>() == null ? null : child.GetComponent<RelationshipItemVisualizer>().propertyColors;

    }
}
