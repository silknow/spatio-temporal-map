using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMarkerSpriteRenderer : MonoBehaviour
{
   private RectTransform _rectTransform;
   private MapPointMarker _mapPointMarker;
   private void Start()
   {
      OnlineMaps.instance.OnChangePosition += UpdateSpritePosition;
      OnlineMaps.instance.OnChangeZoom += UpdateSpritePosition;

      _rectTransform = GetComponent<RectTransform>();
      // Initial update line.
      UpdateSpritePosition();
     
   }

   public void InitRelationShipIndicator(MapPointMarker mapPointMarker)
   {
      _mapPointMarker = mapPointMarker;
      var relations = _mapPointMarker.getRelations();
   }
   private void UpdateSpritePosition()
   {
      if (OnlineMapsMarkerManager.instance.Count == 0)
      {
         gameObject.SetActive(false);
         return;
      }

      var firstMarker = SilkMap.instance.map.getPointPerUri("http://data.silknow.org/object/6497936a-8400-3747-a116-766bfad9b8a7") as MapPointMarker;
      if (firstMarker == null)
      {
         print("no encuentro marker");
         return;
      }
      var dim = firstMarker.getDimension();
      var isInMapview = dim == 2 ? firstMarker.getMarker2D().InMapView() : firstMarker.getMarker3D().InMapView(); 
      gameObject.SetActive( isInMapview);
      var position = OnlineMapsTileSetControl.instance.GetWorldPosition(firstMarker.getVector2());
      _rectTransform.localPosition = new Vector3(position.x, -position.z,0f);

   }
}
