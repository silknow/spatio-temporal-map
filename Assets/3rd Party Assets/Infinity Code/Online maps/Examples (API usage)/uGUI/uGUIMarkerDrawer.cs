/*         INFINITY CODE         */
/*   https://infinity-code.com   */

using UnityEngine;

namespace InfinityCode.OnlineMapsExamples
{
    /// <summary>
    /// Example how to create your own marker drawer for uGUI
    /// </summary>
    [AddComponentMenu("Infinity Code/Online Maps/Examples (API Usage)/uGUIMarkerDrawer")]
    public class uGUIMarkerDrawer : MonoBehaviour
    {
        private static uGUIMarkerDrawer instance;

        /// <summary>
        /// Reference to container of the markers
        /// </summary>
        public RectTransform container;

        /// <summary>
        /// Prefab of the marker
        /// </summary>
        public GameObject prefab;

        /// <summary>
        /// Reference to the canvas
        /// </summary>
        private Canvas canvas;

        /// <summary>
        /// Gets a world camera
        /// </summary>
        private Camera worldCamera
        {
            get
            {
                if (canvas.renderMode == RenderMode.ScreenSpaceOverlay) return null;
                return canvas.worldCamera;
            }
        }

        /// <summary>
        /// This method is called each time the script is enabled
        /// </summary>
        private void OnEnable()
        {
            // Save the references to this drawer and canvas
            instance = this;
            canvas = container.GetComponentInParent<Canvas>();
        }

        /// <summary>
        /// This method is called when the script starts
        /// </summary>
        private void Start()
        {
            // Create and register a new marker drawer
            OnlineMapsControlBase.instance.markerDrawer = new Drawer(OnlineMapsControlBase.instance);
        }

        /// <summary>
        /// Drawer of the markers on canvas
        /// </summary>
        public class Drawer : OnlineMapsMarker2DDrawer
        {
            /// <summary>
            /// Reference to Control
            /// </summary>
            private OnlineMapsControlBase control;

            /// <summary>
            /// Constructor of drawer
            /// </summary>
            /// <param name="control">Reference to Control</param>
            public Drawer(OnlineMapsControlBase control)
            {
                this.control = control;

                // Subscribe to draw markers event
                control.OnDrawMarkers += OnDrawMarkers;
            }

            /// <summary>
            /// Dispose the drawer
            /// </summary>
            public override void Dispose()
            {
                // Unsubscribe from draw marker event
                control.OnDrawMarkers -= OnDrawMarkers;

                // Clear the reference
                control = null;
            }

            /// <summary>
            /// This method is called when drawing markers
            /// </summary>
            private void OnDrawMarkers()
            {
                // Get corners of the map
                double tlx, tly, brx, bry;
                map.GetCorners(out tlx, out tly, out brx, out bry);

                // Draw each markers
                foreach (OnlineMapsMarker marker in OnlineMapsMarkerManager.instance.items)
                {
                    DrawMarker(marker, tlx, tly, brx, bry);
                }
            }

            /// <summary>
            /// This method is called to draw each marker
            /// </summary>
            /// <param name="marker">Marker</param>
            /// <param name="tlx">Left longitude of the map</param>
            /// <param name="tly">Top latitude of the map</param>
            /// <param name="brx">Right longitude of the map</param>
            /// <param name="bry">Bottom latitiude of the map</param>
            private void DrawMarker(OnlineMapsMarker marker, double tlx, double tly, double brx, double bry)
            {
                // Get coordinates of the marker
                double px, py;
                marker.GetPosition(out px, out py);

                // Get instance of marker from custom data
                GameObject markerInstance = marker["instance"] as GameObject;

                // If marker outside the map
                if (px < tlx || px > brx || py < bry || py > tly)
                {
                    // If there is an instance, destroy it
                    if (markerInstance != null)
                    {
                        OnlineMapsUtils.Destroy(markerInstance);
                        marker["instance"] = null;
                    }

                    return;
                }

                // If there is no instance, create it and put the reference to custom data
                if (markerInstance == null)
                {
                    marker["instance"] = markerInstance = Instantiate(instance.prefab);
                    (markerInstance.transform as RectTransform).SetParent(instance.container);
                    markerInstance.transform.localScale = Vector3.one;
                }

                // Convert geographic coordinates to screen position
                Vector2 screenPosition = control.GetScreenPosition(px, py);

                // Get rect transform of the instance
                RectTransform markerRectTransform = markerInstance.transform as RectTransform;

                // Add half height to align the marker to the bottom
                screenPosition.y += markerRectTransform.rect.height / 2;

                // Convert screen space to local space in the canvas
                Vector2 point;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(markerRectTransform.parent as RectTransform, screenPosition, instance.worldCamera, out point);

                // Set position of the marker instance
                markerRectTransform.localPosition = point;
            }
        }
    }
}