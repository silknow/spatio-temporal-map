/*     INFINITY CODE 2013-2019      */
/*   http://www.infinity-code.com   */

using UnityEngine;
using UnityEngine.UI;

namespace InfinityCode.OnlineMapsExamples
{
    /// <summary>
    /// Example of how to make a tooltip using uGUI for all markers
    /// </summary>
    [AddComponentMenu("Infinity Code/Online Maps/Examples (API Usage)/uGUICustomTooltipForAllMarkersExample")]
    public class uGUICustomTooltipForAllMarkersExample : MonoBehaviour
    {
        /// <summary>
        /// Prefab of the tooltip
        /// </summary>
        public GameObject tooltipPrefab;

        /// <summary>
        /// Container for tooltip
        /// </summary>
        public Canvas container;

        private GameObject tooltip;

        private void Start()
        {
            OnlineMapsMarkerManager.CreateItem(Vector2.zero, "Marker 1");
            OnlineMapsMarkerManager.CreateItem(new Vector2(1, 1), "Marker 2");
            OnlineMapsMarkerManager.CreateItem(new Vector2(2, 1), "Marker 3");
            OnlineMapsMarkerBase.OnMarkerDrawTooltip = delegate { };

            OnlineMaps.instance.OnUpdateLate += OnUpdateLate;
        }

        private void OnUpdateLate()
        {
            OnlineMapsMarker tooltipMarker = OnlineMapsTooltipDrawerBase.tooltipMarker as OnlineMapsMarker;
            if (tooltipMarker != null)
            {
                if (tooltip == null)
                {
                    tooltip = Instantiate(tooltipPrefab) as GameObject;
                    (tooltip.transform as RectTransform).SetParent(container.transform);
                }
                Vector2 screenPosition = OnlineMapsControlBase.instance.GetScreenPosition(tooltipMarker.position);
                screenPosition.y += tooltipMarker.height;
                Vector2 point;
                Camera cam = container.renderMode == RenderMode.ScreenSpaceOverlay ? null : container.worldCamera;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(container.transform as RectTransform, screenPosition, cam, out point);
                (tooltip.transform as RectTransform).localPosition = point;
                tooltip.GetComponentInChildren<Text>().text = tooltipMarker.label;

            }
            else
            {
                OnlineMapsUtils.Destroy(tooltip);
                tooltip = null;
            }
        }
    }
}