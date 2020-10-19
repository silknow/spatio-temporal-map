/*         INFINITY CODE         */
/*   https://infinity-code.com   */

using UnityEngine;

namespace InfinityCode.OnlineMapsDemos
{
    [AddComponentMenu("Infinity Code/Online Maps/Demos/ZoomArea")]
    public class ZoomArea : MonoBehaviour
    {
        public void ZoomIn()
        {
            OnlineMaps.instance.zoom++;
        }

        public void ZoomOut()
        {
            OnlineMaps.instance.zoom--;
        }

        public void SetZoom(int zoom)
        {
            OnlineMaps.instance.zoom = zoom;
        }
    }
}