/*         INFINITY CODE         */
/*   https://infinity-code.com   */

using UnityEngine;

namespace InfinityCode.OnlineMapsDemos
{
    [AddComponentMenu("Infinity Code/Online Maps/Demos/UICompass")]
    public class UICompass : MonoBehaviour
    {
        public bool animated = true;
        public float duration = 0.3f;
        public RectTransform arrow;

        private OnlineMapsCameraOrbit cameraOrbit;

        private float targetPan;
        private float startPan;
        private float progress;
        private bool isAnim;

        private float rotation
        {
            get { return Mathf.Repeat(cameraOrbit.rotation.y, 360); }
        }

        private void OnCameraControl()
        {
            arrow.rotation = Quaternion.Euler(0, 0, cameraOrbit.rotation.y);
        }

        public void RotateLeft()
        {
            SetPan(((int)(rotation / 90) + 1) * 90);
        }

        public void RotateRight()
        {
            SetPan((Mathf.CeilToInt(rotation / 90) - 1) * 90);
        }

        public void SetNorth()
        {
            SetPan(rotation > 180 ? 360 : 0);
        }

        private void SetPan(float pan)
        {
            if (!animated)
            {
                cameraOrbit.rotation.y = pan;
                return;
            }

            targetPan = pan;
            startPan = rotation;
            progress = 0;
            isAnim = true;
        }

        private void Start()
        {
            cameraOrbit = OnlineMapsCameraOrbit.instance;

            if (cameraOrbit == null)
            {
                Debug.LogWarning("UI Compass requires Online Maps Camera Orbit.");
                Destroy(gameObject);
                return;
            }

            cameraOrbit.OnCameraControl += OnCameraControl;
        }

        private void Update()
        {
            if (!isAnim) return;

            progress += Time.deltaTime / duration;
            if (progress >= 1)
            {
                progress = 1;
                isAnim = false;
            }

            cameraOrbit.rotation.y = Mathf.Lerp(startPan, targetPan, progress);
        }
    }
}