using System;
using System.Collections.Generic;
using UnityEngine;

public class Clustering2DMarkers : MonoBehaviour
{
    public static Clustering2DMarkers instance;

    public static Action<Cluster, OnlineMapsMarker> OnCreateInstance;

    public Texture2D groupTexture;
    public int clusterToZoom = OnlineMaps.MAXZOOM;
    public int minClusteredItems = 2;

    private static Cluster _rootCluster;
    private static OnlineMaps map;
    private static OnlineMapsProjection projection;

    private static int mapTileX;
    private static int mapTileY;
    private static List<OnlineMapsMarker> markers;
    private static List<OnlineMapsMarker> unclusteredMarkers;
    private static bool inited;
    private static bool needUpdate;

    public static Cluster rootCluster
    {
        get
        {
            if (!inited) Init();
            return _rootCluster;
        }
    }

    public static ClusterItem Add(OnlineMapsMarker marker)
    {
        needUpdate = true;
        return rootCluster.Add(marker);
    }

    public static void AddRange(IEnumerable<OnlineMapsMarker> markers)
    {
        foreach (OnlineMapsMarker marker in markers) rootCluster.Add(marker);
        needUpdate = true;
    }

    public static void AddUnclustered(OnlineMapsMarker marker)
    {
        unclusteredMarkers.Add(marker);
        needUpdate = true;
    }

    private static void GetCorners(out double tlx, out double tly, out double brx, out double bry)
    {
        int countX = map.width / OnlineMapsUtils.tileSize;
        int countY = map.height / OnlineMapsUtils.tileSize;

        double lng, lat;
        map.GetPosition(out lng, out lat);
        int zoom = map.zoom;
        projection.CoordinatesToTile(lng, lat, zoom, out tlx, out tly);

        float hcx = countX / 2f + 1;
        float hcy = countY / 2f + 1;

        brx = tlx + hcx;
        bry = tly + hcy;
        tlx -= hcx;
        tly -= hcy;

        projection.TileToCoordinates(tlx, tly, zoom, out tlx, out tly);
        projection.TileToCoordinates(brx, bry, zoom, out brx, out bry);
    }

    private static void Init()
    {
        _rootCluster = new Cluster(1);
        map = OnlineMaps.instance;
        projection = map.projection;
        map.OnChangePosition += OnChangePosition;
        map.OnChangeZoom += OnChangeZoom;

        inited = true;
    }

    private void LateUpdate()
    {
        if (needUpdate) UpdateMarkers();
    }

    private static void OnChangePosition()
    {
        double tx, ty;
        map.GetTilePosition(out tx, out ty);
        if (mapTileX != (int)tx || mapTileY != (int)ty) needUpdate = true;
    }

    private static void OnChangeZoom()
    {
        needUpdate = true;
    }

    private void OnEnable()
    {
        instance = this;
        unclusteredMarkers = new List<OnlineMapsMarker>();
    }

    private void Start()
    {
        if (!inited) Init();
    }

    public static void Remove(OnlineMapsMarker marker)
    {
        rootCluster.Remove(marker);
        needUpdate = true;
    }

    public static void Remove(IEnumerable<OnlineMapsMarker> markers)
    {
        foreach (OnlineMapsMarker marker in markers) rootCluster.Remove(marker);
        needUpdate = true;
    }

    public static void RemoveAll()
    {
        rootCluster.RemoveAll();
        unclusteredMarkers.Clear();
        needUpdate = true;
    }

    public static void RemoveUnclustered(OnlineMapsMarker marker)
    {
        unclusteredMarkers.Remove(marker);
        needUpdate = true;
    }

    public static void UpdateMarkers()
    {
        double tlx, tly, brx, bry;
        GetCorners(out tlx, out tly, out brx, out bry);

        if (markers == null) markers = new List<OnlineMapsMarker>();
        rootCluster.GetMarkers(tlx, tly, brx, bry, ref markers);
        markers.AddRange(unclusteredMarkers);

        OnlineMapsMarker[] prevMarkers = OnlineMapsMarkerManager.instance.items.ToArray();
        OnlineMapsMarker[] newMarkers = markers.ToArray();
        markers.Clear();

        for (int i = 0; i < prevMarkers.Length; i++)
        {
            OnlineMapsMarker m = prevMarkers[i];

            for (int j = 0; j < newMarkers.Length; j++)
            {
                OnlineMapsMarker m2 = newMarkers[j];
                if (m == m2) break;
            }
        }

        OnlineMapsMarkerManager.SetItems(newMarkers);

        double tx, ty;
        map.GetTilePosition(out tx, out ty);
        mapTileX = (int)tx;
        mapTileY = (int)ty;
        needUpdate = false;
    }

    public static void UpdateMarkerPosition(OnlineMapsMarker marker)
    {
        rootCluster.UpdateMarker(marker);
        needUpdate = true;
    }

    public static void UpdatePositions()
    {
        rootCluster.UpdatePositions();
    }

    public class Cluster : ClusterItem
    {
        public ClusterItem[] childs;
        public int count;
        public int capacity;
        internal int zoom;
        private int totalCount;
        public double? mx;
        public double? my;

        public Cluster(int zoom)
        {
            childs = new ClusterItem[2];
            capacity = 2;
            this.zoom = zoom;
        }

        public Cluster(int zoom, MarkerWrapper marker1, MarkerWrapper marker2) : this(zoom)
        {
            totalCount = 2;
            marker1.GetTilePosition(zoom, out tileX, out tileY);
            projection.TileToCoordinates(tileX + 0.5, tileY + 0.5, zoom, out longitude, out latitude);

            if (zoom < instance.clusterToZoom)
            {
                int mx1, my1, mx2, my2;
                marker1.GetTilePosition(zoom + 1, out mx1, out my1);
                marker2.GetTilePosition(zoom + 1, out mx2, out my2);

                if (mx1 == mx2 && my1 == my2)
                {
                    AddChild(new Cluster(zoom + 1, marker1, marker2));
                }
                else
                {
                    AddChild(marker1);
                    AddChild(marker2);
                }
            }
            else
            {
                AddChild(marker1);
                AddChild(marker2);
            }
        }

        public ClusterItem Add(OnlineMapsMarker marker)
        {
            return Add(new MarkerWrapper(marker));
        }

        public ClusterItem Add(MarkerWrapper marker)
        {
            totalCount++;
            if (zoom < instance.clusterToZoom)
            {
                int mx, my;
                int z = zoom + 1;
                marker.GetTilePosition(z, out mx, out my);
                for (int i = 0; i < count; i++)
                {
                    ClusterItem item = childs[i];
                    if (item.CompareTiles(z, mx, my))
                    {
                        if (item is Cluster) (item as Cluster).Add(marker);
                        else
                        {
                            Cluster c = new Cluster(z, item as MarkerWrapper, marker) { parent = this };
                            childs[i] = c;
                        }
                        return item;
                    }
                }
            }

            AddChild(marker);
            return this;
        }

        private void AddChild(ClusterItem child)
        {
            if (childs == null)
            {
                childs = new ClusterItem[2];
                capacity = 2;
            }
            if (count == capacity)
            {
                capacity *= 2;
                Array.Resize(ref childs, capacity);
            }

            child.parent = this;
            childs[count] = child;
            count++;
        }

        public override bool CompareTiles(int z, int tx, int ty)
        {
            return tx == tileX && ty == tileY;
        }

        public override void Dispose()
        {
            childs = null;
            parent = null;
            count = 0;
            capacity = 0;
            totalCount = 0;

            if (markerRef != null)
            {
                //OnlineMapsUtils.DestroyImmediate(markerRef.instance);
                markerRef = null;
            }
        }

        public MarkerWrapper FindMarkerWrapper(OnlineMapsMarker marker)
        {
            for (int i = 0; i < count; i++)
            {
                ClusterItem item = childs[i];
                if (item is Cluster)
                {
                    MarkerWrapper wrapper = (item as Cluster).FindMarkerWrapper(marker);
                    if (wrapper != null) return wrapper;
                }
                else if (item.markerRef == marker)
                {
                    return item as MarkerWrapper;
                }
            }

            return null;
        }

        public void GetMarkers(double tlx, double tly, double brx, double bry, ref List<OnlineMapsMarker> markers)
        {
            TryGetMarkers(map.zoom, tlx, tly, brx, bry, ref markers);
        }

        public override void GetMarkers(int z, double tlx, double tly, double brx, double bry, ref List<OnlineMapsMarker> markers)
        {
            if (InRange(tlx, tly, brx, bry)) TryGetMarkers(z, tlx, tly, brx, bry, ref markers);
        }

        public override void GetMercatorPosition(out double mx, out double my)
        {
            if (!this.mx.HasValue || !this.my.HasValue) UpdatePosition();

            mx = this.mx.Value;
            my = this.my.Value;
        }

        public bool InRange(double tlx, double tly, double brx, double bry)
        {
            if (zoom <= 3) return true;

            double tx1, ty1, tx2, ty2;
            projection.CoordinatesToTile(tlx, tly, zoom, out tx1, out ty1);
            projection.CoordinatesToTile(brx, bry, zoom, out tx2, out ty2);

            int itx1 = (int)tx1;
            int itx2 = (int)tx2;
            int ity1 = (int)ty1;
            int ity2 = (int)ty2;

            if (itx1 > itx2)
            {
                int maxX = 1 << zoom;
                itx2 += maxX;
                if (itx2 - tileX > maxX)
                {
                    itx1 -= maxX;
                    itx2 -= maxX;
                }
            }

            return itx1 <= tileX && itx2 >= tileX && ity1 <= tileY && ity2 >= tileY;
        }

        public void RemoveAll()
        {
            for (int i = 0; i < count; i++)
            {
                ClusterItem item = childs[i];
                if (item is Cluster) (item as Cluster).RemoveAll();
                item.Dispose();
            }
            childs = null;
            count = 0;
            capacity = 0;
            totalCount = 0;
        }

        public void Remove(OnlineMapsMarker marker)
        {
            TryRemoveMarker(marker);
        }

        public void RemoveChild(MarkerWrapper marker)
        {
            for (int i = 0; i < count; i++)
            {
                if (childs[i] == marker)
                {
                    for (int j = i; j < count - 1; j++) childs[j] = childs[j + 1];
                    count--;
                    totalCount--;

                    Cluster p = parent;
                    while (p != null)
                    {
                        p.totalCount--;
                        p.Update();
                        p = p.parent;
                    }

                    childs[count] = null;
                    if (count == 1 && parent != null)
                    {
                        if (childs[0] is MarkerWrapper)
                        {
                            parent.Replace(this, childs[0] as MarkerWrapper);
                            Dispose();
                        }
                    }
                    break;
                }
            }
        }

        private void Replace(Cluster item1, MarkerWrapper item2)
        {
            if (count > 1)
            {
                for (int i = 0; i < count; i++)
                {
                    if (childs[i] == item1)
                    {
                        childs[i] = item2;
                        item2.parent = this;
                        break;
                    }
                }
            }
            else
            {
                parent.Replace(this, item2);
                Dispose();
            }
        }

        private void TryGetMarkers(int z, double tlx, double tly, double brx, double bry, ref List<OnlineMapsMarker> markers)
        {
            if (zoom < 3 || zoom < z || totalCount < instance.minClusteredItems)
            {
                for (int i = 0; i < count; i++) childs[i].GetMarkers(z, tlx, tly, brx, bry, ref markers);
            }
            else
            {
                if (markerRef == null)
                {
                    UpdatePosition();

                    markerRef = new OnlineMapsMarker();
                    markerRef.SetPosition(longitude, latitude);
                    markerRef.texture = instance.groupTexture;
                    markerRef.label = "Group (childs: " + totalCount + ")";

                    if (OnCreateInstance != null) OnCreateInstance(this, markerRef);
                    markerRef.Init();
                }

                markers.Add(markerRef);
            }
        }

        private bool TryRemoveMarker(OnlineMapsMarker marker)
        {
            for (int i = 0; i < count; i++)
            {
                ClusterItem item = childs[i];
                if (item is Cluster)
                {
                    if ((item as Cluster).TryRemoveMarker(marker)) return true;
                }
                else if (item.markerRef == marker)
                {
                    item.Dispose();
                    for (int j = i; j < count - 1; j++) childs[j] = childs[j + 1];
                    count--;
                    totalCount--;

                    Cluster p = parent;
                    while (p != null)
                    {
                        p.totalCount--;
                        p.Update();
                        p = p.parent;
                    }

                    childs[count] = null;

                    if (count == 1 && parent != null)
                    {
                        if (childs[0] is MarkerWrapper)
                        {
                            parent.Replace(this, childs[0] as MarkerWrapper);
                            Dispose();
                        }
                    }

                    return true;
                }
            }
            return false;
        }

        private void Update()
        {
            UpdatePosition();
            if (markerRef != null)
            {
                markerRef.label = "Group (childs: " + totalCount + ")";
                markerRef.SetPosition(longitude, latitude);
            }
        }

        public bool UpdateMarker(OnlineMapsMarker marker)
        {
            for (int i = 0; i < count; i++)
            {
                ClusterItem item = childs[i];
                if (item is Cluster)
                {
                    if ((item as Cluster).UpdateMarker(marker)) return true;
                }
                else if (item.markerRef == marker)
                {
                    (item as MarkerWrapper).UpdatePosition();
                    return true;
                }
            }
            return false;
        }

        private void UpdatePosition()
        {
            double mx = 0, my = 0;

            for (int i = 0; i < count; i++)
            {
                double cmx, cmy;
                childs[i].GetMercatorPosition(out cmx, out cmy);
                mx += cmx;
                my += cmy;
            }

            mx /= count;
            my /= count;

            this.mx = mx;
            this.my = my;

            int max = 1 << zoom;
            mx *= max;
            my *= max;

            projection.TileToCoordinates(mx, my, zoom, out longitude, out latitude);
        }

        public override void UpdatePositions()
        {
            for (int i = 0; i < count; i++) childs[i].UpdatePositions();

            UpdatePosition();
        }
    }

    public class MarkerWrapper : ClusterItem
    {
        private double mx;
        private double my;

        public MarkerWrapper(OnlineMapsMarker marker)
        {
            markerRef = marker;
            marker.GetPosition(out longitude, out latitude);
            mx = longitude;
            my = latitude;
            OnlineMapsUtils.LatLongToMercat(ref mx, ref my);
        }

        public override bool CompareTiles(int zoom, int tx, int ty)
        {
            int max = 1 << zoom;
            return tx == (int)(mx * max) && ty == (int)(my * max);
        }

        public override void Dispose()
        {
            parent = null;
            markerRef = null;
        }

        public override void GetMarkers(int z, double tlx, double tly, double brx, double bry, ref List<OnlineMapsMarker> markers)
        {
            if (InRange(tlx, tly, brx, bry)) markers.Add(markerRef);
        }

        public override void GetMercatorPosition(out double mx, out double my)
        {
            mx = this.mx;
            my = this.my;
        }

        public void GetTilePosition(int z, out int tx, out int ty)
        {
            int max = 1 << z;
            tx = (int)(mx * max);
            ty = (int)(my * max);
        }

        public bool InRange(double tlx, double tly, double brx, double bry)
        {
            if (tlx > brx || Math.Abs(brx - tlx) < 1)
            {
                brx += 360;
                if (brx - longitude > 360)
                {
                    tlx -= 360;
                    brx -= 360;
                }
            }
            return tlx <= longitude && brx >= longitude && tly >= latitude && bry <= latitude;
        }

        public void UpdatePosition()
        {
            markerRef.GetPosition(out longitude, out latitude);
            mx = longitude;
            my = latitude;
            OnlineMapsUtils.LatLongToMercat(ref mx, ref my);

            Cluster p = parent;

            while (p != null)
            {
                int tx, ty;
                GetTilePosition(p.zoom, out tx, out ty);
                if (p.CompareTiles(p.zoom, tx, ty))
                {
                    parent.RemoveChild(this);
                    p.Add(this);
                    break;
                }
                if (p.parent != null) p = p.parent;
                else
                {
                    parent.RemoveChild(this);
                    p.Add(this);
                    break;
                }
            }
        }

        public override void UpdatePositions()
        {

        }
    }

    public abstract class ClusterItem
    {
        public Cluster parent;
        public int tileX;
        public int tileY;
        protected double longitude;
        protected double latitude;
        protected internal OnlineMapsMarker markerRef;

        public abstract bool CompareTiles(int zoom, int tx, int ty);
        public abstract void Dispose();
        public abstract void GetMarkers(int z, double tlx, double tly, double brx, double bry, ref List<OnlineMapsMarker> markers);
        public abstract void GetMercatorPosition(out double mx, out double my);
        public abstract void UpdatePositions();
    }
}
