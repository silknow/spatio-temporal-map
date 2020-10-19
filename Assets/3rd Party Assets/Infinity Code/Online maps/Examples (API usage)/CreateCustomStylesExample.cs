/*         INFINITY CODE         */
/*   https://infinity-code.com   */

using UnityEngine;

namespace InfinityCode.OnlineMapsExamples
{
    /// <summary>
    /// Example of how to dynamically create custom styles
    /// </summary>
    [AddComponentMenu("Infinity Code/Online Maps/Examples (API Usage)/CreateCustomStylesExample")]
    public class CreateCustomStylesExample : MonoBehaviour
    {
        public string style1 = "https://a.tiles.mapbox.com/v4/mapbox.satellite/{zoom}/{x}/{y}.png?access_token=";
        public string style2 = "https://a.tiles.mapbox.com/v4/mapbox.streets/{zoom}/{x}/{y}.png?access_token=";
        public string mapboxAccessToken;

        private bool useFirstStyle = true;

        private void OnGUI()
        {
            if (GUILayout.Button("Change Style"))
            {
                useFirstStyle = !useFirstStyle;
                
                // Switch map type
                OnlineMaps.instance.mapType = "myprovider.style" + (useFirstStyle ? "1" : "2");
            }
        }

        private void Start()
        {
            // Create a new provider
            OnlineMapsProvider.Create("myprovider").AppendTypes(
                // Create a new map types
                new OnlineMapsProvider.MapType("style1") { urlWithLabels = style1 + mapboxAccessToken, },
                new OnlineMapsProvider.MapType("style2") { urlWithLabels = style2 + mapboxAccessToken, }
            );

            // Select map type
            OnlineMaps.instance.mapType = "myprovider.style1";
        }
    }
}