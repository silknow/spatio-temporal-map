/*     INFINITY CODE 2013-2019      */
/*   http://www.infinity-code.com   */

using UnityEngine;
using UnityEngine.UI;

namespace InfinityCode.OnlineMapsExamples
{
    /// <summary>
    /// Example of how to make a tooltip using uGUI for a single marker
    /// </summary>
    [AddComponentMenu("Infinity Code/Online Maps/Examples (API Usage)/uGUICustomTooltipExample")]
    public class uGUICustomTooltipExample : MonoBehaviour
    {
        /// <summary>
        /// Prefab of the tooltip
        /// </summary>
        public GameObject tooltipPrefab;

        /// <summary>
        /// Container for tooltip
        /// </summary>
        public Canvas container;

        private OnlineMapsMarker marker;
        private GameObject tooltip;

	    private void Start ()
        {
            marker = OnlineMapsMarkerManager.CreateItem(Vector2.zero, "Hello World");
            marker.OnDrawTooltip = delegate {  };

            OnlineMaps.instance.OnUpdateLate += OnUpdateLate;
        }

        private void OnUpdateLate()
        {
            OnlineMapsMarkerBase tooltipMarker = OnlineMapsTooltipDrawerBase.tooltipMarker;
            if (tooltipMarker == marker)
            {
                if (tooltip == null)
                {
                    tooltip = Instantiate(tooltipPrefab) as GameObject;
                    (tooltip.transform as RectTransform).SetParent(container.transform);
                }
                Vector2 screenPosition = OnlineMapsControlBase.instance.GetScreenPosition(marker.position);
                screenPosition.y += marker.height;
                Vector2 point;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(container.transform as RectTransform, screenPosition, null, out point);
                (tooltip.transform as RectTransform).localPosition = point;
                tooltip.GetComponentInChildren<Text>().text = marker.label;

            }
            else if (tooltip != null)
            {
                OnlineMapsUtils.Destroy(tooltip);
                tooltip = null;
            }
        }
    }
}