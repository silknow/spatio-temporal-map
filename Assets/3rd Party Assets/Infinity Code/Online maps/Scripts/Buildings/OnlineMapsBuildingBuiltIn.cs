/*         INFINITY CODE         */
/*   https://infinity-code.com   */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Built-in buildings generator.
/// </summary>
[AddComponentMenu("")]
public class OnlineMapsBuildingBuiltIn : OnlineMapsBuildingBase
{
    private static List<OnlineMapsOSMNode> usedNodes;
    private Material wallMaterial;
    private Material roofMaterial;
    private Mesh mesh;

    private static void AnalizeHouseRoofType(OnlineMapsOSMWay way, ref float baseHeight, ref RoofType roofType, ref float roofHeight)
    {
        string roofShape = way.GetTagValue("roof:shape");
        string roofHeightStr = way.GetTagValue("roof:height");
        string minHeightStr = way.GetTagValue("min_height");
        if (!String.IsNullOrEmpty(roofShape))
        {
            if ((roofShape == "dome" || roofShape == "pyramidal") && !String.IsNullOrEmpty(roofHeightStr))
            {
                GetHeightFromString(roofHeightStr, ref roofHeight);
                baseHeight -= roofHeight;
                roofType = RoofType.dome;
            }
        }
        else if (!String.IsNullOrEmpty(roofHeightStr))
        {
            GetHeightFromString(roofHeightStr, ref roofHeight);
            baseHeight -= roofHeight;
            roofType = RoofType.dome;
        }
        else if (!String.IsNullOrEmpty(minHeightStr))
        {
            float totalHeight = baseHeight;
            GetHeightFromString(minHeightStr, ref baseHeight);
            roofHeight = totalHeight - baseHeight;
            roofType = RoofType.dome;
        }
    }

    private static void AnalizeHouseTags(OnlineMapsBuildings container, OnlineMapsOSMWay way, ref Material wallMaterial, ref Material roofMaterial, ref float baseHeight)
    {
        string heightStr = container.useHeightTag? way.GetTagValue("height"): null;
        bool hasHeight = false;

        if (!string.IsNullOrEmpty(heightStr)) hasHeight = GetHeightFromString(heightStr, ref baseHeight);

        if (!hasHeight && container.useHeightTag)
        {
            string levelsStr = way.GetTagValue("building:levels");
            if (!String.IsNullOrEmpty(levelsStr))
            {
                float countLevels;
                if (float.TryParse(levelsStr, NumberStyles.AllowDecimalPoint, OnlineMapsUtils.numberFormat, out countLevels))
                {
                    baseHeight = countLevels * OnlineMapsBuildings.instance.levelHeight;
                    hasHeight = true;
                }
            }
        }

        if (!hasHeight)
        {
            if (OnlineMapsBuildings.instance.OnGenerateBuildingHeight != null) baseHeight = OnlineMapsBuildings.instance.OnGenerateBuildingHeight(way);
            else
            {
                string minHeightStr = container.useHeightTag? way.GetTagValue("min_height"): null;
                if (!string.IsNullOrEmpty(minHeightStr)) GetHeightFromString(minHeightStr, ref baseHeight);
                else baseHeight = Random.Range(OnlineMapsBuildings.instance.levelsRange.min, OnlineMapsBuildings.instance.levelsRange.max) * OnlineMapsBuildings.instance.levelHeight;
            }
        }

        if (baseHeight < OnlineMapsBuildings.instance.minHeight) baseHeight = OnlineMapsBuildings.instance.minHeight;

        if (container.useColorTag)
        {
            string colorStr = way.GetTagValue("building:colour");
            if (!String.IsNullOrEmpty(colorStr)) wallMaterial.color = roofMaterial.color = StringToColor(colorStr);
        }
    }

    /// <summary>
    /// Creates a new building, based on Open Street Map.
    /// </summary>
    /// <param name="container">Reference to OnlineMapsBuildings.</param>
    /// <param name="way">Way of building.</param>
    /// <param name="nodes">Nodes obtained from Open Street Maps.</param>
    /// <returns>Building instance.</returns>
    public static OnlineMapsBuildingBase Create(OnlineMapsBuildings container, OnlineMapsOSMWay way, Dictionary<string, OnlineMapsOSMNode> nodes)
    {
        if (CheckIgnoredBuildings(way)) return null;

        if (usedNodes == null) usedNodes = new List<OnlineMapsOSMNode>(32);
        else usedNodes.Clear();

        way.GetNodes(nodes, usedNodes);

        if (container.OnPrepareBuildingCreation != null) container.OnPrepareBuildingCreation(way, usedNodes);

        List<Vector3> points = GetLocalPoints(usedNodes, container);

        if (points.Count < 3) return null;
        if (points[0] == points[points.Count - 1]) points.RemoveAt(points.Count - 1);
        if (points.Count < 3) return null;

        for (int i = 0; i < points.Count; i++)
        {
            int prev = i - 1;
            if (prev < 0) prev = points.Count - 1;

            int next = i + 1;
            if (next >= points.Count) next = 0;

            float a1 = OnlineMapsUtils.Angle2D(points[prev], points[i]);
            float a2 = OnlineMapsUtils.Angle2D(points[i], points[next]);

            if (Mathf.Abs(a1 - a2) < 5)
            {
                points.RemoveAt(i);
                i--;
            }
        }

        if (points.Count < 3) return null;

        Vector3 centerPoint;

        if (container.OnCalculateBuildingCenter != null)
        {
            centerPoint = container.OnCalculateBuildingCenter(points);
        }
        else
        {
            Vector4 cp = new Vector4(float.MaxValue, float.MaxValue, float.MinValue, float.MinValue);
            for (int i = 0; i < points.Count; i++)
            {
                Vector3 point = points[i];
                if (point.x < cp.x) cp.x = point.x;
                if (point.z < cp.y) cp.y = point.z;
                if (point.x > cp.z) cp.z = point.x;
                if (point.z > cp.w) cp.w = point.z;
            }

            centerPoint = new Vector3((cp.z + cp.x) / 2, 0, (cp.y + cp.w) / 2);
        }

        for (int i = 0; i < points.Count; i++) points[i] -= centerPoint;

        float baseHeight = 15;
        float roofHeight = 0;

        OnlineMapsBuildingMaterial material = GetRandomMaterial(container);
        Vector2 scale = Vector2.one;

        if (defaultShader == null) defaultShader = Shader.Find("Diffuse");

        GameObject houseGO = CreateGameObject(container, way.id);
        MeshRenderer renderer = houseGO.AddComponent<MeshRenderer>();
        MeshFilter meshFilter = houseGO.AddComponent<MeshFilter>();

        OnlineMapsBuildingBuiltIn building = houseGO.AddComponent<OnlineMapsBuildingBuiltIn>();
        building.way = way;
        building.nodes = new List<OnlineMapsOSMNode>(usedNodes);
        houseGO.transform.localPosition = centerPoint;
        houseGO.transform.localRotation = Quaternion.Euler(Vector3.zero);
        houseGO.transform.localScale = Vector3.one;

        if (material != null)
        {
            if (material.wall != null) building.wallMaterial = Instantiate(material.wall) as Material;
            else building.wallMaterial = new Material(defaultShader);

            if (material.roof != null) building.roofMaterial = Instantiate(material.roof) as Material;
            else building.roofMaterial = new Material(defaultShader);

            scale = material.scale;
        }
        else
        {
            if (defaultWallMaterial == null) defaultWallMaterial = new Material(defaultShader);
            if (defaultRoofMaterial == null) defaultRoofMaterial = new Material(defaultShader);
            building.wallMaterial = Instantiate(defaultWallMaterial) as Material; 
            building.roofMaterial = Instantiate(defaultRoofMaterial) as Material;
        }

        RoofType roofType = RoofType.flat;
        AnalizeHouseTags(container, way, ref building.wallMaterial, ref building.roofMaterial, ref baseHeight);
        AnalizeHouseRoofType(way, ref baseHeight, ref roofType, ref roofHeight);

        building.mesh = new Mesh {name = way.id};

        meshFilter.sharedMesh = building.mesh;
        renderer.sharedMaterials = new []
        {
            building.wallMaterial,
            building.roofMaterial
        };

        Vector2 centerCoords = Vector2.zero;
        float minCX = float.MaxValue, minCY = float.MaxValue, maxCX = float.MinValue, maxCY = float.MinValue;

        foreach (OnlineMapsOSMNode node in usedNodes)
        {
            Vector2 nodeCoords = node;
            centerCoords += nodeCoords;
            if (nodeCoords.x < minCX) minCX = nodeCoords.x;
            if (nodeCoords.y < minCY) minCY = nodeCoords.y;
            if (nodeCoords.x > maxCX) maxCX = nodeCoords.x;
            if (nodeCoords.y > maxCY) maxCY = nodeCoords.y;
        }

        building.container = container;
        building.id = way.id;
        building.initialZoom = container.map.buffer.lastState.floatZoom;
        building.initialSizeInScene = container.control.sizeInScene;
        building.centerCoordinates = new Vector2((maxCX + minCX) / 2, (maxCY + minCY) / 2);
        building.boundsCoords = new Bounds(building.centerCoordinates, new Vector3(maxCX - minCX, maxCY - minCY));

        int wallVerticesCount = (points.Count + 1) * 2;
        int roofVerticesCount = points.Count;
        int verticesCount = wallVerticesCount + roofVerticesCount;
        int countTriangles = wallVerticesCount * 3;

        if (vertices == null) vertices = new List<Vector3>(verticesCount);
        else vertices.Clear();

        if (uvs == null) uvs = new List<Vector2>(verticesCount);
        else uvs.Clear();

        if (wallTriangles == null) wallTriangles = new List<int>(countTriangles);
        else wallTriangles.Clear();

        if (roofTriangles == null) roofTriangles = new List<int>();
        else roofTriangles.Clear();

        double tlx, tly, brx, bry;
        container.map.buffer.GetCorners(out tlx, out tly, out brx, out bry);
        baseHeight *= OnlineMapsElevationManagerBase.GetBestElevationYScale(tlx, tly, brx, bry);

        building.CreateHouseWall(points, baseHeight, building.wallMaterial, scale);
        building.CreateHouseRoof(points, baseHeight, roofHeight, roofType);

        if (building.hasErrors)
        {
            OnlineMapsUtils.Destroy(building.gameObject);
            return null;
        }

        building.mesh.vertices = vertices.ToArray();
        building.mesh.uv = uvs.ToArray();
        building.mesh.subMeshCount = 2;
        building.mesh.SetTriangles(wallTriangles.ToArray(), 0);
        building.mesh.SetTriangles(roofTriangles.ToArray(), 1);

        building.mesh.RecalculateBounds();
        building.mesh.RecalculateNormals();

        if (container.generateColliders)
        {
            building.buildingCollider = houseGO.AddComponent<MeshCollider>();
            (building.buildingCollider as MeshCollider).sharedMesh = building.mesh;
        }

        return building;
    }

    private void CreateHouseRoof(List<Vector3> baseVertices, float baseHeight, float roofHeight, RoofType roofType)
    {
        float[] roofPoints = new float[baseVertices.Count * 2];

        if (roofVertices == null) roofVertices = new List<Vector3>(baseVertices.Count);
        else roofVertices.Clear();

        try
        {
            int countVertices = CreateHouseRoofVerticles(baseVertices, roofVertices, roofPoints, baseHeight);
            CreateHouseRoofTriangles(countVertices, roofVertices, roofType, roofPoints, baseHeight, roofHeight, ref roofTriangles);

            if (roofTriangles.Count == 0)
            {
                hasErrors = true;
                return;
            }

            Vector3 side1 = roofVertices[roofTriangles[1]] - roofVertices[roofTriangles[0]];
            Vector3 side2 = roofVertices[roofTriangles[2]] - roofVertices[roofTriangles[0]];
            Vector3 perp = Vector3.Cross(side1, side2);

            bool reversed = perp.y < 0;
            if (reversed) roofTriangles.Reverse();

            float minX = float.MaxValue;
            float minZ = float.MaxValue;
            float maxX = float.MinValue;
            float maxZ = float.MinValue;

            for (int i = 0; i < roofVertices.Count; i++)
            {
                Vector3 v = roofVertices[i];
                if (v.x < minX) minX = v.x;
                if (v.z < minZ) minZ = v.z;
                if (v.x > maxX) maxX = v.x;
                if (v.z > maxZ) maxZ = v.z;
            }

            float offX = maxX - minX;
            float offZ = maxZ - minZ;

            for (int i = 0; i < roofVertices.Count; i++)
            {
                Vector3 v = roofVertices[i];
                uvs.Add(new Vector2((v.x - minX) / offX, (v.z - minZ) / offZ));
            }

            int triangleOffset = vertices.Count;
            for (int i = 0; i < roofTriangles.Count; i++) roofTriangles[i] += triangleOffset;

            vertices.AddRange(roofVertices);
        }
        catch (Exception)
        {
            Debug.Log(roofTriangles.Count + "   " + roofVertices.Count);
            hasErrors = true;
            throw;
        }
    }

    private static void CreateHouseRoofDome(float height, List<Vector3> vertices, List<int> triangles)
    {
        Vector3 roofTopPoint = Vector3.zero;
        roofTopPoint = vertices.Aggregate(roofTopPoint, (current, point) => current + point) / vertices.Count;
        roofTopPoint.y = height;
        int vIndex = vertices.Count;

        for (int i = 0; i < vertices.Count; i++)
        {
            int p1 = i;
            int p2 = i + 1;
            if (p2 >= vertices.Count) p2 -= vertices.Count;

            triangles.AddRange(new[] { p1, p2, vIndex });
        }

        vertices.Add(roofTopPoint);
    }

    private static void CreateHouseRoofTriangles(int countVertices, List<Vector3> vertices, RoofType roofType, float[] roofPoints, float baseHeight, float roofHeight, ref List<int> triangles)
    {
        if (roofType == RoofType.flat)
        {
            if (roofIndices == null) roofIndices = new List<int>(60);
            triangles.AddRange(OnlineMapsUtils.Triangulate(roofPoints, countVertices, roofIndices));
        }
        else if (roofType == RoofType.dome) CreateHouseRoofDome(baseHeight + roofHeight, vertices, triangles);
    }

    private static int CreateHouseRoofVerticles(List<Vector3> baseVertices, List<Vector3> verticles, float[] roofPoints, float baseHeight)
    {
        float topPoint = baseHeight;
        int countVertices = 0;

        for (int i = 0; i < baseVertices.Count; i++)
        {
            Vector3 p = baseVertices[i];
            float px = p.x;
            float pz = p.z;

            bool hasVerticle = false;

            for (int j = 0; j < countVertices * 2; j += 2)
            {
                if (Math.Abs(roofPoints[j] - px) < float.Epsilon && Math.Abs(roofPoints[j + 1] - pz) < float.Epsilon)
                {
                    hasVerticle = true;
                    break;
                }
            }

            if (!hasVerticle)
            {
                int cv2 = countVertices * 2;

                roofPoints[cv2] = px;
                roofPoints[cv2 + 1] = pz;
                verticles.Add(new Vector3(px, topPoint, pz));

                countVertices++;
            }
        }

        return countVertices;
    }

    private void CreateHouseWall(List<Vector3> baseVertices, float baseHeight, Material material, Vector2 materialScale)
    {
        CreateHouseWallMesh(baseVertices, baseHeight, false);

        Vector2 scale = material.mainTextureScale;
        scale.x *= perimeter / 100 * materialScale.x;
        scale.y *= baseHeight / 30 * materialScale.y;
        material.mainTextureScale = scale;
    }

    private void CreateHouseWallMesh(List<Vector3> baseVertices, float baseHeight, bool inverted)
    {
        bool reversed = CreateHouseWallVerticles(baseHeight, baseVertices, vertices, uvs);
        if (inverted) reversed = !reversed;
        CreateHouseWallTriangles(vertices, reversed);
    }

    private static void CreateHouseWallTriangles(List<Vector3> vertices, bool reversed)
    {
        int countVertices = vertices.Count;
        for (int i = 0; i < countVertices / 4; i++)
        {
            int p1 = i * 4;
            int p2 = p1 + 2;
            int p3 = p2 + 1;
            int p4 = p1 + 1;

            if (p2 >= countVertices) p2 -= countVertices;
            if (p3 >= countVertices) p3 -= countVertices;

            if (reversed)
            {
                wallTriangles.Add(p1);
                wallTriangles.Add(p4);
                wallTriangles.Add(p3);
                wallTriangles.Add(p1);
                wallTriangles.Add(p3);
                wallTriangles.Add(p2);
            }
            else
            {
                wallTriangles.Add(p2);
                wallTriangles.Add(p3);
                wallTriangles.Add(p1);
                wallTriangles.Add(p3);
                wallTriangles.Add(p4);
                wallTriangles.Add(p1);
            }
        }
    }

    private bool CreateHouseWallVerticles(float baseHeight, List<Vector3> baseVertices, List<Vector3> vertices, List<Vector2> uvs)
    {
        float topPoint = baseHeight;

        int baseVerticesCount = baseVertices.Count;
        Vector3 pp = Vector3.zero;
        Vector3 ptv = Vector3.zero;

        for (int i = 0; i <= baseVerticesCount; i++)
        {
            int j = i;
            if (j >= baseVerticesCount) j -= baseVerticesCount;

            Vector3 p = baseVertices[j];
            Vector3 tv = new Vector3(p.x, topPoint, p.z);

            if (i > 0)
            {
                vertices.Add(pp);
                vertices.Add(ptv);

                vertices.Add(p);
                vertices.Add(tv);
            }

            pp = p;
            ptv = tv;
        }

        float currentDistance = 0;
        int countVertices = vertices.Count;
        int fourthVerticesCount = countVertices / 4;
        perimeter = 0;

        for (int i = 0; i < fourthVerticesCount; i++)
        {
            int i1 = i * 4;
            int i2 = i * 4 + 2;

            float magnitude = (vertices[i1] - vertices[i2]).magnitude;
            perimeter += magnitude;
        }

        float prevDistance = 0;

        for (int i = 0; i < fourthVerticesCount; i++)
        {
            int i1 = i * 4;
            int i2 = i * 4 + 2;
            
            float magnitude = (vertices[i1] - vertices[i2]).magnitude;

            float prevU = prevDistance / perimeter;

            currentDistance += magnitude;
            prevDistance = currentDistance;

            float curU = currentDistance / perimeter;
            uvs.Add(new Vector2(prevU, 0));
            uvs.Add(new Vector2(prevU, 1));
            uvs.Add(new Vector2(curU, 0));
            uvs.Add(new Vector2(curU, 1));
        }

        int southIndex = -1;
        float southZ = float.MaxValue;

        for (int i = 0; i < baseVerticesCount; i++)
        {
            if (baseVertices[i].z < southZ)
            {
                southZ = baseVertices[i].z;
                southIndex = i;
            }
        }

        int prevIndex = southIndex - 1;
        if (prevIndex < 0) prevIndex = baseVerticesCount - 1;

        int nextIndex = southIndex + 1;
        if (nextIndex >= baseVerticesCount) nextIndex = 0;

        float angle1 = OnlineMapsUtils.Angle2D(baseVertices[southIndex], baseVertices[nextIndex]);
        float angle2 = OnlineMapsUtils.Angle2D(baseVertices[southIndex], baseVertices[prevIndex]);

        return angle1 < angle2;
    }

    public override void Dispose()
    {
        base.Dispose();

        if (way != null) way.Dispose();
        way = null;

        if (nodes != null) foreach (OnlineMapsOSMNode node in nodes) node.Dispose();
        nodes = null;

        OnlineMapsUtils.Destroy(wallMaterial);
        OnlineMapsUtils.Destroy(roofMaterial);
        OnlineMapsUtils.Destroy(mesh);
    }

    private static OnlineMapsBuildingMaterial GetRandomMaterial(OnlineMapsBuildings container)
    {
        if (container.materials == null || container.materials.Length == 0) return null;

        return container.materials[Random.Range(0, container.materials.Length)];
    }

    public static Color StringToColor(string str)
    {
        str = str.ToLower();
        if (str == "black") return Color.black;
        if (str == "blue") return Color.blue;
        if (str == "cyan") return Color.cyan;
        if (str == "gray") return Color.gray;
        if (str == "green") return Color.green;
        if (str == "magenta") return Color.magenta;
        if (str == "red") return Color.red;
        if (str == "white") return Color.white;
        if (str == "yellow") return Color.yellow;

        try
        {
            int len = str.Length;
            if (len > 7) len = 7;
            byte prevB = 0;
            byte[] rgb = new byte[3];
            int j = 0;

            for (int i = 1; i < len; i++)
            {
                char c = i < len ? str[i] : '0';
                byte b = (byte) (c - (c < 58 ? 48 : 87));
                if (i % 2 == 1) prevB = b;
                else rgb[j++] = (byte)(prevB * 16 + b);
            }

            return new Color32(rgb[0], rgb[1], rgb[2], 255);
        }
        catch (Exception exception)
        {
            Debug.Log(exception.Message + "\n" + exception.StackTrace);
            return Color.white;
        }
    }
}