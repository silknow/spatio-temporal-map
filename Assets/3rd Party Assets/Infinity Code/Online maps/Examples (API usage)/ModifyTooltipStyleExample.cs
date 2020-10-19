/*         INFINITY CODE         */
/*   https://infinity-code.com   */

using UnityEngine;

namespace InfinityCode.OnlineMapsExamples
{
    /// <summary>
    /// Example of how to intercept preparation of style for drawing tooltips.
    /// </summary>
    [AddComponentMenu("Infinity Code/Online Maps/Examples (API Usage)/ModifyTooltipStyleExample")]
    public class ModifyTooltipStyleExample : MonoBehaviour
    {
        private void Start()
        {
            // Subscribe to the event preparation of tooltip style.
            OnlineMapsGUITooltipDrawer.OnPrepareTooltipStyle += OnPrepareTooltipStyle;
        }

        private void OnPrepareTooltipStyle(ref GUIStyle style)
        {
            // Change the style settings.
            style.fontSize = Screen.width / 50;
        }
    }
}
