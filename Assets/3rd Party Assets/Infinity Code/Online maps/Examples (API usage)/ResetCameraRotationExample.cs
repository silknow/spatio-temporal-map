/*         INFINITY CODE         */
/*   https://infinity-code.com   */

using UnityEngine;

namespace InfinityCode.OnlineMapsExamples
{
    /// <summary>
    /// Example of animated reset camera rotation.
    /// </summary>
    [AddComponentMenu("Infinity Code/Online Maps/Examples (API Usage)/ResetCameraRotationExample")]
    public class ResetCameraRotationExample : MonoBehaviour
    {
        /// <summary>
        /// Time of animation (sec).
        /// </summary>
        public float animationTime = 3;

        /// <summary>
        /// Animation Curve.
        /// </summary>
        public AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private float time;
        private bool isReset;
        private float camX;
        private float camY;
        private OnlineMapsCameraOrbit cameraOrbit;

        private void OnGUI()
        {
            if (GUI.Button(new Rect(5, 5, 100, 30), "Reset") && !isReset)
            {
                // Store the current rotation, and marks that reset is started.
                camX = cameraOrbit.rotation.x;
                camY = cameraOrbit.rotation.y;
                isReset = true;
            }
        }

        private void Start()
        {
            cameraOrbit = OnlineMapsCameraOrbit.instance;
        }

        private void Update()
        {
            if (!isReset) return;

            // Updating the progress of the animation.
            time += Time.deltaTime;
            float t = time / animationTime;

            // If the animation is finished
            if (t >= 1)
            {
                // Reset values
                time = 0;
                isReset = false;
                t = 1;
            }

            // Update the rotation of camera.
            float f = animationCurve.Evaluate(t);
            cameraOrbit.rotation.x = Mathf.Lerp(camX, 0, f);
            cameraOrbit.rotation.y = Mathf.Lerp(camY, 0, f);
        }
    }
}