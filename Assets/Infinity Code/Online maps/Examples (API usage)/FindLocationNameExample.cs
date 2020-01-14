/*     INFINITY CODE 2013-2019      */
/*   http://www.infinity-code.com   */

using UnityEngine;

namespace InfinityCode.OnlineMapsExamples
{
    /// <summary>
    /// Example how to find the name of the location by coordinates.
    /// </summary>
    [AddComponentMenu("Infinity Code/Online Maps/Examples (API Usage)/FindLocationNameExample")]
    public class FindLocationNameExample : MonoBehaviour
    {
        /// <summary>
        /// Google API Key
        /// </summary>
        public string googleAPIKey;

        private void Start()
        {
            // Subscribe to click event.
            OnlineMapsControlBase.instance.OnMapClick += OnMapClick;
        }

        private void OnMapClick()
        {
            // Get the coordinates where the user clicked.
            Vector2 mouseCoords = OnlineMapsControlBase.instance.GetCoords();

            // Try find location name by coordinates.
            OnlineMapsGoogleGeocoding request = new OnlineMapsGoogleGeocoding(mouseCoords, googleAPIKey);
            request.Send();
            request.OnComplete += OnRequestComplete;
        }

        private void OnRequestComplete(string s)
        {
            // Show response in console.
            Debug.Log(s);
        }
    }
}