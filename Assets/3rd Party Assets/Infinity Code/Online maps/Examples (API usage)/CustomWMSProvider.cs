/*         INFINITY CODE         */
/*   https://infinity-code.com   */

using UnityEngine;

namespace InfinityCode.OnlineMapsExamples
{
    /// <summary>
    /// Example of how to download a tile from WMS that does not support tiles.
    /// Important: if you have a chance to make Tiled WMS - use it, because this way because this path is quite heavy.
    /// </summary>
    [AddComponentMenu("Infinity Code/Online Maps/Examples (API Usage)/CustomWMSProvider")]
    public class CustomWMSProvider : MonoBehaviour
    {
        /// <summary>
        /// URL pattern for your server
        /// </summary>
        public string url = "http://192.168.0.1:8080/geoserver/tuzla/wms?LAYERS=tuzla&STYLES=&FORMAT=image%2Fpng&SERVICE=WMS&VERSION=1.1.1&REQUEST=GetMap&SRS=EPSG%3A4326&BBOX={lx},{by},{rx},{ty}&WIDTH=256&HEIGHT=256";

        /// <summary>
        /// This method will be called for each token in url
        /// </summary>
        /// <param name="tile">The tile for which url is generated</param>
        /// <param name="token">Token to be processed</param>
        /// <returns>Value for token</returns>
        private string OnReplaceUrlToken(OnlineMapsTile tile, string token)
        {
            // If it is a corner token, return a value
            if (token == "ty") return tile.topLeft.y.ToString(OnlineMapsUtils.numberFormat);
            if (token == "by") return tile.bottomRight.y.ToString(OnlineMapsUtils.numberFormat);
            if (token == "lx") return tile.topLeft.x.ToString(OnlineMapsUtils.numberFormat);
            if (token == "rx") return tile.bottomRight.x.ToString(OnlineMapsUtils.numberFormat);

            // Otherwise, return the token
            return token;
        }

        /// <summary>
        /// This method is called when the script starts
        /// </summary>
        private void Start()
        {
            // Register a new provider and map type
            OnlineMapsProvider.Create("mywms").AppendTypes(
                new OnlineMapsProvider.MapType("style1") {urlWithLabels = url}
            );

            // Select a new map type
            OnlineMaps.instance.mapType = "mywms.style1";

            // Subscribe to replace token event
            OnlineMapsTile.OnReplaceURLToken += OnReplaceUrlToken;

            OnlineMaps.instance.SetPositionAndZoom(29.254738, 40.8027188, 14);
        }
    }
}