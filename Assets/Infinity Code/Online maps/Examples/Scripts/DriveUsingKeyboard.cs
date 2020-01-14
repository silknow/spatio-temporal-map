/*     INFINITY CODE 2013-2019      */
/*   http://www.infinity-code.com   */

using UnityEngine;

namespace InfinityCode.OnlineMapsDemos
{
    [AddComponentMenu("Infinity Code/Online Maps/Demos/DriveUsingKeyboard")]
    public class DriveUsingKeyboard : MonoBehaviour
    {
        public GameObject prefab;
        public float markerScale = 5f;
        public float speed;
        public float maxSpeed = 160;
        public float rotation;
        public bool rotateCamera = true;
        public bool centerOnMarker = true;

        private OnlineMaps map;
        private OnlineMapsMarker3D marker;
        private double lng, lat;

        private void Start()
        {
            map = OnlineMaps.instance;

            map.GetPosition(out lng, out lat);

            marker = OnlineMapsMarker3DManager.CreateItem(lng, lat, prefab);
            marker.scale = markerScale;
            marker.rotationY = rotation;
        }

        private void Update()
        {
            float acc = Input.GetAxis("Vertical");
            if (Mathf.Abs(acc) > 0) speed = Mathf.Lerp(speed, maxSpeed * Mathf.Sign(acc), Time.deltaTime * Mathf.Abs(acc));
            else speed = Mathf.Lerp(speed, 0, Time.deltaTime * 0.1f);

            if (Mathf.Abs(speed) < 0.1) return;

            float r = Input.GetAxis("Horizontal");
            rotation += r * Time.deltaTime * speed;
            OnlineMapsUtils.GetCoordinateInDistance(lng, lat, speed * Time.deltaTime / 3600, rotation + 180, out lng, out lat);

            marker.rotationY = rotation;
            marker.SetPosition(lng, lat);
            if (centerOnMarker) map.SetPosition(lng, lat);
            if (rotateCamera) OnlineMapsCameraOrbit.instance.rotation = new Vector2(OnlineMapsCameraOrbit.instance.rotation.x, rotation + 180);
        }
    }
}