/*         INFINITY CODE         */
/*   https://infinity-code.com   */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace InfinityCode.OnlineMapsExamples
{
    /// <summary>
    /// Example of how to draw baseboards for tileset
    /// </summary>
    [AddComponentMenu("Infinity Code/Online Maps/Examples (API Usage)/MapBaseboards")]
    public class MapBaseboards : MonoBehaviour
    {
        /// <summary>
        /// Side material
        /// </summary>
        public Material sideMaterial;

        /// <summary>
        /// Minimum side height
        /// </summary>
        public float minHeight = 64;

        /// <summary>
        /// Should the bottom be closed?
        /// </summary>
        public bool closeBottom = false;

        /// <summary>
        /// Bottom material
        /// </summary>
        public Material bottomMaterial;

        private OnlineMapsTileSetControl control;
        private Mesh mesh;
        private Mesh mapMesh;

        /// <summary>
        /// This method is called when the script starts
        /// </summary>
        private void Start()
        {
            // Save the reference to control
            control = OnlineMapsTileSetControl.instance;

            // Subscribe to update mesh event
            control.OnMeshUpdated += UpdateSides;

            // Create a new GameObject for baseboards
            GameObject sidesContainer = new GameObject("Sides");
            sidesContainer.transform.parent = control.transform;
            sidesContainer.transform.localPosition = Vector3.zero;
            sidesContainer.transform.localScale = Vector3.one;
            sidesContainer.transform.localRotation = Quaternion.Euler(Vector3.zero);
            MeshFilter meshFilter = sidesContainer.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = sidesContainer.AddComponent<MeshRenderer>();

            // Create the baseboard mesh
            mesh = new Mesh();
            mesh.MarkDynamic();

            // Initialize baseboard materials
            if (!closeBottom)
            {
                meshRenderer.sharedMaterial = sideMaterial;
            }
            else
            {
                meshRenderer.sharedMaterials = new[] {sideMaterial, bottomMaterial};
                mesh.subMeshCount = 2;
            }

            meshFilter.sharedMesh = mesh;
            meshRenderer.material = sideMaterial;
        }

        /// <summary>
        /// This method is called each time the map mesh is updated
        /// </summary>
        private void UpdateSides()
        {
            // Clear baseboard mesh
            mesh.Clear();

            if (mapMesh == null) mapMesh = control.GetComponent<MeshFilter>().sharedMesh;

            // Initialize arrays for vertices
            Vector3[] mapVertices = mapMesh.vertices;

            List<Vector3> left = new List<Vector3>();
            List<Vector3> right = new List<Vector3>();
            List<Vector3> top = new List<Vector3>();
            List<Vector3> bottom = new List<Vector3>();

            float minY = float.MaxValue;

            // Iterate over all map points to find border points
            for (int i = 0; i < mapVertices.Length; i++)
            {
                Vector3 v = mapVertices[i];
                if (Mathf.Abs(v.x) <= 0.01f) left.Add(v);
                if (Mathf.Abs(v.x + control.sizeInScene.x) <= 0.01f) right.Add(v);
                if (Mathf.Abs(v.z) <= 0.01f) top.Add(v);
                if (Mathf.Abs(v.z - control.sizeInScene.y) <= 0.01f) bottom.Add(v);
                if (minY > v.y) minY = v.y;
            }

            // Remove duplicates and sort points of sides
            left = left.Distinct().OrderBy(v => v.z).ToList();
            right = right.Distinct().OrderBy(v => v.z).Reverse().ToList();
            top = top.Distinct().OrderBy(v => v.x).ToList();
            bottom = bottom.Distinct().OrderBy(v => v.x).Reverse().ToList();
            minY -= minHeight;

            // Initialize lists of vertices, uv, normals and trinagles
            List<Vector3> vertices = new List<Vector3>();
            List<Vector2> uv = new List<Vector2>();
            List<Vector3> normals = new List<Vector3>();
            List<int> triangles = new List<int>();

            // Update each side
            UpdateSide(left, vertices, uv, normals, triangles, Vector3.left, minY);
            UpdateSide(top, vertices, uv, normals, triangles, Vector3.forward, minY);
            UpdateSide(right, vertices, uv, normals, triangles, Vector3.right, minY);
            UpdateSide(bottom, vertices, uv, normals, triangles, Vector3.back, minY);

            int countVertices = vertices.Count;

            // Close bottom if necessary
            if (closeBottom)
            {
                vertices.Add(new Vector3(0, minY, 0));
                vertices.Add(new Vector3(0, minY, control.sizeInScene.y));
                vertices.Add(new Vector3(-control.sizeInScene.x, minY, control.sizeInScene.y));
                vertices.Add(new Vector3(-control.sizeInScene.x, minY, 0));

                uv.Add(new Vector2(0, 0));
                uv.Add(new Vector2(0, 1));
                uv.Add(new Vector2(1, 1));
                uv.Add(new Vector2(1, 0));

                normals.Add(Vector3.down);
                normals.Add(Vector3.down);
                normals.Add(Vector3.down);
                normals.Add(Vector3.down);

                mesh.subMeshCount = 2;
            }

            // Set vertices, uv, normals and triangles to the baseboard mesh
            mesh.vertices = vertices.ToArray();
            mesh.uv = uv.ToArray();
            mesh.normals = normals.ToArray();
            mesh.SetTriangles(triangles.ToArray(), 0);

            if (closeBottom)
            {
                mesh.SetTriangles(new[]
                {
                    countVertices,
                    countVertices + 1,
                    countVertices + 2,
                    countVertices,
                    countVertices + 2,
                    countVertices + 3
                }, 1);
            }
        }

        /// <summary>
        /// This method is called to update each side of the baseboard
        /// </summary>
        /// <param name="side">Points of the side</param>
        /// <param name="vertices">Vertices of the baseboard mesh</param>
        /// <param name="uv">UV of the baseboard mesh</param>
        /// <param name="normals">Normals of the baseboard mesh</param>
        /// <param name="triangles">Triangles of the baseboard mesh</param>
        /// <param name="normal">Normal of the side</param>
        /// <param name="minY">Baseboard minimum height</param>
        private static void UpdateSide(List<Vector3> side, List<Vector3> vertices, List<Vector2> uv, List<Vector3> normals, List<int> triangles, Vector3 normal, float minY)
        {
            // Iterate all points of the side, except for the last
            for (int i = 0; i < side.Count - 1; i++)
            {
                int cv = vertices.Count;

                // Get current and next points
                Vector3 p1 = side[i];
                Vector3 p2 = side[i + 1];

                // Add vertices of the polygon
                vertices.Add(p1);
                vertices.Add(p2);
                vertices.Add(new Vector3(p2.x, minY, p2.z));
                vertices.Add(new Vector3(p1.x, minY, p1.z));

                // Add uv of the polygon
                uv.Add(new Vector2(0, 1));
                uv.Add(new Vector2(1, 1));
                uv.Add(new Vector2(1, 0));
                uv.Add(new Vector2(0, 0));

                // Add normals of the polygon
                normals.Add(normal);
                normals.Add(normal);
                normals.Add(normal);
                normals.Add(normal);

                // Add triangles of the polygon
                triangles.Add(cv);
                triangles.Add(cv + 1);
                triangles.Add(cv + 2);
                triangles.Add(cv);
                triangles.Add(cv + 2);
                triangles.Add(cv + 3);
            }
        }
    }
}