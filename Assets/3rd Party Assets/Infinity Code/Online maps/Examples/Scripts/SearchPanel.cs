/*         INFINITY CODE         */
/*   https://infinity-code.com   */

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace InfinityCode.OnlineMapsDemos
{
    [AddComponentMenu("Infinity Code/Online Maps/Demos/Search Panel")]
    public class SearchPanel:MonoBehaviour
    {
        public InputField inputField;
        private OnlineMapsMarker marker;

        public void Search()
        {
            if (!OnlineMapsKeyManager.hasGoogleMaps)
            {
                Debug.LogWarning("Please enter Map / Key Manager / Google Maps");
                return;
            }

            if (inputField == null) return;
            if (inputField.text.Length < 3) return;

            string locationName = inputField.text;

            OnlineMapsGoogleGeocoding request = new OnlineMapsGoogleGeocoding(locationName, OnlineMapsKeyManager.GoogleMaps());
            request.OnComplete += OnGeocodingComplete;
            request.Send();
        }

        private void OnGeocodingComplete(string response)
        {
            OnlineMapsGoogleGeocodingResult[] results = OnlineMapsGoogleGeocoding.GetResults(response);
            if (results == null || results.Length == 0)
            {
                Debug.Log(response);
                return;
            }

            OnlineMapsGoogleGeocodingResult r = results[0];
            OnlineMaps.instance.position = r.geometry_location;

            Vector2 center;
            int zoom;
            OnlineMapsUtils.GetCenterPointAndZoom(new[] { r.geometry_bounds_northeast, r.geometry_bounds_southwest }, out center, out zoom);
            OnlineMaps.instance.zoom = zoom;

            if (marker == null) marker = OnlineMapsMarkerManager.CreateItem(r.geometry_location, r.formatted_address);
            else
            {
                marker.position = r.geometry_location;
                marker.label = r.formatted_address;
            }
        }

        private void Update()
        {
            EventSystem eventSystem = EventSystem.current;
            if ((Input.GetKeyUp(KeyCode.KeypadEnter) || Input.GetKeyUp(KeyCode.Return)) && eventSystem.currentSelectedGameObject == inputField.gameObject)
            {
                Search();
            }
        }
    }
}
