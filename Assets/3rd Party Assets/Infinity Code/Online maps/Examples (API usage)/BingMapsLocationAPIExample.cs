/*         INFINITY CODE         */
/*   https://infinity-code.com   */

using System.Collections.Generic;
using UnityEngine;

namespace InfinityCode.OnlineMapsExamples
{
    /// <summary>
    /// Example of a request to Bing Maps Location API, and get the result.
    /// </summary>
    [AddComponentMenu("Infinity Code/Online Maps/Examples (API Usage)/BingMapsLocationAPIExample")]
    public class BingMapsLocationAPIExample : MonoBehaviour
    {
        /// <summary>
        /// Bing Maps API Key
        /// </summary>
        public string key;

        private void Start()
        {
            // Looking for a location by name.
            OnlineMapsBingMapsLocation.FindByQuery("Moscow", key).OnComplete += OnQueryComplete;

            // Subscribe to map click event.
            OnlineMapsControlBase.instance.OnMapClick += OnMapClick;
        }

        /// <summary>
        /// This method is called when a response is received.
        /// </summary>
        /// <param name="response">Response string</param>
        private static void OnQueryComplete(string response)
        {
            Debug.Log(response);

            // Get an array of results.
            OnlineMapsBingMapsLocationResult[] results = OnlineMapsBingMapsLocation.GetResults(response);
            if (results == null)
            {
                Debug.Log("No results");
                return;
            }

            // Log results info.
            Debug.Log(results.Length);
            foreach (OnlineMapsBingMapsLocationResult result in results)
            {
                Debug.Log(result.name);
                Debug.Log(result.formattedAddress);
                foreach (KeyValuePair<string, string> pair in result.address)
                {
                    Debug.Log(pair.Key + ": " + pair.Value);
                }
                Debug.Log("------------------------------");
            }
        }

        /// <summary>
        /// This method is called when click on map.
        /// </summary>
        private void OnMapClick()
        {
            // Looking for a location by coordinates.
            OnlineMapsBingMapsLocation.FindByPoint(OnlineMaps.instance.position, key).OnComplete += OnQueryComplete;
        }
    }
}