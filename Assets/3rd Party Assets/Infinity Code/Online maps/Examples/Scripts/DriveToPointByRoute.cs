/*         INFINITY CODE         */
/*   https://infinity-code.com   */

using System;
using UnityEngine;

namespace InfinityCode.OnlineMapsDemos
{
    [AddComponentMenu("Infinity Code/Online Maps/Demos/DriveToPointByRoute")]
    public class DriveToPointByRoute : MonoBehaviour
    {
        public GameObject prefab;
        public float markerScale = 5f;

        public GameObject targetPrefab;
        public float targetScale = 5f;

        public float speed;
        public float rotation = 0;

        private OnlineMaps map;
        private OnlineMapsTileSetControl control;
        private OnlineMapsMarker3D marker;
        private OnlineMapsMarker3D targetMarker;
        private double lng, lat;
        private double targetLng, targetLat;
        private OnlineMapsVector2d[] points;
        private int pointIndex = -1;
        private double progress;
        private OnlineMapsDrawingLine route;
        private float targetRotation;

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

            if (!OnlineMapsKeyManager.hasGoogleMaps)
            {
                Debug.LogWarning("Please enter Map / Key Manager / Google Maps");
                return;
            }

            OnlineMapsGoogleDirections request = new OnlineMapsGoogleDirections(OnlineMapsKeyManager.GoogleMaps(), new Vector2((float)lng, (float)lat), control.GetCoords());
            request.OnComplete += OnRequestComplete;
            request.Send();
        }

        private void OnRequestComplete(string response)
        {
            OnlineMapsGoogleDirectionsResult result = OnlineMapsGoogleDirections.GetResult(response);
            if (result == null || result.routes.Length == 0)
            {
                Debug.Log("No result");
                return;
            }

            points = result.routes[0].overview_polylineD;
            if (route == null)
            {
                route = new OnlineMapsDrawingLine(points, Color.green, 3);
                OnlineMapsDrawingElementManager.AddItem(route);
            }
            else route.points = points;

            pointIndex = 0;
        }

        private void Update()
        {
            if (pointIndex == -1) return;

            // Start point
            OnlineMapsVector2d p1 = points[pointIndex];

            // End point
            OnlineMapsVector2d p2 = points[pointIndex + 1];

            double p1x, p1y, p2x, p2y;
            map.projection.CoordinatesToTile(p1.x, p1.y, map.zoom, out p1x, out p1y);
            map.projection.CoordinatesToTile(p2.x, p2.y, map.zoom, out p2x, out p2y);

            // Total step distance
            double dx, dy;
            OnlineMapsUtils.DistanceBetweenPoints(p1.x, p1.y, p2.x, p2.y, out dx, out dy);
            double stepDistance = Math.Sqrt(dx * dx + dy * dy);

            // Total step time
            double totalTime = stepDistance / speed * 3600;

            // Current step progress
            progress += Time.deltaTime / totalTime;

            OnlineMapsVector2d position;

            if (progress < 1)
            {
                position = OnlineMapsVector2d.Lerp(p1, p2, progress);
                marker.SetPosition(position.x, position.y);

                // Orient marker
                targetRotation = (float) OnlineMapsUtils.Angle2D(p1x, p1y, p2x, p2y) - 90;
            }
            else
            {
                position = p2;
                marker.SetPosition(position.x, position.y);
                pointIndex++;
                progress = 0;
                if (pointIndex >= points.Length - 1)
                {
                    Debug.Log("Finish");
                    pointIndex = -1;
                }
                else
                {
                    OnlineMapsVector2d p3 = points[pointIndex + 1];
                    map.projection.CoordinatesToTile(p2.x, p2.y, map.zoom, out p1x, out p1y);
                    map.projection.CoordinatesToTile(p3.x, p3.y, map.zoom, out p2x, out p2y);

                    targetRotation = (float) OnlineMapsUtils.Angle2D(p1x, p1y, p2x, p2y) - 90;
                }
            }

            marker.rotationY = Mathf.LerpAngle(marker.rotationY, targetRotation, Time.deltaTime * 10);
            marker.GetPosition(out lng, out lat);
            map.SetPosition(lng, lat);
        }
    }
}