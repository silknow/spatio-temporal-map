/*         INFINITY CODE         */
/*   https://infinity-code.com   */

using System;
using UnityEngine;
using UnityEngine.UI;

namespace InfinityCode.OnlineMapsDemos
{
    /// <summary>
    /// Example is how to use a combination of data from Google Places API on bubble popup.
    /// </summary>
    [AddComponentMenu("Infinity Code/Online Maps/Demos/UIBubblePopup")]
    public class UIBubblePopup : MonoBehaviour
    {
        /// <summary>
        /// Root canvas
        /// </summary>
        public Canvas canvas;

        /// <summary>
        /// Bubble popup
        /// </summary>
        public GameObject bubble;

        /// <summary>
        /// Title text
        /// </summary>
        public Text title;

        /// <summary>
        /// Address text
        /// </summary>
        public Text address;

        /// <summary>
        /// Photo RawImage
        /// </summary>
        public RawImage photo;

        public CData[] datas;

        /// <summary>
        /// Reference to active marker
        /// </summary>
        private OnlineMapsMarker targetMarker;

        /// <summary>
        /// This method is called when downloading photo texture.
        /// </summary>
        /// <param name="texture2D">Photo texture</param>
        private void OnDownloadPhotoComplete(OnlineMapsWWW www)
        {
            Texture2D texture = new Texture2D(1, 1);
            www.LoadImageIntoTexture(texture);

            // Set place texture to bubble popup
            photo.texture = texture;
        }

        /// <summary>
        /// This method is called by clicking on the map
        /// </summary>
        private void OnMapClick()
        {
            // Remove active marker reference
            targetMarker = null;

            // Hide the popup
            bubble.SetActive(false);
        }

        /// <summary>
        /// This method is called by clicking on the marker
        /// </summary>
        /// <param name="marker">The marker on which clicked</param>
        private void OnMarkerClick(OnlineMapsMarkerBase marker)
        {
            // Set active marker reference
            targetMarker = marker as OnlineMapsMarker;

            // Get a result item from instance of the marker
            CData data = marker["data"] as CData;
            if (data == null) return;

            // Show the popup
            bubble.SetActive(true);

            // Set title and address
            title.text = data.title;
            address.text = data.address;

            // Destroy the previous photo
            if (photo.texture != null)
            {
                OnlineMapsUtils.Destroy(photo.texture);
                photo.texture = null;
            }

            OnlineMapsWWW www = new OnlineMapsWWW(data.photo_url);
            www.OnComplete += OnDownloadPhotoComplete;

            // Initial update position
            UpdateBubblePosition();
        }

        /// <summary>
        /// Start is called on the frame when a script is enabled just before any of the Update methods are called the first time.
        /// </summary>
        private void Start()
        {
            // Subscribe to events of the map 
            OnlineMaps.instance.OnChangePosition += UpdateBubblePosition;
            OnlineMaps.instance.OnChangeZoom += UpdateBubblePosition;
            OnlineMapsControlBase.instance.OnMapClick += OnMapClick;

            if (OnlineMapsControlBaseDynamicMesh.instance != null)
            {
                OnlineMapsControlBaseDynamicMesh.instance.OnMeshUpdated += UpdateBubblePosition;
            }

            if (OnlineMapsCameraOrbit.instance != null)
            {
                OnlineMapsCameraOrbit.instance.OnCameraControl += UpdateBubblePosition;
            }

            if (datas != null)
            {
                foreach (CData data in datas)
                {
                    OnlineMapsMarker marker = OnlineMapsMarkerManager.CreateItem(data.longitude, data.latitude);
                    marker["data"] = data;
                    marker.OnClick += OnMarkerClick;
                }
            }


            // Initial hide popup
            bubble.SetActive(false);
        }

        /// <summary>
        /// Updates the popup position
        /// </summary>
        private void UpdateBubblePosition()
        {
            // If no marker is selected then exit.
            if (targetMarker == null) return;

            // Hide the popup if the marker is outside the map view
            if (!targetMarker.inMapView)
            {
                if (bubble.activeSelf) bubble.SetActive(false);
            }
            else if (!bubble.activeSelf) bubble.SetActive(true);

            // Convert the coordinates of the marker to the screen position.
            Vector2 screenPosition = OnlineMapsControlBase.instance.GetScreenPosition(targetMarker.position);

            // Add marker height
            screenPosition.y += targetMarker.height;

            // Get a local position inside the canvas.
            Vector2 point;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, screenPosition, null, out point);

            // Set local position of the popup
            (bubble.transform as RectTransform).localPosition = point;
        }

        [Serializable]
        public class CData
        {
            public string title;
            public string address;
            public string photo_url;
            public double longitude;
            public double latitude;
        }
    }
}