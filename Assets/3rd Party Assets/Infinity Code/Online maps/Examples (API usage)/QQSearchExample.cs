/*         INFINITY CODE         */
/*   https://infinity-code.com   */

using UnityEngine;

namespace InfinityCode.OnlineMapsExamples
{
    /// <summary>
    /// Search for a POIs, by using QQ search
    /// </summary>
    [AddComponentMenu("Infinity Code/Online Maps/Examples (API Usage)/QQSearchExample")]
    public class QQSearchExample : MonoBehaviour
    {
        /// <summary>
        /// QQ api key
        /// </summary>
        public string key;

        private void Start()
        {
            // Start a new search
            OnlineMapsQQSearch.Find(
                key, // QQ api key
                "成都", // keywords
                new OnlineMapsQQSearch.Params("北京") // Params of request with region
                {
                    // Addtional params
                    page_size = 20,
                    page_index = 1
                }
                ).OnComplete += OnComplete; // Subscribe to OnComplete event
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
            OnlineMapsQQSearchResult result = OnlineMapsQQSearch.GetResult(response);

            // Validate result and status
            if (result == null || result.status != 0)
            {
                Debug.Log("Something wrong");
                return;
            }

            foreach (OnlineMapsQQSearchResult.Data data in result.data)
            {
                // Create a new marker for each POI
                OnlineMapsMarkerManager.CreateItem(data.location.lng, data.location.lat, data.title);
            }
        }
    }
}