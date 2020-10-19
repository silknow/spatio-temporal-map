/*         INFINITY CODE         */
/*   https://infinity-code.com   */

using UnityEngine;

namespace InfinityCode.OnlineMapsExamples
{
    /// <summary>
    /// Example of how to override the input, and use the center of screen as the cursor, and Z key as a left mouse button.
    /// </summary>
    [AddComponentMenu("Infinity Code/Online Maps/Examples (API Usage)/InterceptInputExample")]
    public class InterceptInputExample : MonoBehaviour
    {
        private Vector2 OnGetInputPosition()
        {

#if !UNITY_EDITOR
            // On the device returns center of screen.
            return Camera.main.ViewportToScreenPoint(new Vector3(0.5F, 0.5F, 0));
 #else
            // In the editor returns the coordinates of the mouse cursor.
            return Input.mousePosition;
#endif
        }

        private int OnGetTouchCount()
        {
            // If pressed Z, then it will work like the left mouse button.
            return Input.GetKey(KeyCode.Z) ? 1 : 0;
        }

        private void Start()
        {
            // Intercepts getting the cursor coordinates.
            OnlineMapsControlBase.instance.OnGetInputPosition += OnGetInputPosition;

            // Intercepts getting the number of touches.
            OnlineMapsControlBase.instance.OnGetTouchCount += OnGetTouchCount;
        }
    }
}