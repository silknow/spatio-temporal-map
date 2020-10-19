/*         INFINITY CODE         */
/*   https://infinity-code.com   */

using UnityEngine;
using UnityEngine.UI;

namespace InfinityCode.OnlineMapsDemos
{
    [AddComponentMenu("Infinity Code/Online Maps/Demos/Demo")]
    public class Demo : MonoBehaviour
    {
        public Text twoThreeDText;
        public Toggle labelsToggle;
        public Toggle trafficToggle;
        public Toggle elevationsToggle;

        public Transform camera2D;
        public Transform camera3D;

        public float CameraChangeTime = 1;

        private float animValue;
        private OnlineMaps map;
        private bool is2D = true;
        private bool isCameraModeChange;

        private Transform fromTransform;
        private Transform toTransform;

        public void ChangeMode()
        {
            if (isCameraModeChange) return;
            is2D = !is2D;

            twoThreeDText.text = is2D ? "3D" : "2D";
            elevationsToggle.gameObject.SetActive(!is2D);

            animValue = 0;
            isCameraModeChange = true;

            Camera c = Camera.main;
            fromTransform = is2D ? camera3D : camera2D;
            toTransform = is2D ? camera2D : camera3D;

            c.orthographic = false;
            if (!is2D) c.fieldOfView = 28;
        }

        public void SetLabels()
        {
            map.labels = labelsToggle.isOn;
            map.Redraw();
        }

        public void SetTraffic()
        {
            map.traffic = trafficToggle.isOn;
            map.Redraw();
        }

        public void SetElevations()
        {
            OnlineMapsElevationManagerBase.instance.enabled = elevationsToggle.isOn;
            map.Redraw();
        }

        private void Start()
        {
            map = OnlineMaps.instance;
            OnlineMapsElevationManagerBase.instance.enabled = false;
        }

        private void Update()
        {
            if (!isCameraModeChange) return;

            animValue += Time.deltaTime / CameraChangeTime;

            if (animValue > 1)
            {
                animValue = 1;
                isCameraModeChange = false;
            }

            Camera c = Camera.main;

            c.transform.position = Vector3.Lerp(fromTransform.position, toTransform.position, animValue);
            c.transform.rotation = Quaternion.Lerp(fromTransform.rotation, toTransform.rotation, animValue);

            float fromFOV = is2D ? 60 : 28;
            float toFOV = is2D ? 28 : 60;

            c.fieldOfView = Mathf.Lerp(fromFOV, toFOV, animValue);

            if (!isCameraModeChange && is2D) c.orthographic = true;
        }
    }
}