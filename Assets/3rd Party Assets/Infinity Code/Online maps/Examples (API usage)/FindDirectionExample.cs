/*         INFINITY CODE         */
/*   https://infinity-code.com   */

using System.Collections.Generic;
using UnityEngine;

namespace InfinityCode.OnlineMapsExamples
{
    /// <summary>
    /// Search a route between two locations and draws the route.
    /// </summary>
    [AddComponentMenu("Infinity Code/Online Maps/Examples (API Usage)/FindDirectionExample")]
    public class FindDirectionExample : MonoBehaviour
    {
        public string googleAPIKey;

        private void Start()
        {
            if (string.IsNullOrEmpty(googleAPIKey)) Debug.LogWarning("Please specify Google API Key");

            // Begin to search a route from Los Angeles to the specified coordinates.
            OnlineMapsGoogleDirections request = new OnlineMapsGoogleDirections
            (
                googleAPIKey, 
                "Los Angeles", // FROM (string or Vector2)
                new Vector2(-118.178960f, 35.063995f) // TO (string or Vector2)
            );

            // Specifies that search results must be sent to OnFindDirectionComplete.
            request.OnComplete += OnFindDirectionComplete;

            request.Send();
        }

        private void OnFindDirectionComplete(string response)
        {
            // Get the resut object.
            OnlineMapsGoogleDirectionsResult result = OnlineMapsGoogleDirections.GetResult(response);

            // Check that the result is not null, and the number of routes is not zero.
            if (result == null || result.routes.Length == 0) 
            {
                Debug.Log("Find direction failed");
                Debug.Log(response);
                return;
            }

            // Showing the console instructions for each step.
            foreach (OnlineMapsGoogleDirectionsResult.Leg leg in result.routes[0].legs)
            {
                foreach (OnlineMapsGoogleDirectionsResult.Step step in leg.steps)
                {
                    Debug.Log(step.string_instructions);
                }
            }

            // Create a line, on the basis of points of the route.
            OnlineMapsDrawingLine route = new OnlineMapsDrawingLine(result.routes[0].overview_polylineD, Color.green);

            // Draw the line route on the map.
            OnlineMapsDrawingElementManager.AddItem(route);
        }
    }
}