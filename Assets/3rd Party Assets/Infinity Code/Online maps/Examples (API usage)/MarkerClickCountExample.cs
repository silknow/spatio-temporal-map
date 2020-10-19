/*         INFINITY CODE         */
/*   https://infinity-code.com   */

using UnityEngine;

namespace InfinityCode.OnlineMapsExamples
{
    /// <summary>
    /// Example of calculating the amount of clicking on the marker.
    /// </summary>
    [AddComponentMenu("Infinity Code/Online Maps/Examples (API Usage)/MarkerClickCountExample")]
    public class MarkerClickCountExample : MonoBehaviour
    {
        /// <summary>
        /// Prefab of 3D marker
        /// </summary>
        public GameObject prefab;

        public class MarkerClickCountExampleCustomData
        {
            public int clickCount;
        }

        private void Start()
        {
            // Create a new markers.
            OnlineMapsMarker3D marker1 = OnlineMapsMarker3DManager.CreateItem(new Vector2(0, 0), prefab);
            OnlineMapsMarker3D marker2 = OnlineMapsMarker3DManager.CreateItem(new Vector2(10, 0), prefab);

            // Create new customData.
            marker1["clickCount"] = new MarkerClickCountExampleCustomData();
            marker2["clickCount"] = new MarkerClickCountExampleCustomData();

            // Subscribe to click event.
            marker1.OnClick += OnMarkerClick;
            marker2.OnClick += OnMarkerClick;

            marker1.OnPress += OnPress;
        }

        private void OnPress(OnlineMapsMarkerBase onlineMapsMarkerBase)
        {
            Debug.Log("OnPress");
        }

        private void OnMarkerClick(OnlineMapsMarkerBase marker)
        {
            MarkerClickCountExampleCustomData data = marker["clickCount"] as MarkerClickCountExampleCustomData;
            if (data == null) return;

            data.clickCount++;
            Debug.Log(data.clickCount);
        }
    }    
}