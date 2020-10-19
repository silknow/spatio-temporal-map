/*         INFINITY CODE         */
/*   https://infinity-code.com   */

using UnityEngine;

namespace InfinityCode.OnlineMapsExamples
{
    /// <summary>
    /// Example of rotation of the camera together with a marker.
    /// </summary>
    [AddComponentMenu("Infinity Code/Online Maps/Examples (API Usage)/RotateMapInsteadMarkerExample")]
    public class RotateMapInsteadMarkerExample : MonoBehaviour
    {
        private OnlineMapsMarker marker;
        private OnlineMapsCameraOrbit cameraOrbit;

        private void Start()
        {
            cameraOrbit = OnlineMapsCameraOrbit.instance;

            // Create a new marker.
            marker = OnlineMapsMarkerManager.CreateItem(new Vector2(), "Player");

            // Subscribe to UpdateBefore event.
            OnlineMaps.instance.OnUpdateBefore += OnUpdateBefore;
        }

        private void OnUpdateBefore()
        {
            // Update camera rotation
            cameraOrbit.rotation = new Vector2(30, marker.rotation * 360);
        }
    }
}