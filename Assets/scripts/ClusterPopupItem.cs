using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClusterPopupItem : MonoBehaviour
{
   private MapPointMarker _marker;

   public void SetMarker(string id)
   {
      _marker = SilkMap.instance.map.getPointPerUri(id) as MapPointMarker;
   }
   public void OnItemClick()
   {
      MapUIManager.instance.SetSelectedMarker(_marker);
      MapUIManager.instance.ShowDetailsPanel();
   }
}
