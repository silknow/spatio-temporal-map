/*         INFINITY CODE         */
/*   https://infinity-code.com   */

using UnityEngine;

namespace InfinityCode.OnlineMapsExamples
{
    /// <summary>
    /// Example how to get the color under the cursor
    /// </summary>
    [AddComponentMenu("Infinity Code/Online Maps/Examples (API Usage)/ColorUnderCursorExample")]
    public class ColorUnderCursorExample : MonoBehaviour
    {
        private void Start()
        {
            // Subscribe to OnMapClick
            OnlineMapsControlBase.instance.OnMapClick += OnMapClick;
        }

        /// <summary>
        /// This method is called when the map is clicked.
        /// </summary>
        private void OnMapClick()
        {
            // Get the coordinates under the cursor.
            double lng, lat;
            OnlineMapsControlBase.instance.GetCoords(out lng, out lat);

            // Convert coordinates to tile position
            double tx, ty;
            OnlineMaps.instance.projection.CoordinatesToTile(lng, lat, OnlineMaps.instance.zoom, out tx, out ty);

            // Get tile index
            int itx = (int)tx;
            int ity = (int)ty;

            // Get tile
            OnlineMapsTile tile = OnlineMaps.instance.tileManager.GetTile(OnlineMaps.instance.zoom, itx, ity);

            // If the tile exists, but is not yet loaded, take the parent
            while (tile != null && tile.status != OnlineMapsTileStatus.loaded)
            {
                tile = tile.parent;
                tx /= 2;
                ty /= 2;
            }

            // If the tile does not exist
            if (tile == null)
            {
                Debug.Log("No loaded tiles under cursor");
                return;
            }

            // Calculate the relative position
            double rx = tx - (int)tx;
            double ry = ty - (int)ty;

            // For Target - Tileset
            if (!OnlineMapsControlBase.instance.resultIsTexture)
            {
                Color color = tile.texture.GetPixelBilinear((float)rx, 1 - (float)ry);
                Debug.Log(color);
            }
            // For Target - Texture
            else
            {
                int row = (int)((1 - ry) * OnlineMapsUtils.tileSize);
                Color color = (tile as OnlineMapsRasterTile).colors[(int)((row + rx) * OnlineMapsUtils.tileSize)];
                Debug.Log(color);
            }
        }
    }
}