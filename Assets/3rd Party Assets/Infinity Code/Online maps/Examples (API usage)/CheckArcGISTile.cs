/*         INFINITY CODE         */
/*   https://infinity-code.com   */

using UnityEngine;

namespace InfinityCode.OnlineMapsExamples
{
    /// <summary>
    /// Example of how to verify that an ArcGIS tile has been successfully downloaded
    /// </summary>
    [AddComponentMenu("Infinity Code/Online Maps/Examples (API Usage)/CheckArcGISTile")]
    public class CheckArcGISTile : MonoBehaviour
    {
        /// <summary>
        /// Empty tile color
        /// </summary>
        private Color emptyColor = new Color32(204, 204, 204, 255);

        /// <summary>
        /// This method is called for each loaded tile.
        /// </summary>
        /// <param name="tile">The tile that was loaded</param>
        private void OnTileLoaded(OnlineMapsTile tile)
        {
            Debug.Log(tile);
            // Get pixels from texture corners
            Texture2D texture = tile.texture;
            Color c1 = texture.GetPixel(1, 1);
            Color c2 = texture.GetPixel(254, 254);

            // If both colors are empty, the tile is empty
            if (c1 == emptyColor && c2 == emptyColor) tile.status = OnlineMapsTileStatus.error;
        }

        /// <summary>
        /// This method is called when the script starts
        /// </summary>
        private void Start()
        {
            // Subscribe to tile loaded event
            OnlineMapsTileManager.OnTileLoaded += OnTileLoaded;
        }
    }
}