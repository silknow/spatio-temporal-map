/*         INFINITY CODE         */
/*   https://infinity-code.com   */

using UnityEngine;

namespace InfinityCode.OnlineMapsExamples
{
    /// <summary>
    /// Example of how to make the event hover on the marker.
    /// </summary>
    [AddComponentMenu("Infinity Code/Online Maps/Examples (API Usage)/OnHoverExample")]
    public class OnHoverExample : MonoBehaviour
    {
        private OnlineMapsMarkerBase hoverMarker;

        private void Start()
        {
            OnlineMaps map = OnlineMaps.instance;

            // Create a new marker
            OnlineMapsMarker marker = OnlineMapsMarkerManager.CreateItem(new Vector2(), "Marker");

            // Subscribe to marker events
            marker.OnRollOver += OnRollOver;
            marker.OnRollOut += OnRollOut;

            // Reset map position
            map.position = new Vector2();
        }

        private void OnRollOut(OnlineMapsMarkerBase marker)
        {
            // Remove a reference to marker
            hoverMarker = null;
        }

        private void OnRollOver(OnlineMapsMarkerBase marker)
        {
            // Make a reference to marker
            hoverMarker = marker;
        }

        private void Update()
        {
            // If a marker is present log marker label.
            if (hoverMarker != null) Debug.Log(hoverMarker.label);
        }
    }
}