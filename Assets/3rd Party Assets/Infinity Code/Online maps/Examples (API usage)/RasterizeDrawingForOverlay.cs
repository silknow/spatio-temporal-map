/*         INFINITY CODE         */
/*   https://infinity-code.com   */

using System.Collections;
using UnityEngine;

#if !UNITY_WEBGL || UNITY_EDITOR
using System.IO;
#endif

namespace InfinityCode.OnlineMapsExamples
{
    /// <summary>
    /// Example of how to rasterize drawing elements so that later it can be used as overlay
    /// </summary>
    [AddComponentMenu("Infinity Code/Online Maps/Examples (API Usage)/RasterizeDrawingForOverlay")]
    public class RasterizeDrawingForOverlay : MonoBehaviour
    {
        /// <summary>
        /// Minimum zoom for rasterization
        /// </summary>
        public int zoomFrom = 1;

        /// <summary>
        /// Maximum zoom for rasterization
        /// </summary>
        public int zoomTo = 10;

        private bool started = false;
        private IEnumerator routine;
        private Texture2D texture;
        private Color32[] colors;
        private OnlineMaps map;

        /// <summary>
        /// Grows the Bounds to include the point
        /// </summary>
        /// <param name="lx">Left longitude</param>
        /// <param name="ty">Top latitude</param>
        /// <param name="rx">Right longitude</param>
        /// <param name="by">Bottom latitude</param>
        /// <param name="px">Point longitude</param>
        /// <param name="py">Point latitude</param>
        private static void Encapsulate(ref double lx, ref double ty, ref double rx, ref double by, double px, double py)
        {
            if (px < lx) lx = px;
            if (px > rx) rx = px;

            if (py > ty) ty = py;
            if (py < by) by = py;
        }

        /// <summary>
        /// Clear references and update asset database
        /// </summary>
        private void Finish()
        {
            Destroy(texture);
            texture = null;
            colors = null;
            map = null;

#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
        }

        /// <summary>
        /// Get boundaries of drawing elements
        /// </summary>
        /// <param name="lx">Left longitude</param>
        /// <param name="ty">Top latitude</param>
        /// <param name="rx">Right longitude</param>
        /// <param name="by">Bottom latitude</param>
        private void GetBounds(out double lx, out double ty, out double rx, out double by)
        {
            // Initialize boundaries
            lx = double.MaxValue;
            ty = double.MinValue;
            rx = double.MinValue;
            by = double.MaxValue;

            // Iterate each drawing element
            foreach (var el in OnlineMapsDrawingElementManager.instance)
            {
                // If it is a rectangle, update the boundaries using corners
                if (el is OnlineMapsDrawingRect)
                {
                    OnlineMapsDrawingRect rect = el as OnlineMapsDrawingRect;
                    Encapsulate(ref lx, ref ty, ref rx, ref by, rect.x, rect.y);
                    Encapsulate(ref lx, ref ty, ref rx, ref by, rect.x + rect.width, rect.y + rect.height);
                    continue;
                }

                // Get the points of drawing element
                IEnumerable points;
                if (el is OnlineMapsDrawingLine) points = (el as OnlineMapsDrawingLine).points;
                else if (el is OnlineMapsDrawingPoly) points = (el as OnlineMapsDrawingPoly).points;
                else continue;

                int valueType = -1; // 0 - Vector2, 1 - float, 2 - double, 3 - OnlineMapsVector2d
                object v1 = null;
                int i = -1;
                double ppx = 0;

                // Iterate each point
                foreach (object p in points)
                {
                    i++;

                    // If the type of the value is unknown, check by value
                    if (valueType == -1)
                    {
                        if (p is Vector2) valueType = 0;
                        else if (p is float) valueType = 1;
                        else if (p is double) valueType = 2;
                        else if (p is OnlineMapsVector2d) valueType = 3;
                    }

                    object v2 = v1;
                    v1 = p;

                    double px = 0;
                    double py = 0;

                    if (valueType == 0)
                    {
                        if (i == 0)
                        {
                            Vector2 p1 = (Vector2) v1;
                            ppx = p1.x;
                            Encapsulate(ref lx, ref ty, ref rx, ref by, p1.x, p1.y);
                            continue;
                        }

                        Vector2 v = (Vector2) v1;
                        px = v.x;
                        py = v.y;
                    }
                    else if (valueType == 3)
                    {
                        if (i == 0)
                        {
                            OnlineMapsVector2d p1 = (OnlineMapsVector2d) v1;
                            ppx = p1.x;
                            Encapsulate(ref lx, ref ty, ref rx, ref by, p1.x, p1.y);
                            continue;
                        }

                        Vector2 v = (OnlineMapsVector2d) v1;
                        px = v.x;
                        py = v.y;
                    }
                    else if (i % 2 == 1)
                    {
                        if (i == 1)
                        {
                            if (valueType == 1)
                            {
                                ppx = (float) v2;
                                Encapsulate(ref lx, ref ty, ref rx, ref by, ppx, (float) v1);
                            }
                            else
                            {
                                ppx = (double) v2;
                                Encapsulate(ref lx, ref ty, ref rx, ref by, ppx, (double) v1);
                            }

                            continue;
                        }

                        if (valueType == 1)
                        {
                            px = (float) v2;
                            py = (float) v1;
                        }
                        else
                        {
                            px = (double) v2;
                            py = (double) v1;
                        }
                    }

                    while (true)
                    {
                        double ox = px - ppx;

                        if (ox > 180) px -= 360;
                        else if (ox < -180) px += 360;
                        else break;
                    }

                    Encapsulate(ref lx, ref ty, ref rx, ref by, px, py);

                    ppx = px;
                }
            }
        }

        /// <summary>
        /// Draws UI elements using IMGUI
        /// </summary>
        private void OnGUI()
        {
            if (!started)
            {
                if (GUILayout.Button("Rasterize"))
                {
                    // Start rasterization
                    routine = Rasterize();
                    StartCoroutine(routine);
                }
            }
            else
            {
                if (GUILayout.Button("Cancel"))
                {
                    // Stop rasterization and finalize
                    StopCoroutine(routine);
                    Finish();
                }
            }
        }

        /// <summary>
        /// Rasterizes all drawing elements in the specified zoom range
        /// </summary>
        /// <returns>IEnumerator for Coroutine</returns>
        public IEnumerator Rasterize()
        {
            // If there is no drawing elements, return
            if (OnlineMapsDrawingElementManager.CountItems == 0) yield break;

            // Get boundaries of drawing elements
            double lx, ty, rx, by;
            GetBounds(out lx, out ty, out rx, out by);

            map = OnlineMaps.instance;

            // Initialize a temporary texture and color array
            texture = new Texture2D(256, 256, TextureFormat.ARGB32, false);
            colors = new Color32[256 * 256];

            // Iterate zoom levels
            for (int z = zoomFrom; z <= zoomTo; z++)
            {
                // Calculate number of tiles in zoom level
                int max = 1 << z;

                // Convert coordinates of boundaries to tile position
                double tlx, tty, trx, tby;
                map.projection.CoordinatesToTile(lx, ty, z, out tlx, out tty);
                map.projection.CoordinatesToTile(rx, by, z, out trx, out tby);

                int ilx = (int) tlx;
                int ity = (int) tty;
                int irx = (int) trx;
                int iby = (int) tby;

                // Checking the transition through 180 meridian
                if (ilx > irx) irx += max;

                // Iterate x position
                for (int x = ilx; x <= irx; x++)
                {
                    int cx = x;

                    // Checking the transition through 180 meridian
                    if (cx >= max) cx -= max;

                    // Iterate y position
                    for (int y = ity; y <= iby; y++)
                    {
                        // Rasterize a tile
                        yield return RasterizeTile(z, cx, y);
                    }
                }
            }

            // Finalize and update asset database
            Finish();
        }

        /// <summary>
        /// This method is called to rasterize each tile
        /// </summary>
        /// <param name="zoom">Tile Zoom</param>
        /// <param name="x">Tile X</param>
        /// <param name="y">Tile Y</param>
        /// <returns>IEnumerator for Coroutine</returns>
        private IEnumerator RasterizeTile(int zoom, int x, int y)
        {
            // One frame delay
            yield return null;

            // Clear color array
            Color32 empty = new Color32(255, 255, 255, 0);
            for (int i = 0; i < 256 * 256; i++) colors[i] = empty;

            Vector2 bufferPosition = new Vector2(x, y);

            // Iterate each drawing element and draw it into color buffer
            foreach (OnlineMapsDrawingElement el in map.control.drawingElementManager)
            {
                el.Draw(colors, bufferPosition, 256, 256, zoom);
            }

            // Check empty tile
            bool hasColor = false;
            for (int i = 0; i < 256 * 256; i++)
            {
                if (colors[i].a != 0)
                {
                    hasColor = true;
                    break;
                }
            }

            // If the tile has no colors, ignore it
            if (!hasColor)
            {
                Debug.Log("Ignore " + zoom + "/" + x + "/" + y);
                yield break;
            }

            // Set colors to texture
            texture.SetPixels32(colors);
            texture.Apply(false);

            // Encode texture to png
            byte[] bytes = texture.EncodeToPNG();

            // Save file in Resources / DrawingTiles
            string path = Application.dataPath + "/Resources/DrawingTiles/" + zoom + "/" + x + "/" + y + ".png";
            Debug.Log(path);

#if !UNITY_WEBGL || UNITY_EDITOR
            FileInfo info = new FileInfo(path);
            if (!info.Directory.Exists) info.Directory.Create();
            File.WriteAllBytes(path, bytes);
#endif
        }
    }
}