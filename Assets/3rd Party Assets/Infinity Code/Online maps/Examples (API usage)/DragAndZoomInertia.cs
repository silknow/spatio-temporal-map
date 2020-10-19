/*         INFINITY CODE         */
/*   https://infinity-code.com   */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace InfinityCode.OnlineMapsExamples
{
    /// <summary>
    /// Example of how to make drag and zoom inertia for the map.
    /// </summary>
    [AddComponentMenu("Infinity Code/Online Maps/Examples (API Usage)/DragAndZoomInertia")]
    public class DragAndZoomInertia : MonoBehaviour
    {
        /// <summary>
        /// Deceleration rate (0 - 1).
        /// </summary>
        public float friction = 0.9f;

        private bool isInteract;
        private List<double> speedX;
        private List<double> speedY;
        private List<float> speedZ;
        private double rsX;
        private double rsY;
        private float rsZ;
        private double ptx;
        private double pty;
        private float pz;
        private const int maxSamples = 5;

        private OnlineMaps map;
        private OnlineMapsControlBase control;

        private void FixedUpdate()
        {
            if (isInteract && control.GetTouchCount() == 0) isInteract = false;

            // If there is interaction with the map.
            if (isInteract)
            {
                // Calculates speeds.
                double tx, ty;
                map.GetTilePosition(out tx, out ty, 20);

                double cSpeedX = tx - ptx;
                double cSpeedY = ty - pty;
                float cSpeedZ = map.floatZoom - pz;

                int halfMax = 1 << 19;
                int max = 1 << 20;
                if (cSpeedX > halfMax) cSpeedX -= max;
                else if (cSpeedX < -halfMax) cSpeedX += max;

                while (speedX.Count >= maxSamples) speedX.RemoveAt(0);
                while (speedY.Count >= maxSamples) speedY.RemoveAt(0);
                while (speedZ.Count >= maxSamples) speedZ.RemoveAt(0);

                speedX.Add(cSpeedX);
                speedY.Add(cSpeedY);
                speedZ.Add(cSpeedZ);

                ptx = tx;
                pty = ty;
                pz = map.floatZoom;
            }
            // If no interaction with the map.
            else if (rsX * rsX + rsY * rsY > 0.001 || rsZ > 0.001)
            {
                // Continue to move the map with the current speed.
                double tx, ty;
                map.GetTilePosition(out tx, out ty, 20);

                tx += rsX;
                ty += rsY;

                int max = 1 << 20;
                if (tx >= max) tx -= max;
                else if (tx < 0) tx += max;

                map.SetTilePosition(tx, ty, 20);
                //control.ZoomOnPoint(rsZ, lastScreenPoint);
                map.floatZoom += rsZ;

                // Reduces the current speed.
                rsX *= friction;
                rsY *= friction;
                rsZ *= friction;
            }
        }

        /// <summary>
        /// This method is called when you press on the map.
        /// </summary>
        private void OnMapPress()
        {
            // Get tile coordinates of map
            map.GetTilePosition(out ptx, out pty, 20);
            pz = map.floatZoom;

            // Is marked, that is the interaction with the map.
            isInteract = true;
        }

        /// <summary>
        /// This method is called when you release on the map.
        /// </summary>
        private void OnMapRelease()
        {
            if (control.GetTouchCount() != 0) return;

            // Is marked, that ended the interaction with the map.
            isInteract = false;

            // Calculates the average speed.
            rsX = speedX.Count > 0 ? speedX.Average() : 0;
            rsY = speedY.Count > 0 ? speedY.Average() : 0;
            rsZ = speedZ.Count > 0 ? speedZ.Average() : 0;

            speedX.Clear();
            speedY.Clear();
            speedZ.Clear();
        }


        private void Start()
        {
            map = OnlineMaps.instance; ;
            control = OnlineMapsControlBase.instance;

            // Subscribe to map events
            control.OnMapPress += OnMapPress;
            control.OnMapRelease += OnMapRelease;

            // Initialize arrays of speed
            speedX = new List<double>(maxSamples);
            speedY = new List<double>(maxSamples);
            speedZ = new List<float>(maxSamples);
        }
    }
}