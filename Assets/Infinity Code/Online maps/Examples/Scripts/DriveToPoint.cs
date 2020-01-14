/*     INFINITY CODE 2013-2019      */
/*   http://www.infinity-code.com   */

using System;
using UnityEngine;

namespace InfinityCode.OnlineMapsDemos
{
    [AddComponentMenu("Infinity Code/Online Maps/Demos/DriveToPoint")]
    public class DriveToPoint : MonoBehaviour
    {
        public GameObject prefab;
        public float markerScale = 5f;

        public GameObject targetPrefab;
        public float targetScale = 5f;

        public float speed;
        public float maxSpeed = 160;
        public float rotation = 0;
        public bool centerOnMarker = true;

        public Material lineRendererMaterial;
        public float lineRendererDuration = 0.5f;

        private OnlineMaps map;
        private OnlineMapsTileSetControl control;
        private OnlineMapsMarker3D marker;
        private OnlineMapsMarker3D targetMarker;
        private double lng, lat;
        private double targetLng, targetLat;
        private bool hasTargetPoint;
        private LineRenderer lineRenderer;
        private float lineRendererProgress = 1;

        private void Start()
        {
            map = OnlineMaps.instance;
            control = OnlineMapsTileSetControl.instance;

            control.OnMapClick += OnMapClick;

            map.GetPosition(out lng, out lat);

            marker = OnlineMapsMarker3DManager.CreateItem(lng, lat, prefab);
            marker.scale = markerScale;
            marker.rotationY = rotation;
        }

        private void OnMapClick()
        {
            control.GetCoords(out targetLng, out targetLat);

            if (targetMarker == null)
            {
                targetMarker = OnlineMapsMarker3DManager.CreateItem(targetLng, targetLat, targetPrefab);
                targetMarker.scale = targetScale;
            }
            else targetMarker.SetPosition(targetLng, targetLat);

            double tx1, ty1, tx2, ty2;
            map.projection.CoordinatesToTile(lng, lat, map.zoom, out tx1, out ty1);
            map.projection.CoordinatesToTile(targetLng, targetLat, map.zoom, out tx2, out ty2);

            rotation = (float) OnlineMapsUtils.Angle2D(tx1, ty1, tx2, ty2) - 90;
            hasTargetPoint = true;

            if (lineRenderer == null)
            {
                GameObject go = new GameObject("LineRenderer");
                go.transform.SetParent(transform, false);
                lineRenderer = go.AddComponent<LineRenderer>();
                lineRenderer.material = lineRendererMaterial;
#if UNITY_2017_3_OR_NEWER
                lineRenderer.positionCount = 2;
                lineRenderer.widthCurve = AnimationCurve.Constant(0, 1, 10);
#elif UNITY_2017_1_OR_NEWER
                lineRenderer.positionCount = 2;
                lineRenderer.widthCurve = AnimationCurve.Linear(0, 10, 1, 10);
#else
                lineRenderer.SetVertexCount(2);
                lineRenderer.SetWidth(10, 10);
#endif
            }
            else lineRenderer.enabled = true;

            Vector3 p1 = control.GetWorldPosition(lng, lat);
            lineRenderer.SetPosition(0, p1);
            lineRenderer.SetPosition(1, p1);

            lineRendererProgress = 0;
        }

        private void Update()
        {
            if (!hasTargetPoint) return;

            double dx, dy;
            OnlineMapsUtils.DistanceBetweenPoints(lng, lat, targetLng, targetLat, out dx, out dy);

            double distance = Math.Sqrt(dx * dx + dy * dy);
            float cMaxSpeed = maxSpeed;
            if (distance < 0.1) cMaxSpeed = maxSpeed * (float) (distance / 0.1);

            speed = Mathf.Lerp(speed, cMaxSpeed, Time.deltaTime);

            OnlineMapsUtils.GetCoordinateInDistance(lng, lat, speed * Time.deltaTime / 3600, rotation + 180, out lng, out lat);

            OnlineMapsUtils.DistanceBetweenPoints(lng, lat, targetLng, targetLat, out dx, out dy);
            if (Math.Sqrt(dx * dx + dy * dy) < 0.001)
            {
                hasTargetPoint = false;
                speed = 0;
            }

            marker.rotationY = rotation;
            marker.SetPosition(lng, lat);
            if (centerOnMarker) map.SetPosition(lng, lat);

            if (lineRendererProgress < 1)
            {
                lineRendererProgress += Time.deltaTime / lineRendererDuration;

                Vector3 p1 = control.GetWorldPosition(lng, lat);
                Vector3 p2 = control.GetWorldPosition(targetLng, targetLat);
                Vector3 p3 = lineRendererProgress > 0.5 ? Vector3.Lerp(p1, p2, (lineRendererProgress - 0.5f) * 2f) : p1;
                Vector3 p4 = lineRendererProgress < 0.5 ? Vector3.Lerp(p1, p2, lineRendererProgress * 2) : p2;
                lineRenderer.SetPosition(0, p4);
                lineRenderer.SetPosition(1, p3);
            }
            else
            {
                lineRendererProgress = 1;
                if (lineRenderer.enabled) lineRenderer.enabled = false;
            }
        }
    }
}