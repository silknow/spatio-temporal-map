/*     INFINITY CODE 2013-2019      */
/*   http://www.infinity-code.com   */

using UnityEngine;

namespace InfinityCode.OnlineMapsExamples
{
    /// <summary>
    /// Example of how to draw tooltip without a background.
    /// </summary>
    [AddComponentMenu("Infinity Code/Online Maps/Examples (API Usage)/TooltipWithoutBackgroundExample")]
    public class TooltipWithoutBackgroundExample : MonoBehaviour
    {
        private void Start()
        {
            // Subscribe to the event preparation of tooltip style.
            OnlineMapsGUITooltipDrawer.OnPrepareTooltipStyle += OnPrepareTooltipStyle;
        }

        private void OnPrepareTooltipStyle(ref GUIStyle style)
        {
            // Hide background.
            style.normal.background = null;
        }
    }
}