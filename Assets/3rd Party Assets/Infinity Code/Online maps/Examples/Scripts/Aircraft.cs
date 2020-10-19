/*         INFINITY CODE         */
/*   https://infinity-code.com   */

using System;
using UnityEngine;

namespace InfinityCode.OnlineMapsDemos
{
    [AddComponentMenu("Infinity Code/Online Maps/Demos/Aircraft")]
    public class Aircraft : MonoBehaviour
    {
        public GameObject container;
        public float altitude = 1000; // meters
        public float speed = 900; // km/h
        public Vector3 cameraOffset = new Vector3(-10, -3, 0);

        public float tiltSpeed = 1;
        public float altitudeChangeSpeed = 100;
        public AnimationCurve altitudeZoomCurve = AnimationCurve.Linear(0, 19, 1, 13);
        public float maxAltitude = 4000; // meters

        private double px, py;
        public float tilt = 0;

        public float rotateSpeed = 1;
        
        private OnlineMaps map;
        private OnlineMapsElevationManagerBase elevationManager;
        private OnlineMapsTileSetControl control;

        private void Start()
        {
            map = OnlineMaps.instance;
            control = OnlineMapsTileSetControl.instance;
            elevationManager = OnlineMapsElevationManagerBase.instance;

            double tlx, tly, brx, bry;
            map.GetCorners(out tlx, out tly, out brx, out bry);

            Vector3 position = control.GetWorldPosition(map.position);
            position.y = altitude;
            if (elevationManager != null) position.y *= OnlineMapsElevationManagerBase.GetBestElevationYScale(tlx, tly, brx, bry) * elevationManager.scale;

            gameObject.transform.position = position;
            map.GetPosition(out px, out py);
        }

        private void Update()
        {
            const float maxTilt = 50;

            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                tilt -= Time.deltaTime * tiltSpeed * maxTilt;
            }
            else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                tilt += Time.deltaTime * tiltSpeed * maxTilt;
            }
            else if (tilt != 0)
            {
                float tiltOffset = Time.deltaTime * tiltSpeed * maxTilt;
                if (Mathf.Abs(tilt) > tiltOffset) tilt -= tiltOffset * Mathf.Sign(tilt);
                else tilt = 0;
            }

            if (Input.GetKey(KeyCode.W))
            {
                altitude += altitudeChangeSpeed * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                altitude -= altitudeChangeSpeed * Time.deltaTime;
            }

            tilt = Mathf.Clamp(tilt, -maxTilt, maxTilt);
            container.transform.localRotation = Quaternion.Euler(tilt, 0, 0);

            if (Math.Abs(tilt) > float.Epsilon)
            {
                transform.Rotate(Vector3.up, tilt * rotateSpeed * Time.deltaTime);
            }

            double tlx, tly, brx, bry, dx, dy;

            map.GetTopLeftPosition(out tlx, out tly);
            map.GetBottomRightPosition(out brx, out bry);

            OnlineMapsUtils.DistanceBetweenPoints(tlx, tly, brx, bry, out dx, out dy);

            double mx = (brx - tlx) / dx;
            double my = (tly - bry) / dy;

            double v = (double)speed * Time.deltaTime / 3600.0;

            double ox = mx * v * Math.Cos(transform.rotation.eulerAngles.y * OnlineMapsUtils.Deg2Rad);
            double oy = my * v * Math.Sin((360 - transform.rotation.eulerAngles.y) * OnlineMapsUtils.Deg2Rad);

            px += ox;
            py += oy;

            map.SetPositionAndZoom(px, py, altitudeZoomCurve.Evaluate(altitude / maxAltitude));

            Vector3 pos = transform.position;
            pos.y = altitude;
            if (elevationManager != null) pos.y *= OnlineMapsElevationManagerBase.GetBestElevationYScale(tlx, tly, brx, bry) * elevationManager.scale;
            transform.position = pos;

            Vector2 distance = OnlineMapsUtils.DistanceBetweenPoints(map.topLeftPosition, map.bottomRightPosition);
            OnlineMapsControlBaseDynamicMesh.instance.sizeInScene = distance * 1000;
            Vector3 d = transform.position - OnlineMapsControlBaseDynamicMesh.instance.center;
            map.transform.position = new Vector3(d.x, map.transform.position.y, d.z);

            Camera.main.transform.position = transform.position - transform.rotation * cameraOffset;
            Camera.main.transform.LookAt(transform);
        }
    }
}