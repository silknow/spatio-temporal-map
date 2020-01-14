/*     INFINITY CODE 2013-2019      */
/*   http://www.infinity-code.com   */

using System.Collections.Generic;
using UnityEngine;

namespace InfinityCode.OnlineMapsExamples
{
    /// <summary>
    /// Example of a request to Open Route Service.
    /// </summary>
    [AddComponentMenu("Infinity Code/Online Maps/Examples (API Usage)/OpenRouteServiceExample")]
    public class OpenRouteServiceExample : MonoBehaviour
    {
        /// <summary>
        /// Open Route Service API key
        /// </summary>
        public string key;

        private void Start()
        {
            // Looking for pedestrian route between the coordinates.
            OnlineMapsOpenRouteService.Directions(
                new OnlineMapsOpenRouteService.DirectionParams(key, 
                    new []
                    {
                        // Coordinates
                        new OnlineMapsVector2d(8.6817521f, 49.4173462f), 
                        new OnlineMapsVector2d(8.6828883f, 49.4067577f)
                    })
                {
                    // Extra params
                    language = "ru",
                    profile = OnlineMapsOpenRouteService.DirectionParams.Profile.footWalking
                }).OnComplete += OnRequestComplete;
        }

        /// <summary>
        /// This method is called when a response is received.
        /// </summary>
        /// <param name="response">Response string</param>
        private void OnRequestComplete(string response)
        {
            Debug.Log(response);

            OnlineMapsOpenRouteServiceDirectionResult result = OnlineMapsOpenRouteService.GetDirectionResults(response);
            if (result == null || result.routes.Length == 0)
            {
                Debug.Log("Open Route Service Directions failed.");
                return;
            }

            // Get the points of the first route.
            List<OnlineMapsVector2d> points = result.routes[0].points;

            // Draw the route.
            OnlineMapsDrawingElementManager.AddItem(new OnlineMapsDrawingLine(points, Color.red));

            // Set the map position to the first point of route.
            OnlineMaps.instance.position = points[0];
        }
    }
}