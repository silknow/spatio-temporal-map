/*     INFINITY CODE 2013-2019      */
/*   http://www.infinity-code.com   */

using UnityEngine;

namespace InfinityCode.OnlineMapsExamples
{
    /// <summary>
    /// Example of how to drag the markers by long press. It is convenient for mobile devices.
    /// </summary>
    [AddComponentMenu("Infinity Code/Online Maps/Examples (API Usage)/DragMarkerByLongPressExample")]
    public class DragMarkerByLongPressExample : MonoBehaviour
    {
        private void Start()
        {
            // Create a new marker.
            OnlineMapsMarker marker = OnlineMapsMarkerManager.CreateItem(OnlineMaps.instance.position, "My Marker");

            // Subscribe to OnLongPress event.
            marker.OnLongPress += OnMarkerLongPress;
        }

        private void OnMarkerLongPress(OnlineMapsMarkerBase marker)
        {
            // Starts moving the marker.
            OnlineMapsControlBase.instance.dragMarker = marker;
            OnlineMapsControlBase.instance.isMapDrag = false;
        }
    }
}