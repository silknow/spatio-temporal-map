using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelationshipItemVisualizer : MonoBehaviour
{
    public List<Color> propertyColors;
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
        if (_propertyCount > propertyColors.Count)
        {
            print("FALTAN COLORES");
            return;
        }

        FillCircle();

    }
    private void OnDestroy()
    { 
        OnlineMaps.instance.OnUpdateLate -= UpdateSpritePosition;
    }


    private void FillCircle()
    {
        var sizeOfProperty = 1.0f / _propertyCount;
        float offset = 0f;
        var totalNumOfPoints = (SilkMap.instance.map.getNumPoints() * 1.0f);
        for (int i = 0; i < _propertyCount; i++)
        {
           var rel = Instantiate(relationshipArcPrefab, this.GetComponent<RectTransform>()).GetComponent<RelationshipArcProperty>();
           rel.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0,0,offset * 360f);
           rel.baseColor = propertyColors[i];
           rel.totalSizeOfProperty = sizeOfProperty;
           var percent = mapPointMarker.getNumObjectsWithProperty(_properties[i].GetName()) / totalNumOfPoints;
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
