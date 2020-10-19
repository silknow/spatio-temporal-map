/*         INFINITY CODE         */
/*   https://infinity-code.com   */

using UnityEngine;

namespace InfinityCode.OnlineMapsExamples
{
    /// <summary>
    /// Example of a dynamic change texture of 2D marker.
    /// </summary>
    [AddComponentMenu("Infinity Code/Online Maps/Examples (API Usage)/ChangeMarkerTextureExample")]
    public class ChangeMarkerTextureExample : MonoBehaviour
    {
        // Must enable "Read / Write enabled".
        public Texture2D newMarkerTexture;

        private void OnGUI()
        {
            // When you click on ...
            if (GUI.Button(new Rect(10, 10, 100, 20), "Change markers"))
            {
                // ... all markers will change the texture.
                foreach (OnlineMapsMarker marker in OnlineMapsMarkerManager.instance)
                {
                    marker.texture = newMarkerTexture;
                    marker.Init();
                }

                // Redraw map
                OnlineMaps.instance.Redraw();
            }
        }
    }
}