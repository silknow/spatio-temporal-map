/*         INFINITY CODE         */
/*   https://infinity-code.com   */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace InfinityCode.OnlineMapsDemos
{
    [AddComponentMenu("Infinity Code/Online Maps/Demos/HorizonWithoutElevation")]
    public class HorizonWithoutElevation : MonoBehaviour
    {
        /// <summary>
        /// Number of tiles horizontally (X axis).
        /// </summary>
        [Tooltip("Number of tiles horizontally (X axis).")]
        public int countX = 3;

        /// <summary>
        /// Number of tiles vertically (Z axis).
        /// </summary>
        [Tooltip("Number of tiles vertically (Z axis).")]
        public int countY = 3;

        /// <summary>
        /// Offset of the horizon mesh along the y-axis relative to the map. Keep it negative.
        /// </summary>
        [Tooltip("Offset of the horizon mesh along the y-axis relative to the map. Keep it negative.")]
        public float positionYOffset = -5;

        /// <summary>
        /// Offset zoom of tiles relative to map.zoom. Keep positive.
        /// </summary>
        [Tooltip("Offset zoom of tiles relative to map.zoom. Keep positive.")]
        public int zoomOffset = 3;

        /// <summary>
        /// Shader of the tiles mesh.
        /// </summary>
        [Tooltip("Shader of the tiles mesh.")] public Shader shader;

        /// <summary>
        /// Offset of the render queue relative to render queue of the shader.
        /// </summary>
        [Tooltip("Offset of the render queue relative to render queue of the shader.")]
        public int renderQueueOffset = 100;

        private int resolution = 2;

        private OnlineMapsTile[] tiles;
        private List<OnlineMapsTile> extraTiles;
        private Mesh mesh;
        private Vector3[] vertices;
        private Vector3[] normals;
        private Vector2[] uv;
        private int[] triangles;
        private OnlineMaps map;
        private OnlineMapsTileSetControl control;
        private MeshRenderer meshRenderer;

        private Vector2 ctl;
        private Vector2 cbr;

        private void InitMesh()
        {
            GameObject go = new GameObject("Horizon");
            go.transform.parent = map.gameObject.transform;
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.Euler(Vector3.zero);
            go.transform.localScale = Vector3.one;

            MeshFilter meshFilter = go.AddComponent<MeshFilter>();
            meshRenderer = go.AddComponent<MeshRenderer>();
            int countTiles = countX * countY;

            mesh = new Mesh();
            mesh.name = "Horizon";
            mesh.MarkDynamic();

            meshFilter.sharedMesh = mesh;

            tiles = new OnlineMapsTile[countX * countY];
            extraTiles = new List<OnlineMapsTile>(countX * 2 + countY * 2 + 4);

            int countVertices = (countX + 1) * (countY + 1) * resolution * resolution;
            vertices = new Vector3[countVertices];
            normals = new Vector3[countVertices];
            uv = new Vector2[countVertices];
            triangles = new int[6 * resolution * resolution];

            mesh.vertices = vertices;
            mesh.subMeshCount = countTiles;
            float r1 = resolution - 1;

            int index = 0;
            for (int i = 0; i < (countX + 1) * (countY + 1); i++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    for (int y = 0; y < resolution; y++)
                    {
                        normals[index] = Vector3.up;
                        uv[index++] = new Vector2(x / r1, 1 - y / r1);
                    }
                }
            }

            mesh.uv = uv;
            mesh.normals = normals;

            Material[] materials = new Material[countTiles];

            for (int i = 0; i < countTiles; i++)
            {
                int ti = 0;
                for (int x = 0; x < resolution - 1; x++)
                {
                    for (int y = 0; y < resolution - 1; y++)
                    {
                        int vi = i * resolution * resolution + x * resolution + y;
                        triangles[ti] = vi;
                        triangles[ti + 1] = vi + resolution + 1;
                        triangles[ti + 2] = vi + 1;
                        triangles[ti + 3] = vi;
                        triangles[ti + 4] = vi + resolution;
                        triangles[ti + 5] = vi + resolution + 1;
                        ti += 6;
                    }
                }

                mesh.SetTriangles(triangles, i);

                Material material = new Material(shader);
                material.renderQueue = shader.renderQueue + renderQueueOffset;
                materials[i] = material;
            }

            meshRenderer.sharedMaterials = materials;

            UpdateMesh();

            mesh.RecalculateBounds();
        }

        private void OnTileDownloaded(OnlineMapsTile tile)
        {
            for (int i = 0; i < countX * countY; i++)
            {
                if (tiles[i] == tile)
                {
                    meshRenderer.sharedMaterials[i].mainTexture = tile.texture;
                }
            }
        }

        private void PreloadBorders()
        {
            int zoom = map.zoom - zoomOffset;
            if (zoom < 3) zoom = 3;

            double tx, ty;
            map.GetTilePosition(out tx, out ty, zoom);

            int itx = Mathf.RoundToInt((float) (tx - countX / 2f));
            int ity = Mathf.RoundToInt((float) (ty - countY / 2f));

            foreach (OnlineMapsTile tile in extraTiles) tile.Unblock(this);
            extraTiles.Clear();

            int max = 1 << zoom;
            OnlineMapsTileManager tileManager = map.tileManager;

            for (int x = -1; x < countX + 1; x++)
            {
                int tileX = itx + x;
                if (tileX >= max) tileX -= max;

                int tileY = ity - 1;
                if (tileY >= max) tileY -= max;

                OnlineMapsTile tile = tileManager.GetTile(zoom, tileX, tileY);
                if (tile == null)
                {
                    OnlineMapsTile parentTile = tileManager.GetTile(zoom - 1, tileX / 2, tileY / 2);
                    tile = new OnlineMapsRasterTile(tileX, tileY, zoom, map);
                    tile.parent = parentTile;
                }

                tile.Block(this);
                extraTiles.Add(tile);

                tileY = ity + countY;
                if (tileY >= max) tileY -= max;

                tile = tileManager.GetTile(zoom, tileX, tileY);
                if (tile == null)
                {
                    OnlineMapsTile parentTile = tileManager.GetTile(zoom - 1, tileX / 2, tileY / 2);
                    tile = new OnlineMapsRasterTile(tileX, tileY, zoom, map);
                    tile.parent = parentTile;
                }

                tile.Block(this);
                extraTiles.Add(tile);
            }

            for (int y = 0; y < countY; y++)
            {
                int tileY = ity + y;
                if (tileY >= max) tileY -= max;

                int tileX = itx - 1;
                if (tileX >= max) tileX -= max;

                OnlineMapsTile tile = tileManager.GetTile(zoom, tileX, tileY);
                if (tile == null)
                {
                    OnlineMapsTile parentTile = tileManager.GetTile(zoom - 1, tileX / 2, tileY / 2);
                    tile = new OnlineMapsRasterTile(tileX, tileY, zoom, map);
                    tile.parent = parentTile;
                }

                tile.Block(this);
                extraTiles.Add(tile);

                tileX = itx + countX;
                if (tileX >= max) tileX -= max;

                tile = tileManager.GetTile(zoom, tileX, tileY);
                if (tile == null)
                {
                    OnlineMapsTile parentTile = tileManager.GetTile(zoom - 1, tileX / 2, tileY / 2);
                    tile = new OnlineMapsRasterTile(tileX, tileY, zoom, map);
                    tile.parent = parentTile;
                }

                tile.Block(this);
                extraTiles.Add(tile);
            }
        }

        private void Start()
        {
            if (zoomOffset <= 0) throw new Exception("Zoom offset should be positive.");
            if (shader == null) shader = Shader.Find("Diffuse");

            map = OnlineMaps.instance;
            control = OnlineMapsTileSetControl.instance;
            OnlineMapsTile.OnTileDownloaded += OnTileDownloaded;
            if (OnlineMapsCache.instance != null)
            {
                OnlineMapsCache.instance.OnLoadedFromCache += OnTileDownloaded;
            }

            map.OnMapUpdated += UpdateMesh;

            InitMesh();
        }

        private void UpdateMesh()
        {
            int zoom = map.zoom - zoomOffset;
            if (zoom < 3) zoom = 3;

            for (int i = 0; i < tiles.Length; i++)
                if (tiles[i] != null)
                    tiles[i].Unblock(this);

            double tx, ty;
            map.GetTilePosition(out tx, out ty, zoom);

            int itx = Mathf.RoundToInt((float) (tx - countX / 2f));
            int ity = Mathf.RoundToInt((float) (ty - countY / 2f));

            Vector3 offset = new Vector3(0, positionYOffset, 0) - transform.position;

            int max = 1 << zoom;
            Material[] materials = meshRenderer.sharedMaterials;

            float r1 = resolution - 1;
            int vi = 0;

            for (int x = 0; x < countX; x++)
            {
                int tileX = itx + x;
                int nextTileX = tileX + 1;
                if (tileX >= max) tileX -= max;
                if (nextTileX >= max) nextTileX -= max;

                for (int y = 0; y < countY; y++)
                {
                    int tileY = ity + y;
                    int nextTileY = tileY + 1;

                    if (tileY >= max) tileY -= max;
                    if (nextTileY >= max) nextTileY -= max;

                    OnlineMapsTile tile = map.tileManager.GetTile(zoom, tileX, tileY);
                    if (tile == null)
                    {
                        OnlineMapsTile parentTile = map.tileManager.GetTile(zoom - 1, tileX / 2, tileY / 2);
                        tile = new OnlineMapsRasterTile(tileX, tileY, zoom, map);
                        tile.parent = parentTile;
                    }

                    int tileIndex = x * countY + y;
                    tiles[tileIndex] = tile;
                    tile.Block(this);

                    double px, py;

                    map.projection.TileToCoordinates(tileX, tileY, zoom, out px, out py);
                    Vector3 v1 = control.GetWorldPosition(px, py) + offset;

                    map.projection.TileToCoordinates(nextTileX, nextTileY, zoom, out px, out py);
                    Vector3 v2 = control.GetWorldPosition(px, py) + offset;
                    Vector3 ov = (v2 - v1) / r1;

                    float uvScale = 1;
                    Vector2 uvOffset = Vector2.zero;

                    if (tile.texture == null)
                    {
                        while (tile.parent != null)
                        {
                            tile = tile.parent;

                            int s = 1 << (zoom - tile.zoom);

                            uvScale = 1f / s;
                            uvOffset.x = tileX % s * uvScale;
                            uvOffset.y = (s - tileY % s - 1) * uvScale;

                            if (tile.texture != null) break;
                        }
                    }

                    for (int vx = 0; vx < resolution; vx++)
                    {
                        for (int vz = 0; vz < resolution; vz++)
                        {
                            Vector3 v = new Vector3(ov.x * vx + v1.x, 0, ov.z * vz + v1.z);
                            v.y = positionYOffset;
                            uv[vi] = new Vector2((vx / r1) * uvScale + uvOffset.x, (1 - vz / r1) * uvScale + uvOffset.y);
                            vertices[vi++] = v;
                        }
                    }

                    materials[tileIndex].mainTexture = tile.texture;
                    materials[tileIndex].color = new Color(1, 1, 1, tile.texture != null ? 1 : 0);
                }
            }

            mesh.vertices = vertices;
            mesh.uv = uv;

            PreloadBorders();
        }
    }
}