/*         INFINITY CODE         */
/*   https://infinity-code.com   */

using UnityEngine;

namespace InfinityCode.OnlineMapsExamples
{
    /// <summary>
    /// Example how to find out that all tiles are loaded
    /// </summary>
    [AddComponentMenu("Infinity Code/Online Maps/Examples (API Usage)/CheckAllTilesLoadedExample")]
    public class CheckAllTilesLoadedExample : MonoBehaviour
    {
        private void Start()
        {
            // Subscribe to OnAllTilesLoaded
            OnlineMapsTile.OnAllTilesLoaded += OnAllTilesLoaded;
        }

        /// <summary>
        /// This method will be called when all tiles are loaded
        /// </summary>
        private void OnAllTilesLoaded()
        {
            Debug.Log("All tiles loaded");
        }
    }
}