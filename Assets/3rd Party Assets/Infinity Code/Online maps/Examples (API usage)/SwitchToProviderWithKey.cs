/*         INFINITY CODE         */
/*   https://infinity-code.com   */

using UnityEngine;

namespace InfinityCode.OnlineMapsExamples
{
    /// <summary>
    /// Example of how to switch to a provider that requires authorization
    /// </summary>
    [AddComponentMenu("Infinity Code/Online Maps/Examples (API Usage)/SwitchToProviderWithKey")]
    public class SwitchToProviderWithKey : MonoBehaviour
    {
        /// <summary>
        /// Draws UI elements using IMGUI
        /// </summary>
        private void OnGUI()
        {
            if (GUILayout.Button("Set DigitalGlobe"))
            {
                // Switch to DigitalGlobe / Satellite
                string mapTypeID = "digitalglobe.satellite";

                OnlineMaps.instance.mapType = mapTypeID;

                // Get map type
                OnlineMapsProvider.MapType mapType = OnlineMapsProvider.FindMapType(mapTypeID);

                // Try get access token field from map type
                OnlineMapsProvider.ExtraField field = GetExtraField(mapType.extraFields, "accesstoken");

                // If the field is not in the map type, try to get it from the provider
                if (field == null) field = GetExtraField(mapType.provider.extraFields, "accesstoken");

                // If the field is present, set value
                if (field != null) field.value = "My DigitalGlobe Token";
            }
        }

        /// <summary>
        /// Get field by name
        /// </summary>
        /// <param name="extraFields">Array of fields</param>
        /// <param name="token">Field name</param>
        /// <returns>Field or null</returns>
        private static OnlineMapsProvider.ExtraField GetExtraField(OnlineMapsProvider.IExtraField[] extraFields, string token)
        {
            if (extraFields == null) return null;

            // Iterate each field
            foreach (var field in extraFields)
            {
                // If this is a field, compare it token
                if (field is OnlineMapsProvider.ExtraField)
                {
                    OnlineMapsProvider.ExtraField f = field as OnlineMapsProvider.ExtraField;
                    if (f.token == token) return f;
                }
                // If this is group, check group fields
                else if (field is OnlineMapsProvider.ToggleExtraGroup)
                {
                    var res = GetExtraField((field as OnlineMapsProvider.ToggleExtraGroup).fields, token);
                    if (res != null) return res;
                }
            }

            return null;
        }
    }
}