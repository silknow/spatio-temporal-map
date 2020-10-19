/*         INFINITY CODE         */
/*   https://infinity-code.com   */

using UnityEngine;

namespace InfinityCode.OnlineMapsExamples
{
    /// <summary>
    /// Example of how to dynamically create a 2D marker in the GPS locations of user.
    /// </summary>
    [AddComponentMenu("Infinity Code/Online Maps/Examples (API Usage)/Marker_GPS_Example")]
    public class Marker_GPS_Example : MonoBehaviour
    {
        // Marker, which should display the location.
        private OnlineMapsMarker playerMarker;

        private void Start()
        {
            // Create a new marker.
            playerMarker = OnlineMapsMarkerManager.CreateItem(new Vector2(0, 0), null, "Player");

            // Get instance of LocationService.
            OnlineMapsLocationService locationService = OnlineMapsLocationService.instance;

            if (locationService == null)
            {
                Debug.LogError(
                    "Location Service not found.\nAdd Location Service Component (Component / Infinity Code / Online Maps / Plugins / Location Service).");
                return;
            }

            // Subscribe to the change location event.
            locationService.OnLocationChanged += OnLocationChanged;
        }

        // When the location has changed
        private void OnLocationChanged(Vector2 position)
        {
            // Change the position of the marker.
            playerMarker.position = position;

            // Redraw map.
            OnlineMaps.instance.Redraw();
        }
    }
}
