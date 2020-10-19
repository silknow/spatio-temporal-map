/*         INFINITY CODE         */
/*   https://infinity-code.com   */

using UnityEngine;

namespace InfinityCode.OnlineMapsExamples
{
    /// <summary>
    /// How to make the map follow GameObject
    /// </summary>
    [AddComponentMenu("Infinity Code/Online Maps/Examples (API Usage)/FollowGameObject")]
    public class FollowGameObject : MonoBehaviour
    {
        /// <summary>
        /// GameObject to be followed by the map
        /// </summary>
        public GameObject target;

        /// <summary>
        /// Last position of GameObject
        /// </summary>
        private Vector3 lastPosition;

        /// <summary>
        /// Reference to the map
        /// </summary>
        private OnlineMaps map;

        /// <summary>
        /// Reference to the control
        /// </summary>
        private OnlineMapsTileSetControl control;

        /// <summary>
        /// Last tile position of the center of the map
        /// </summary>
        private double tx, ty;

        private void Start()
        {
            // Set a reference to the map and control
            map = OnlineMaps.instance;
            control = OnlineMapsTileSetControl.instance;

            // Disable the movement and zoom of the map with the mouse
            control.allowUserControl = false;
            control.allowZoom = false;

            // Subscribe to change zoom event
            map.OnChangeZoom += OnChangeZoom;

            // Store tile position of the center of the map
            map.GetTilePosition(out tx, out ty);

            // Initial update the map
            UpdateMap();
        }

        /// <summary>
        /// This method is called when the zoom changes (in this case, using a script or inspector)
        /// </summary>
        private void OnChangeZoom()
        {
            // Store tile position of the center of the map
            map.GetTilePosition(out tx, out ty);
        }

        private void Update()
        {
            // If GameObject position has changed, update the map
            if (target.transform.position != lastPosition) UpdateMap();
        }

        /// <summary>
        /// Updates map position
        /// </summary>
        private void UpdateMap()
        {
            // Store last position of GameObject
            lastPosition = target.transform.position;

            // Size of map in scene
            Vector2 size = control.sizeInScene;

            // Calculate offset (in tile position)
            Vector3 offset = map.transform.rotation * (lastPosition - map.transform.position - control.center);
            offset.x = offset.x / OnlineMapsUtils.tileSize / size.x * map.width * map.zoomCoof;
            offset.z = offset.z / OnlineMapsUtils.tileSize / size.y * map.height * map.zoomCoof;

            // Calculate current tile position of the center of the map
            tx -= offset.x;
            ty += offset.z;

            // Set position of the map center
            if (Mathf.Abs(offset.x) > float.Epsilon || Mathf.Abs(offset.z) > float.Epsilon) map.SetTilePosition(tx, ty);

            // Update map GameObject position
            map.transform.position = lastPosition - control.center;
        }
    }
}