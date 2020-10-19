/*         INFINITY CODE         */
/*   https://infinity-code.com   */

using UnityEngine;

namespace InfinityCode.OnlineMapsExamples
{
    /// <summary>
    /// Search for a location by name, calculates best position and zoom to show it.
    /// </summary>
    [AddComponentMenu("Infinity Code/Online Maps/Examples (API Usage)/FindLocationExample")]
    public class FindLocationExample : MonoBehaviour
    {
        /// <summary>
        /// Google API Key
        /// </summary>
        public  string googleAPIKey;

        /// <summary>
        /// Add marker at first found location.
        /// </summary>
        public bool addMarker = true;

        /// <summary>
        /// Log Google Geocode API response.
        /// </summary>
        public bool logResponse = true;

        /// <summary>
        /// Set map position at first found location.
        /// </summary>
        public bool setPosition = true;

        /// <summary>
        /// Set best zoom at first found location.
        /// </summary>
        public bool setZoom = true;

        private void Start()
        {
            if (string.IsNullOrEmpty(googleAPIKey)) Debug.LogWarning("Please specify Google API Key");

            // Start search Chicago.
            OnlineMapsGoogleGeocoding request = new OnlineMapsGoogleGeocoding("Chicago", googleAPIKey);
            request.Send();

            // Specifies that search results should be sent to OnFindLocationComplete.
            request.OnComplete += OnFindLocationComplete;
        }

        private void OnFindLocationComplete(string result)
        {
            // Log Google Geocode API response.
            if (logResponse) Debug.Log(result);

            // Get the coordinates of the first found location.
            Vector2 position = OnlineMapsGoogleGeocoding.GetCoordinatesFromResult(result);

            if (position != Vector2.zero)
            {
                // Create a new marker at the position of Chicago.
                if (addMarker) OnlineMapsMarkerManager.CreateItem(position, "Chicago");

                // Set best zoom
                if (setZoom)
                {
                    // Load response XML
                    OnlineMapsXML xml = OnlineMapsXML.Load(result);

                    // Get bounds node
                    OnlineMapsXML bounds = xml.Find("//geometry/viewport");
                    if (!bounds.isNull)
                    {
                        // Get corners nodes
                        OnlineMapsXML southwest = bounds["southwest"];
                        OnlineMapsXML northeast = bounds["northeast"];

                        // Get coordinates from nodes
                        Vector2 sw = OnlineMapsXML.GetVector2FromNode(southwest);
                        Vector2 ne = OnlineMapsXML.GetVector2FromNode(northeast);

                        // Get best zoom
                        Vector2 center;
                        int zoom;
                        OnlineMapsUtils.GetCenterPointAndZoom(new[] {sw, ne}, out center, out zoom);

                        // Set map zoom
                        OnlineMaps.instance.zoom = zoom;
                    }
                }

                // Set map position
                if (setPosition) OnlineMaps.instance.position = position;
            }
            else
            {
                Debug.Log("Oops... Something is wrong.");
            }
        }
    }
}