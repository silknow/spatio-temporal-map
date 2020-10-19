/*         INFINITY CODE         */
/*   https://infinity-code.com   */

using UnityEngine;

namespace InfinityCode.OnlineMapsExamples
{
    /// <summary>
    /// Example of the conversion of screen coordinates into geographic coordinates.
    /// </summary>
    [AddComponentMenu("Infinity Code/Online Maps/Examples (API Usage)/ScreenToGeoExample")]
    public class ScreenToGeoExample : MonoBehaviour
    {
        private void Update()
        {
            // Screen coordinate of the cursor.
            Vector3 mousePosition = Input.mousePosition;

            // Converts the screen coordinates to geographic.
            Vector3 mouseGeoLocation = OnlineMapsControlBase.instance.GetCoords(mousePosition);

            // Showing geographical coordinates in the console.
            Debug.Log(mouseGeoLocation);
        }
    }
}