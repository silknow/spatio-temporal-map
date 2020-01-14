/*     INFINITY CODE 2013-2019      */
/*   http://www.infinity-code.com   */

using UnityEngine;

namespace InfinityCode.OnlineMapsExamples
{
    /// <summary>
    /// Search for a POIs, by using AMap search
    /// </summary>
    [AddComponentMenu("Infinity Code/Online Maps/Examples (API Usage)/AMapSearchExample")]
    public class AMapSearchExample : MonoBehaviour
    {
        /// <summary>
        /// AMap API Key
        /// </summary>
        public string key;

        private void Start()
        {
            // Start a new search
            OnlineMapsAMapSearch.Find(new OnlineMapsAMapSearch.TextParams(key)
            {
                // Params of request
                keywords = "北京大学",
                city = "beijing",

            }).OnComplete += OnComplete; // Subscribe to OnComplete event
        }

        /// <summary>
        /// On request Complete
        /// </summary>
        /// <param name="response">Response</param>
        private void OnComplete(string response)
        {
            // Log response
            Debug.Log(response);

            // Load result object
            OnlineMapsAMapSearchResult result = OnlineMapsAMapSearch.GetResult(response);

            // Validate result and status
            if (result == null || result.status != 1) return;

            foreach (OnlineMapsAMapSearchResult.POI poi in result.pois)
            {
                // Get POI location
                double lng, lat;
                poi.GetLocation(out lng, out lat);

                // Create a new marker for each POI
                OnlineMapsMarkerManager.CreateItem(lng, lat, poi.name);
            }
        }
    }
}