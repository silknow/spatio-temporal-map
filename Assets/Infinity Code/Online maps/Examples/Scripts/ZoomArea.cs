/*     INFINITY CODE 2013-2019      */
/*   http://www.infinity-code.com   */

using UnityEngine;

namespace InfinityCode.OnlineMapsDemos
{
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