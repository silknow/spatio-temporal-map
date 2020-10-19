/*         INFINITY CODE         */
/*   https://infinity-code.com   */

using UnityEngine;

namespace InfinityCode.OnlineMapsExamples
{
    /// <summary>
    /// Example of interception requests to download tiles
    /// </summary>
    [AddComponentMenu("Infinity Code/Online Maps/Examples (API Usage)/CustomDownloadTileExample")]
    public class CustomDownloadTileExample : MonoBehaviour
    {
        private OnlineMaps map;

        private void Start()
        {
            map = OnlineMaps.instance;

            // Subscribe to the tile download event.
            OnlineMapsTileManager.OnStartDownloadTile += OnStartDownloadTile;
        }

        private void OnStartDownloadTile(OnlineMapsTile tile)
        {
            // Note: create a texture only when you are sure that the tile exists.
            // Otherwise, you will get a memory leak.
            Texture2D tileTexture = new Texture2D(256, 256);

            // Here your code to load tile texture from any source.
            
            // Note: If the tile will load asynchronously, set
            // tile.status = OnlineMapsTileStatus.loading;
            // Otherwise, the map will try to load the tile again and again.

            // Apply your texture in the buffer and redraws the map.
            if (map.control.resultIsTexture)
            {
                // Apply tile texture
                (tile as OnlineMapsRasterTile).ApplyTexture(tileTexture as Texture2D);

                // Send tile to buffer
                map.buffer.ApplyTile(tile);

                // Destroy the texture, because it is no longer needed.
                OnlineMapsUtils.Destroy(tileTexture);
            }
            else
            {
                // Send tile texture
                tile.texture = tileTexture;

                // Change tile status
                tile.status = OnlineMapsTileStatus.loaded;
            }

            // Note: If the tile does not exist or an error occurred, set
            // tile.status = OnlineMapsTileStatus.error;
            // Otherwise, the map will try to load the tile again and again.

            // Redraw map (using best redraw type)
            map.Redraw();
        }
    }
}