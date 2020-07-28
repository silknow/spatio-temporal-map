using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMarkerSpriteRenderer : MonoBehaviour
{
   [SerializeField] private SpriteRenderer _sprite;
   private void Start()
   {
      OnlineMaps.instance.OnChangePosition += UpdateSpritePosition;
      OnlineMaps.instance.OnChangeZoom += UpdateSpritePosition;

      // Initial update line.
      UpdateSpritePosition();
   }

   private void Update()
   {
      // If size changed, then update line.
      //if (Mathf.Abs(size - _size) > float.Epsilon) UpdateSpritePosition();
   }

   private void UpdateSpritePosition()
   {
      if (OnlineMapsMarkerManager.instance.Count == 0)
      {
         _sprite.gameObject.SetActive(false);
         return;
      }

      var firstMarker = OnlineMapsMarkerManager.instance.items[0];
      _sprite.gameObject.SetActive( firstMarker.inMapView);
      var position = OnlineMapsTileSetControl.instance.GetWorldPosition(firstMarker.position);

      position += new Vector3(0,0.15f,0);
      _sprite.transform.position = position;
      
   }
}
