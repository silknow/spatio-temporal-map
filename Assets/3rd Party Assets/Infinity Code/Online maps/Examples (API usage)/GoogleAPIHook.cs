/*         INFINITY CODE         */
/*   https://infinity-code.com   */

using UnityEngine;

namespace InfinityCode.OnlineMapsExamples
{
    /// <summary>
    /// Example of how to redirect specific requests through a proxy
    /// </summary>
    [AddComponentMenu("Infinity Code/Online Maps/Examples (API Usage)/GoogleAPIHook")]
    public class GoogleAPIHook : MonoBehaviour
    {
        /// <summary>
        /// Proxy url
        /// </summary>
        public string proxy = "http://localhost/redirectFast.php?";

        /// <summary>
        /// This method is called when each request is initialized
        /// </summary>
        /// <param name="url">URL of request</param>
        /// <returns>Processed url</returns>
        private string OnInit(string url)
        {
            // If url should be redirected, return proxy + url
            if (url.Contains("maps.googleapis.com/maps/api")) return proxy + url;

            // Otherwise, return original url
            return url;
        }

        /// <summary>
        /// This method is called when the script starts
        /// </summary>
        private void Start()
        {
            // Subscribe to request initialization event
            OnlineMapsWWW.OnInit += OnInit;
        }
    }
}