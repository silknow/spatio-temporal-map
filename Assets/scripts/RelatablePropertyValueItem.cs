using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RelatablePropertyValueItem : MonoBehaviour
{
   public Text valueText;
   public Toggle showRelationsToggle;
   private string _value;
   private Property _property;
   public void SetProperty(Property prop)
   {
      _property = prop;
      showRelationsToggle.onValueChanged.RemoveAllListeners();
      showRelationsToggle.onValueChanged.AddListener(delegate {
         ToggleValueChanged(showRelationsToggle,prop.GetName(),_value);
      });
   }
   

   public void setValue(string value)
   {
      _value = value;
      valueText.text = _value;
   }
   void ToggleValueChanged(Toggle change, string property, string value)
   {
      if (change.isOn)
      {
         //Show relations with this value
         MapUIManager.instance.GetSelectedMarker().showRelations(property);
         Debug.LogFormat("Show relations for property {0}",property);
      }
      else
      {
         //Hide relations with this value
         MapUIManager.instance.GetSelectedMarker().hideRelations(property);
      }
      
        
   }
}
