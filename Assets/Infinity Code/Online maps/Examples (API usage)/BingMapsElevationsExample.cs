/*     INFINITY CODE 2013-2019      */
/*   http://www.infinity-code.com   */

using System;
using UnityEngine;

namespace InfinityCode.OnlineMapsExamples
{
    /// <summary>
    /// Example of how to get the elevations in the area using Bing Maps Elevation API.
    /// </summary>
    [AddComponentMenu("Infinity Code/Online Maps/Examples (API Usage)/BingMapsElevationsExample")]
    public class BingMapsElevationsExample : MonoBehaviour
    {
        /// <summary>
        /// Bing Maps API key
        /// </summary>
        public string key;

        private void Start()
        {
            // Get the coordinates of the map corners
            double tlx, tly, brx, bry;
            OnlineMaps.instance.GetCorners(out tlx, out tly, out brx, out bry);

            // Ceate a new request and subscribe to OnComplete event
            OnlineMapsBingMapsElevation.GetElevationByBounds(key, tlx, tly, brx, bry, 32, 32).OnComplete += OnComplete;
        }

        /// <summary>
        /// On request Complete
        /// </summary>
        /// <param name="response">Response</param>
        private void OnComplete(string response)
        {
            // Log response
            Debug.Log(response);

            // You have two options:
            // 1. Load the result object. It's slower, but you have the entire object.
            // 2. Load the elevation data into array. It's fast. Supports one-dimensional and two-dimensional arrays.

            // 1. Load result object
            OnlineMapsBingMapsElevationResult result = OnlineMapsBingMapsElevation.GetResult(response, OnlineMapsBingMapsElevation.Output.json);

            // Log elevations length
            if (result != null) Debug.Log(result.resourceSets[0].resources[0].elevations.Length);
            else Debug.Log("Result is null");

            // 2. Load the elevation data into two-dimensional array.
            short[,] elevations = new short[32, 32];
            Array ea = elevations;
            OnlineMapsBingMapsElevation.ParseElevationArray(response, OnlineMapsBingMapsElevation.Output.json, ref ea);

            // Log first elevation value
            Debug.Log(elevations[0, 0]);
        }
    }
}