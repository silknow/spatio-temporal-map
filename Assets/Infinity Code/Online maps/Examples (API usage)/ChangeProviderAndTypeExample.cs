/*     INFINITY CODE 2013-2019      */
/*   http://www.infinity-code.com   */

using UnityEngine;

namespace InfinityCode.OnlineMapsExamples
{
    /// <summary>
    /// Example how to get the available providers and to change the current provider.
    /// </summary>
    [AddComponentMenu("Infinity Code/Online Maps/Examples (API Usage)/ChangeProviderAndTypeExample")]
    public class ChangeProviderAndTypeExample : MonoBehaviour
    {
        /// <summary>
        /// Logs providers id and map types
        /// </summary>
        private void LogTypeList()
        {
            // Gets all providers
            OnlineMapsProvider[] providers = OnlineMapsProvider.GetProviders();
            foreach (OnlineMapsProvider provider in providers)
            {
                Debug.Log(provider.id);
                foreach (OnlineMapsProvider.MapType type in provider.types)
                {
                    Debug.Log(type);
                }
            }
        }

        private void Start()
        {
            // Show full provider list
            LogTypeList();

            // Select Google Satellite
            OnlineMaps.instance.mapType = "google.satellite"; // providerID.typeID

            // Select the first type for ArcGIS.
            OnlineMaps.instance.mapType = "arcgis"; // providerID
        }
    }
}