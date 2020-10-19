/*         INFINITY CODE         */
/*   https://infinity-code.com   */

using System.Collections.Generic;
using UnityEngine;

namespace InfinityCode.OnlineMapsExamples
{
    /// <summary>
    /// Example of runtime saving map state.
    /// </summary>
    [AddComponentMenu("Infinity Code/Online Maps/Examples (API Usage)/SaveMapStateExample")]
    public class SaveMapStateExample : MonoBehaviour
    {
        private string key = "MapSettings";

        /// <summary>
        /// Loading saved state.
        /// </summary>
        private void LoadState()
        {
            if (!PlayerPrefs.HasKey(key)) return;

            OnlineMaps map = OnlineMaps.instance;

            OnlineMapsXML prefs = OnlineMapsXML.Load(PlayerPrefs.GetString(key));

            OnlineMapsXML generalSettings = prefs["General"];
            map.position = generalSettings.Get<Vector2>("Coordinates");
            map.zoom = generalSettings.Get<int>("Zoom");

            List<OnlineMapsMarker> markers = new List<OnlineMapsMarker>();

            OnlineMapsMarkerManager.SetItems(markers);
        }

        private void OnGUI()
        {
            // By clicking on the button to save the current state.
            if (GUI.Button(new Rect(5, 5, 150, 30), "Save State")) SaveState();
        }

        private void SaveState()
        {
            OnlineMaps map = OnlineMaps.instance;

            //OnlineMapsXML prefs = new OnlineMapsXML("Map");


            // Save position and zoom
            /*OnlineMapsXML generalSettings = prefs.Create("General");
            generalSettings.Create("Coordinates", map.position);
            generalSettings.Create("Zoom", map.zoom);

            // Save 2D markers
            map.SaveMarkers(prefs);

            // Save 3D markers
            OnlineMapsJSONItem markers3DJSON = OnlineMapsMarker3DManager.instance.ToJSON();

            // Save settings to PlayerPrefs
            PlayerPrefs.SetString(key, prefs.outerXml);*/
        }

        private void Start()
        {
            LoadState();
        }
    }
}