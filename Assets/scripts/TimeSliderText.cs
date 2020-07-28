using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeSliderText : MonoBehaviour
{
     [SerializeField]
        [Tooltip("The text shown will be formatted using this string.  {0} is replaced with the actual value")]
        private string formatText = "{0}";
        private Text _text;
       
}
