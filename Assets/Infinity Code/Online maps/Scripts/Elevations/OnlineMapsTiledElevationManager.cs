/*     INFINITY CODE 2013-2019      */
/*   http://www.infinity-code.com   */

using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implements elevation managers, which loads elevation data by tiles
/// </summary>
/// <typeparam name="T">Type of elevation manager</typeparam>
public abstract class OnlineMapsTiledElevationManager<T> : OnlineMapsElevationManager<T>
    where T : OnlineMapsTiledElevationManager<T>
{
    /// <summary>
    /// Called when data download starts.
    /// </summary>
    public Action<Tile> OnDownload;

    /// <summary>
    /// Called when data is successfully downloaded.
    /// </summary>
    public Action<Tile, OnlineMapsWWW> OnDownloadSuccess;

    /// <summary>
    /// Offset of tile zoom from map zoom
    /// </summary>
    public int zoomOffset = 3;

    protected Dictionary<ulong, Tile> tiles;
    protected bool needUpdateMinMax = true;

    private int prevTileX;
    private int prevTileY;

    protected abstract int tileWidth { get; }
    protected abstract int tileHeight { get; }

    public override bool hasData
    {
        get { return true; }
    }

    protected override float GetElevationValue(double x, double z, float yScale, double tlx, double tly, double brx, double bry)
    {
        float v = GetUnscaledElevationValue(x, z, tlx, tly, brx, bry);

        if (bottomMode == OnlineMapsElevationBottomMode.minValue) v -= minValue;
        return v * yScale * scale;
    }

    protected override float GetUnscaledElevationValue(double x, double z, double tlx, double tly, double brx, double bry)
    {
        if (tiles == null)
        {
            tiles = new Dictionary<ulong, Tile>();
            return 0;
        }
        x = x / -sizeInScene.x;
        z = z / sizeInScene.y;

        double ttlx, ttly, tbrx, tbry;

        map.projection.CoordinatesToTile(tlx, tly, map.zoom, out ttlx, out ttly);
        map.projection.CoordinatesToTile(brx, bry, map.zoom, out tbrx, out tbry);

        if (tbrx < ttlx) tbrx += 1 << map.zoom;

        double cx = (tbrx - ttlx) * x + ttlx;
        double cz = (tbry - ttly) * z + ttly;

        int zoom = map.zoom - zoomOffset;
        double tx, ty;
        map.projection.TileToCoordinates(cx, cz, map.zoom, out cx, out cz);
        map.projection.CoordinatesToTile(cx, cz, zoom, out tx, out ty);
        int ix = (int)tx;
        int iy = (int)ty;

        ulong key = OnlineMapsTileManager.GetTileKey(zoom, ix, iy);
        Tile tile;
        bool finded = tiles.TryGetValue(key, out tile);
        if (finded && !tile.loaded) finded = false;

        if (!finded)
        {
            int nz = zoom;

            while (!finded && nz < OnlineMaps.MAXZOOM)
            {
                nz++;
                map.projection.CoordinatesToTile(cx, cz, nz, out tx, out ty);
                ix = (int)tx;
                iy = (int)ty;
                key = OnlineMapsTileManager.GetTileKey(nz, ix, iy);

                finded = tiles.TryGetValue(key, out tile) && tile.loaded;
            }
        }

        if (!finded)
        {
            int nz = zoom;

            while (!finded && nz > 1)
            {
                nz--;
                map.projection.CoordinatesToTile(cx, cz, nz, out tx, out ty);
                ix = (int)tx;
                iy = (int)ty;
                key = OnlineMapsTileManager.GetTileKey(nz, ix, iy);

                finded = tiles.TryGetValue(key, out tile) && tile.loaded;
            }
        }

        if (!finded) return 0;

        map.projection.CoordinatesToTile(cx, cz, tile.zoom, out tx, out ty);
        return tile.GetElevation(tx, ty);
    }

    private void OnChangePosition()
    {
        double tx, ty;
        map.GetTilePosition(out tx, out ty);

        if (needUpdateMinMax || prevTileX != (int)tx || prevTileY != (int)ty)
        {
            UpdateMinMax();
            prevTileX = (int)tx;
            prevTileY = (int)ty;
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (map != null)
        {
            map.OnChangePosition -= OnChangePosition;
            map.OnLateUpdateBefore -= OnLateUpdateBefore;
        }

        OnlineMapsTileManager.OnPrepareDownloadTile -= OnStartDownloadTile;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (tiles == null) tiles = new Dictionary<ulong, Tile>();
    }

    private void OnLateUpdateBefore()
    {
        if (needUpdateMinMax) UpdateMinMax();
    }

    private void OnStartDownloadTile(OnlineMapsTile tile)
    {
        int zoom = map.zoom - zoomOffset;
        if (tile.zoom < zoom || zoom < 1 || !zoomRange.InRange(map.zoom)) return;

        int s = 1 << (tile.zoom - zoom);
        int x = tile.x / s;
        int y = tile.y / s;

        ulong key = OnlineMapsTileManager.GetTileKey(zoom, x, y);
        if (tiles.ContainsKey(key)) return;

        Tile t = new Tile
        {
            x = x,
            y = y,
            zoom = zoom,
            width = tileWidth,
            height = tileHeight
        };
        tiles.Add(key, t);

        if (OnDownload != null) OnDownload(t);
        else StartDownloadElevationTile(t);
    }

    protected override void Start()
    {
        map.OnChangePosition += OnChangePosition;
        map.OnLateUpdateBefore += OnLateUpdateBefore;

        OnlineMapsTileManager.OnPrepareDownloadTile += OnStartDownloadTile;
    }

    /// <summary>
    /// Starts downloading elevation data for a tile
    /// </summary>
    /// <param name="tile">Tile</param>
    public abstract void StartDownloadElevationTile(Tile tile);

    protected override void UpdateMinMax()
    {
        double tlx, tly, brx, bry;
        map.GetTileCorners(out tlx, out tly, out brx, out bry);

        int zoom = map.zoom - zoomOffset;
        if (zoom < 1)
        {
            minValue = maxValue = 0;
            return;
        }

        int itlx = (int) tlx;
        int itly = (int) tly;
        int ibrx = (int) brx;
        int ibry = (int) bry;

        int s = 1 << zoomOffset;

        itlx /= s;
        itly /= s;
        ibrx /= s;
        ibry /= s;

        minValue = short.MaxValue;
        maxValue = short.MinValue;

        for (int x = itlx; x <= ibrx; x++)
        {
            for (int y = itly; y <= ibry; y++)
            {
                ulong key = OnlineMapsTileManager.GetTileKey(zoom, x, y);
                Tile tile;
                if (!tiles.TryGetValue(key, out tile)) continue;

                if (tile.minValue < minValue) minValue = tile.minValue;
                if (tile.maxValue > maxValue) maxValue = tile.maxValue;
            }
        }
    }

    /// <summary>
    /// Elevation tile
    /// </summary>
    public class Tile
    {
        /// <summary>
        /// Is the tile loaded?
        /// </summary>
        public bool loaded = false;

        /// <summary>
        /// Tile X
        /// </summary>
        public int x;

        /// <summary>
        /// Tile Y
        /// </summary>
        public int y;

        /// <summary>
        /// Tile zoom
        /// </summary>
        public int zoom;

        /// <summary>
        /// Minimum elevation value
        /// </summary>
        public short minValue;

        /// <summary>
        /// Maximum elevation value
        /// </summary>
        public short maxValue;

        /// <summary>
        /// Elevation data width
        /// </summary>
        public int width;

        /// <summary>
        /// Elevation data height
        /// </summary>
        public int height;

        /// <summary>
        /// Elevation values
        /// </summary>
        public short[,] elevations;

        /// <summary>
        /// Get elevation value from tile
        /// </summary>
        /// <param name="tx">Relative X (0-1)</param>
        /// <param name="ty">Relative Y (0-1)</param>
        /// <returns>Elevation value</returns>
        public float GetElevation(double tx, double ty)
        {
            if (!loaded) return 0;

            double rx = (tx - (int)tx) * 255;
            double ry = (ty - (int)ty) * 255;

            int x1 = (int) rx;
            int x2 = x1 + 1;
            int y1 = (int) ry;
            int y2 = y1 + 1;

            if (x2 > 255) x2 = 255;
            if (y2 > 255) y2 = 255;

            double dx = rx - x1;
            double dy = ry - y1;

            double v1 = elevations[x1, y1];
            double v2 = elevations[x2, y1];
            double v3 = elevations[x1, y2];
            double v4 = elevations[x2, y2];

            v1 = (v2 - v1) * dx + v1;
            v2 = (v4 - v3) * dx + v3;
            return (float)((v2 - v1) * dy + v1);
        }
    }
}