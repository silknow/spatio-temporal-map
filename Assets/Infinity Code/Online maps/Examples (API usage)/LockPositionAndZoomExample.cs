/*     INFINITY CODE 2013-2019      */
/*   http://www.infinity-code.com   */

using UnityEngine;

namespace InfinityCode.OnlineMapsExamples
{
    /// <summary>
    /// Example of how to limit the position and zoom the map.
    /// </summary>
    [AddComponentMenu("Infinity Code/Online Maps/Examples (API Usage)/LockPositionAndZoomExample")]
    public class LockPositionAndZoomExample : MonoBehaviour
    {
        private void Start()
        {
            // Lock map zoom range
            OnlineMaps.instance.zoomRange = new OnlineMapsRange(10, 15);

            // Lock map coordinates range
            OnlineMaps.instance.positionRange = new OnlineMapsPositionRange(33, -119, 34, -118);

            // Initializes the position and zoom
            OnlineMaps.instance.zoom = 10;
            OnlineMaps.instance.position = OnlineMaps.instance.positionRange.center;
        }
    }
}