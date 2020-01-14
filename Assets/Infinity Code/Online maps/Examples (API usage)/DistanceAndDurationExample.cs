/*     INFINITY CODE 2013-2019      */
/*   http://www.infinity-code.com   */

using System.Linq;
using UnityEngine;

namespace InfinityCode.OnlineMapsExamples
{
    /// <summary>
    /// Example how to calculate the distance and the duration of the route
    /// </summary>
    [AddComponentMenu("Infinity Code/Online Maps/Examples (API Usage)/DistanceAndDurationExample")]
    public class DistanceAndDurationExample : MonoBehaviour
    {
        public string googleAPIKey;

        private void Start()
        {
            if (string.IsNullOrEmpty(googleAPIKey)) Debug.LogWarning("Please specify Google API Key");

            // Find route using Google Directions API
            OnlineMapsGoogleDirections request = new OnlineMapsGoogleDirections(googleAPIKey, "Los Angeles", new Vector2(-118.178960f, 35.063995f));
            request.OnComplete += OnGoogleDirectionsComplete;
            request.Send();
        }

        /// <summary>
        /// This method is called when the response from Google Directions API is received
        /// </summary>
        /// <param name="response">Response from Google Direction API</param>
        private void OnGoogleDirectionsComplete(string response)
        {
            Debug.Log(response);

            // Try load result
            OnlineMapsGoogleDirectionsResult result = OnlineMapsGoogleDirections.GetResult(response);
            if (result == null || result.routes.Length == 0) return;

            // Get the first route
            OnlineMapsGoogleDirectionsResult.Route route = result.routes[0];

            // Draw route on the map
            OnlineMapsDrawingElementManager.AddItem(new OnlineMapsDrawingLine(route.overview_polyline, Color.red, 3));

            // Calculate the distance
            int distance = route.legs.Sum(l => l.distance.value); // meters

            // Calculate the duration
            int duration = route.legs.Sum(l => l.duration.value); // seconds

            // Log distane and duration
            Debug.Log("Distance: " + distance + " meters, or " + (distance / 1000f).ToString("F2") + " km");
            Debug.Log("Duration: " + duration + " sec, or " + (duration / 60f).ToString("F1") + " min, or " + (duration / 3600f).ToString("F1") + " hours");
        }
    }
}
