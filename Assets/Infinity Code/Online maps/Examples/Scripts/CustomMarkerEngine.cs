/*     INFINITY CODE 2013-2019      */
/*   http://www.infinity-code.com   */

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace InfinityCode.OnlineMapsDemos
{
    public class CustomMarkerEngine : MonoBehaviour
    {
        private List<MarkerInstance> markers;

        public RectTransform container;
        public GameObject prefab;
        public MarkerData[] datas;

        private Canvas canvas;
        private OnlineMaps map;
        private OnlineMapsTileSetControl control;

        private Camera worldCamera
        {
            get
            {
                if (canvas.renderMode == RenderMode.ScreenSpaceOverlay) return null;
                return canvas.worldCamera;
            }
        }

        private void OnEnable()
        {
            canvas = container.GetComponentInParent<Canvas>();
        }

        private void SetText(RectTransform rt, string childName, string value)
        {
            Transform child = rt.Find(childName);
            if (child == null) return;

            Text t = child.gameObject.GetComponent<Text>();
            if (t != null) t.text = value;
        }

        private void Start()
        {
            map = OnlineMaps.instance;
            control = OnlineMapsTileSetControl.instance;

            map.OnMapUpdated += UpdateMarkers;
            OnlineMapsCameraOrbit.instance.OnCameraControl += UpdateMarkers;

            markers = new List<MarkerInstance>();

            foreach (MarkerData data in datas)
            {
                GameObject markerGameObject = Instantiate(prefab) as GameObject;
                markerGameObject.name = data.title;
                RectTransform rectTransform = markerGameObject.transform as RectTransform;
                rectTransform.SetParent(container);
                markerGameObject.transform.localScale = Vector3.one;
                MarkerInstance marker = new MarkerInstance();
                marker.data = data;
                marker.gameObject = markerGameObject;
                marker.transform = rectTransform;

                SetText(rectTransform, "Title", data.title);
                SetText(rectTransform, "Population", data.population);

                markers.Add(marker);
            }

            UpdateMarkers();
        }

        private void UpdateMarkers()
        {
            foreach (MarkerInstance marker in markers) UpdateMarker(marker);
        }

        private void UpdateMarker(MarkerInstance marker)
        {
            double px = marker.data.longitude;
            double py = marker.data.latitude;

            Vector2 screenPosition = control.GetScreenPosition(px, py);

            if (screenPosition.x < 0 || screenPosition.x > Screen.width ||
                screenPosition.y < 0 || screenPosition.y > Screen.height)
            {
                marker.gameObject.SetActive(false);
                return;
            }

            RectTransform markerRectTransform = marker.transform;

            if (!marker.gameObject.activeSelf) marker.gameObject.SetActive(true);

            Vector2 point;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(markerRectTransform.parent as RectTransform, screenPosition, worldCamera, out point);
            markerRectTransform.localPosition = point;
        }

        [Serializable]
        public class MarkerData
        {
            public string title;
            public double longitude;
            public double latitude;
            public string population;
        }

        public class MarkerInstance
        {
            public MarkerData data;
            public GameObject gameObject;
            public RectTransform transform;
        }
    }
}