/*         INFINITY CODE         */
/*   https://infinity-code.com   */

using UnityEngine;

namespace InfinityCode.OnlineMapsExamples
{
    /// <summary>
    /// Example of get place predictions from Google Autocomplete API.
    /// </summary>
    [AddComponentMenu("Infinity Code/Online Maps/Examples (API Usage)/FindAutocompleteExample")]
    public class FindAutocompleteExample : MonoBehaviour
    {
        /// <summary>
        /// Google API Key
        /// </summary>
        public string apiKey;

        private void Start()
        {
            // Makes a request to Google Places Autocomplete API.
            OnlineMapsGooglePlacesAutocomplete.Find(
                "Los ang",
                apiKey
                ).OnComplete += OnComplete;
        }

        /// <summary>
        /// This method is called when a response is received.
        /// </summary>
        /// <param name="s">Response string</param>
        private void OnComplete(string s)
        {
            // Trying to get an array of results.
            OnlineMapsGooglePlacesAutocompleteResult[] results = OnlineMapsGooglePlacesAutocomplete.GetResults(s);

            // If there is no result
            if (results == null)
            {
                Debug.Log("Error");
                Debug.Log(s);
                return;
            }

            // Log description of each result.
            foreach (OnlineMapsGooglePlacesAutocompleteResult result in results)
            {
                Debug.Log(result.description);
            }
        }
    }
}