using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelationshipItemVisualizer : MonoBehaviour
{
    
    private int _propertyCount =3;
    private List<Property> _properties;
    public GameObject relationshipArcPrefab;
    private RectTransform _rectTransform;
    public Dictionary<Property, float> propertyPercentage;

    [HideInInspector] public MapPointMarker mapPointMarker;

    private void Awake()
    { 
       OnlineMaps.instance.OnUpdateLate += UpdateSpritePosition;
    }

    void Start()
    {
        propertyPercentage = new Dictionary<Property, float>();

        var propManager = SilkMap.instance.map.GetPropertyManager();
        _properties = propManager.GetRelatableProperties();
        _propertyCount = _properties.Count;
        OnlineMaps.instance.OnUpdateLate += UpdateSpritePosition;
        _rectTransform = GetComponent<RectTransform>();
        FillCircle();

    }
    // private void OnDestroy()
    // {
    //     if (OnlineMaps.instance.OnUpdateLate != null) OnlineMaps.instance.OnUpdateLate -= UpdateSpritePosition;
    // }


    private void FillCircle()
    {
        var sizeOfProperty = 1.0f / _propertyCount;
        float offset = 0f;
        //Exclude itself from total num
        var totalNumOfPoints = Mathf.Max((SilkMap.instance.map.getNumPoints()-1),0);
        for (int i = 0; i < _propertyCount; i++)
        {
           var rel = Instantiate(relationshipArcPrefab, this.GetComponent<RectTransform>()).GetComponent<RelationshipArcProperty>();
           rel.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0,0,offset * 360f);
           rel.baseColor = _properties[i].GetRelationColor();
           rel.totalSizeOfProperty = sizeOfProperty;

           var percent = totalNumOfPoints == 0 ? 0 : (mapPointMarker.getNumObjectsWithProperty(_properties[i].GetName()) / (float)totalNumOfPoints);
           percent = Mathf.Max(percent, 0.0f);
           propertyPercentage.Add(_properties[i],percent);
           rel.sizeOfRelation = sizeOfProperty * percent; 
           offset += sizeOfProperty;
        }
    }
    private void UpdateSpritePosition()
    {
        if(this== null)
            return;
        var dim = mapPointMarker.getDimension();
        var isInMapview = dim == 2 ? mapPointMarker.getMarker2D().InMapView() : mapPointMarker.getMarker3D().InMapView(); 
        var isEnabled = dim == 2 ? mapPointMarker.getMarker2D().enabled : mapPointMarker.getMarker3D().enabled; 
        gameObject.SetActive( isInMapview  && isEnabled);
        
        var position = OnlineMapsTileSetControl.instance.GetWorldPosition(mapPointMarker.getVector2());
        _rectTransform.localPosition = new Vector3(position.x, -position.z,0f);

        if (dim == 3)
        {
            _rectTransform.pivot = new Vector2(0.5f,0.45f);  
        }
        else
        {
            _rectTransform.pivot = new Vector2(0.5f,0.75f); 
        }

    }
}
