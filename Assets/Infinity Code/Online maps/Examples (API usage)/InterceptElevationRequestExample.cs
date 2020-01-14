/*     INFINITY CODE 2013-2019      */
/*   http://www.infinity-code.com   */

using UnityEngine;

namespace InfinityCode.OnlineMapsExamples
{
    /// <summary>
    /// Example of how to intercept the request to the elevation data, and send elevation value to Online Maps.
    /// </summary>
    [AddComponentMenu("Infinity Code/Online Maps/Examples (API Usage)/InterceptElevationRequestExample")]
    public class InterceptElevationRequestExample : MonoBehaviour
    {
        private OnlineMapsTileSetControl control;

        private void Start()
        {
            // Get Tileset control.
            control = OnlineMapsControlBase.instance as OnlineMapsTileSetControl;

            if (control == null)
            {
                Debug.LogError("You must use the Tileset control.");
                return;
            }

            // Intercept elevation request
            OnlineMapsElevationManagerBase.instance.OnGetElevation += OnGetElevation;
        }

        private void OnGetElevation(double leftLongitude, double topLatitude, double rightLongitude, double bottomLatitude)
        {
            // Elevation map must be 32x32
            short[,] elevation = new short[32, 32];

            // Here you get the elevation from own sources.

            // Set elevation map
            OnlineMapsBingMapsElevationManager.instance.SetElevationData(elevation);
        }
    }
}