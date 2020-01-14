/*     INFINITY CODE 2013-2019      */
/*   http://www.infinity-code.com   */

using UnityEngine;

namespace InfinityCode.OnlineMapsExamples
{
    /// <summary>
    /// Example of how to create a marker on click.
    /// </summary>
    [AddComponentMenu("Infinity Code/Online Maps/Examples (API Usage)/CreateMarkerOnClick")]
    public class CreateMarkerOnClick:MonoBehaviour
    {
        private void Start()
        {
            // Subscribe to the click event.
            OnlineMapsControlBase.instance.OnMapClick += OnMapClick;
        }

        private void OnMapClick()
        {
            // Get the coordinates under the cursor.
            double lng, lat;
            OnlineMapsControlBase.instance.GetCoords(out lng, out lat);

            // Create a label for the marker.
            string label = "Marker " + (OnlineMapsMarkerManager.CountItems + 1);

            // Create a new marker.
            OnlineMapsMarkerManager.CreateItem(lng, lat, label);
        }
    }
}
