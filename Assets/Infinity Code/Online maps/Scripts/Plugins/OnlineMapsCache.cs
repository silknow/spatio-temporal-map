/*     INFINITY CODE 2013-2019      */
/*   http://www.infinity-code.com   */

#if (!UNITY_WP_8_1 && !UNITY_WEBGL) || UNITY_EDITOR
#define ALLOW_FILECACHE
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

#if ALLOW_FILECACHE
using System.IO;
#endif

/// <summary>
/// Class for caching tiles in memory and the file system.
/// </summary>
[AddComponentMenu("Infinity Code/Online Maps/Plugins/Cache")]
[OnlineMapsPlugin("Cache", typeof(OnlineMapsControlBase), true)]
public class OnlineMapsCache:MonoBehaviour, IOnlineMapsSavableComponent
{
    private const string atlasFilename = "tilecacheatlas.dat";
    private static OnlineMapsCache _instance;

    /// <summary>
    /// Event occurs when loading the tile from the file cache or memory cache.
    /// </summary>
    public Action<OnlineMapsTile> OnLoadedFromCache;

    /// <summary>
    /// Event occurs when loading the tile from the file cache.
    /// </summary>
    public Action<OnlineMapsTile> OnLoadedFromFileCache;

    /// <summary>
    /// Location of the file cache.
    /// </summary>
    public CacheLocation fileCacheLocation = CacheLocation.persistentDataPath;

    /// <summary>
    /// Custom file cache path.
    /// </summary>
    public string fileCacheCustomPath;

    /// <summary>
    /// Template file name in the file cache.
    /// </summary>
    public string fileCacheTilePath = "TileCache/{pid}/{mid}/{lbs}/{lng}/{zoom}/{x}/{y}";

    /// <summary>
    /// Rate of unloaded tiles from the file cache (0-1).
    /// </summary>
    public float fileCacheUnloadRate = 0.3f;

    /// <summary>
    /// The maximum size of the file cache (mb).
    /// </summary>
    public int maxFileCacheSize = 100;

    /// <summary>
    /// The maximum size of the memory cache (mb).
    /// </summary>
    public int maxMemoryCacheSize = 10;

    /// <summary>
    /// Rate of unloaded tiles from the memory cache (0-1).
    /// </summary>
    public float memoryCacheUnloadRate = 0.3f;

    /// <summary>
    /// Flag indicating that the file cache is used.
    /// </summary>
    public bool useFileCache = true;

    /// <summary>
    /// Flag indicating that the memory cache is used.
    /// </summary>
    public bool useMemoryCache = true;

    private static StringBuilder stringBuilder;

    private List<OnlineMaps> maps;
    private MemoryCacheItem[] memoryCache;
    
    private int countMemoryItems;

    [NonSerialized]
    private FileCacheAtlas fileCacheAtlas;

    [NonSerialized]
    private int _memoryCacheSize;
    private IEnumerator saveFileCacheAtlasCoroutine;
    private OnlineMapsSavableItem[] savableItems;

    /// <summary>
    /// The reference to an instance of the cache.
    /// </summary>
    public static OnlineMapsCache instance
    {
        get { return _instance; }
    }

    /// <summary>
    /// The current size of the file cache (mb). 
    /// </summary>
    public int fileCacheSize
    {
        get
        {
            if (fileCacheAtlas == null) LoadFileCacheAtlas();
            return fileCacheAtlas.size;
        }
    }

    /// <summary>
    /// The current size of the memory cache (mb).
    /// </summary>
    public int memoryCacheSize
    {
        get { return _memoryCacheSize; }
    }

    private void AddFileCacheItem(OnlineMapsTile tile, byte[] bytes)
    {
        if (!useFileCache || maxFileCacheSize <= 0) return;
        if (fileCacheAtlas == null) LoadFileCacheAtlas();

        fileCacheAtlas.Add(this, tile, bytes);
        if (fileCacheAtlas.size > maxFileCacheSize * 1000000) fileCacheAtlas.DeleteOldItems(this);

        if (saveFileCacheAtlasCoroutine == null)
        {
            saveFileCacheAtlasCoroutine = SaveFileCacheAtlas();
            StartCoroutine(saveFileCacheAtlasCoroutine);
        }
    }

    private void AddMemoryCacheItem(OnlineMapsTile tile)
    {
        if (!useMemoryCache || maxMemoryCacheSize <= 0) return;

        string tileKey = GetTileKey(tile);
        int tileHash = tileKey.GetHashCode();

        AddMemoryCacheItem(tile, tileHash, tileKey);
    }

    private void AddMemoryCacheItem(OnlineMapsTile tile, int tileHash, string tileKey)
    {
        for (int i = 0; i < countMemoryItems; i++)
        {
            if (memoryCache[i].hash == tileHash && memoryCache[i].key == tileKey) return;
        }

        if (memoryCache == null) memoryCache = new MemoryCacheItem[maxMemoryCacheSize * 10];
        else if (memoryCache.Length == countMemoryItems) Array.Resize(ref memoryCache, memoryCache.Length + 50);

        MemoryCacheItem item = new MemoryCacheItem(tileKey, tile);
        memoryCache[countMemoryItems++] = item;
        _memoryCacheSize += item.size;
        if (memoryCacheSize > maxMemoryCacheSize * 1000000) UnloadOldMemoryCacheItems();
    }

    /// <summary>
    /// Clear all caches.
    /// </summary>
    public void ClearAllCaches()
    {
        ClearMemoryCache();
        ClearFileCache();
    }

    /// <summary>
    /// Clear file cache.
    /// </summary>
    public void ClearFileCache()
    {
#if ALLOW_FILECACHE
        StringBuilder builder = GetFileCacheFolder();
        Directory.Delete(builder.ToString(), true);
        fileCacheAtlas = new FileCacheAtlas();
#endif
    }

    /// <summary>
    /// Clear memory cache.
    /// </summary>
    public void ClearMemoryCache()
    {
        for (int i = 0; i < countMemoryItems; i++)
        {
            memoryCache[i].Dispose();
            memoryCache[i] = null;
        }
        _memoryCacheSize = 0;
        countMemoryItems = 0;
    }

    /// <summary>
    /// Gets the file cache folder.
    /// </summary>
    /// <returns>File cache folder</returns>
    public StringBuilder GetFileCacheFolder()
    {
        if (stringBuilder == null) stringBuilder = new StringBuilder();
        else stringBuilder.Length = 0;

        if (fileCacheLocation == CacheLocation.persistentDataPath) stringBuilder.Append(OnlineMapsUtils.persistentDataPath).Append("/").Append("OnlineMapsCache");
        else
        {
            if (!string.IsNullOrEmpty(fileCacheCustomPath)) //throw new Exception("Custom path is empty.");
                stringBuilder.Append(fileCacheCustomPath);
        }
        return stringBuilder;
    }

    /// <summary>
    /// Fast way to get the size of the file cache.
    /// </summary>
    /// <returns>Size of the file cache (bytes)</returns>
    public int GetFileCacheSizeFast()
    {
#if ALLOW_FILECACHE
        if (fileCacheAtlas != null) return fileCacheAtlas.size;

        StringBuilder builder = GetFileCacheFolder();
        builder.Append("/").Append(atlasFilename);
        string filename = builder.ToString();

        if (!File.Exists(filename)) return 0;

        FileStream stream = new FileStream(filename, FileMode.Open);
        BinaryReader reader = new BinaryReader(stream);

        reader.ReadByte();
        reader.ReadByte();
        reader.ReadInt16();
        int fileCacheSize = reader.ReadInt32();

        (reader as IDisposable).Dispose();
        return fileCacheSize;
#else
        return 0;
#endif
    }

    private static string GetTileKey(OnlineMapsTile tile)
    {
        if (stringBuilder == null) stringBuilder = new StringBuilder();
        else stringBuilder.Length = 0;

        OnlineMapsRasterTile rTile = tile as OnlineMapsRasterTile;

        stringBuilder.Append(rTile.mapType.fullID).Append(tile.key).Append(rTile.labels).Append(rTile.language);
        return stringBuilder.ToString();
    }

    private static string GetTrafficKey(OnlineMapsTile tile)
    {
        if (stringBuilder == null) stringBuilder = new StringBuilder();
        else stringBuilder.Length = 0;

        stringBuilder.Append("traffic").Append(tile.key);
        return stringBuilder.ToString();
    }

    /// <summary>
    /// Get the full path to the file name in the file cache.
    /// </summary>
    /// <param name="shortFilename">Relative file name in the file cache.</param>
    /// <returns>Full path to the file.</returns>
    public string GetFullTilePath(string shortFilename)
    {
#if ALLOW_FILECACHE
        stringBuilder = GetFileCacheFolder();
        stringBuilder.Append("/").Append(shortFilename).Append(".png");
        return stringBuilder.ToString();
#else
        return null;
#endif
    }

    public OnlineMapsSavableItem[] GetSavableItems()
    {
        if (savableItems != null) return savableItems;

        savableItems = new[]
        {
            new OnlineMapsSavableItem("cache", "Cache", SaveSettings)
            {
                loadCallback = LoadSettings
            }
        };

        return savableItems;
    }

    /// <summary>
    /// Geet the relative path to the tile in the file cache.
    /// </summary>
    /// <param name="tile">Tile</param>
    /// <returns>Relative path to the tile in the file cache</returns>
    public StringBuilder GetShortTilePath(OnlineMapsTile tile)
    {
#if ALLOW_FILECACHE
        int startIndex = 0;
        if (stringBuilder == null) stringBuilder = new StringBuilder();
        else stringBuilder.Length = 0;

        OnlineMapsRasterTile rTile = tile as OnlineMapsRasterTile;

        int l = fileCacheTilePath.Length;
        for (int i = 0; i < l; i++)
        {
            char c = fileCacheTilePath[i];
            if (c == '{')
            {
                for (int j = i + 1; j < l; j++)
                {
                    c = fileCacheTilePath[j];
                    if (c == '}')
                    {
                        stringBuilder.Append(fileCacheTilePath.Substring(startIndex, i - startIndex));
                        string v = fileCacheTilePath.Substring(i + 1, j - i - 1).ToLower();
                        if (v == "pid") stringBuilder.Append(rTile.mapType.provider.id);
                        else if (v == "mid") stringBuilder.Append(rTile.mapType.id);
                        else if (v == "zoom" || v == "z") stringBuilder.Append(tile.zoom);
                        else if (v == "x") stringBuilder.Append(tile.x);
                        else if (v == "y") stringBuilder.Append(tile.y);
                        else if (v == "quad") OnlineMapsUtils.TileToQuadKey(tile.x, tile.y, tile.zoom, stringBuilder);
                        else if (v == "lng") stringBuilder.Append(rTile.language);
                        else if (v == "lbs") stringBuilder.Append(rTile.labels ? "le" : "ld");
                        else stringBuilder.Append(v);
                        i = j;
                        startIndex = j + 1;
                        break;
                    }
                }
            }
        }

        stringBuilder.Append(fileCacheTilePath.Substring(startIndex, l - startIndex));
        return stringBuilder;
#else
        return null;
#endif
    }

    private void LoadSettings(OnlineMapsJSONObject json)
    {
        json.DeserializeObject(this);
    }

    private void LoadFileCacheAtlas()
    {
        fileCacheAtlas = new FileCacheAtlas();
        fileCacheAtlas.Load(this);
    }

    private void LoadTile(OnlineMapsTile tile, byte[] bytes)
    {
        OnlineMaps map = tile.map;
        Texture2D texture = new Texture2D(256, 256, TextureFormat.ARGB32, map.control.mipmapForTiles);
        texture.LoadImage(bytes);
        texture.wrapMode = TextureWrapMode.Clamp;

        if (map.control.resultIsTexture)
        {
            (tile as OnlineMapsRasterTile).ApplyTexture(texture);
            map.buffer.ApplyTile(tile);
            OnlineMapsUtils.Destroy(texture);
        }
        else
        {
            tile.texture = texture;
            tile.status = OnlineMapsTileStatus.loaded;
        }

        OnlineMapsRasterTile rTile = tile as OnlineMapsRasterTile;

        if (map.traffic && !string.IsNullOrEmpty(rTile.trafficURL))
        {
            if (map.traffic && !string.IsNullOrEmpty(rTile.trafficURL))
            {
                rTile.trafficWWW = new OnlineMapsWWW(rTile.trafficURL);
                rTile.trafficWWW["tile"] = tile;
                rTile.trafficWWW.OnComplete += OnlineMapsTileManager.OnTrafficWWWComplete;
            }
        }
    }

    private void OnDestroy()
    {
        OnlineMapsTileManager.OnLoadFromCache -= OnStartDownloadTileM;
        OnlineMaps.OnPreloadTiles -= OnPreloadTiles;
        OnlineMapsTile.OnTileDownloaded -= OnTileDownloaded;
        maps = null;
    }

    private void OnDisable()
    {
        if (saveFileCacheAtlasCoroutine != null) StopCoroutine(saveFileCacheAtlasCoroutine);
        if (fileCacheAtlas != null) fileCacheAtlas.Save(this);

        ClearMemoryCache();
    }

    private void OnEnable()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogError("You can have only one instance of Online Maps Cache at a time.");
            Destroy(this);
            return;
        }
        _instance = this;
        OnlineMaps.OnStart += OnMapStart;
    }

    private void OnMapStart(OnlineMaps map)
    {
        if (maps == null) maps = new List<OnlineMaps>();
        if (maps.Contains(map)) return;

        maps.Add(map);
        
    }

    private void OnPreloadTiles(OnlineMaps map)
    {
        lock (OnlineMapsTile.lockTiles)
        {
            float start = Time.realtimeSinceStartup;
            for (int i = 0; i < map.tileManager.tiles.Count; i++)
            {
                OnlineMapsTile tile = map.tileManager.tiles[i];
                if (tile.status != OnlineMapsTileStatus.none || tile.cacheChecked) continue;
                if (!TryLoadFromCache(tile)) tile.cacheChecked = true;
                else if (OnLoadedFromCache != null) OnLoadedFromCache(tile);
                if (Time.realtimeSinceStartup - start > 0.02) return;
            }
        }
    }

    private void OnStartDownloadTileM(OnlineMapsTile tile)
    {
        if (!TryLoadFromCache(tile))
        {
            if (OnlineMapsTileManager.OnStartDownloadTile != null) OnlineMapsTileManager.OnStartDownloadTile(tile);
            else OnlineMapsTileManager.StartDownloadTile(tile);
        }
    }

    private void OnTileDownloaded(OnlineMapsTile tile)
    {
        if (useMemoryCache) AddMemoryCacheItem(tile);
        if (useFileCache) AddFileCacheItem(tile, tile.www.bytes);
    }

    private OnlineMapsJSONItem SaveSettings()
    {
        return OnlineMapsJSON.Serialize(new
        {
            useMemoryCache,
            maxMemoryCacheSize,
            memoryCacheUnloadRate,

            useFileCache,
            maxFileCacheSize,
            fileCacheUnloadRate,
            fileCacheLocation,
            fileCacheCustomPath,
            fileCacheTilePath
        });
    }

    private IEnumerator SaveFileCacheAtlas()
    {
        yield return new WaitForSeconds(5);
        if (fileCacheAtlas != null) fileCacheAtlas.Save(this);
        saveFileCacheAtlasCoroutine = null;
    }

    private void Start()
    {
        OnlineMapsTileManager.OnLoadFromCache += OnStartDownloadTileM;
        OnlineMaps.OnPreloadTiles += OnPreloadTiles;
        OnlineMapsTile.OnTileDownloaded += OnTileDownloaded;
    }

    private bool TryLoadFromCache(OnlineMapsTile tile)
    {
#if ALLOW_FILECACHE
        if (useFileCache && TryLoadFromFileCache(tile)) return true;
#endif
        return false;
    }

    private bool TryLoadFromFileCache(OnlineMapsTile tile)
    {
#if ALLOW_FILECACHE
        if (fileCacheAtlas == null) LoadFileCacheAtlas();

        StringBuilder filename = GetShortTilePath(tile);
        string shortFilename = filename.ToString();
        if (fileCacheAtlas.Contains(shortFilename))
        {
            string fullTilePath = GetFullTilePath(shortFilename);
            if (!File.Exists(fullTilePath)) return false;

            tile.status = OnlineMapsTileStatus.loading;

            OnlineMapsThreadManager.AddThreadAction(() =>
            {
                byte[] bytes = File.ReadAllBytes(fullTilePath);

                OnlineMapsThreadManager.AddMainThreadAction(() =>
                {
                    LoadTile(tile, bytes);
                    AddMemoryCacheItem(tile);
                    tile.MarkLoaded();
                    if (OnLoadedFromFileCache != null) OnLoadedFromFileCache(tile);
                    if (OnLoadedFromCache != null) OnLoadedFromCache(tile);
                    tile.map.Redraw();
                });
            });

            return true;
        }
#endif
        return false;
    }

    private void UnloadOldMemoryCacheItems()
    {
        int countUnload = Mathf.RoundToInt(countMemoryItems * memoryCacheUnloadRate);
        if (countUnload == 0) return;
        if (countUnload < 0) throw new Exception("Can not unload a negative number of items. Check memoryCacheUnloadRate.");
        if (countMemoryItems < countUnload) countUnload = countMemoryItems;

        long[] unloadTimes = new long[countUnload];
        int[] unloadIndices = new int[countUnload];
        int c = 0;

        for (int i = 0; i < countMemoryItems; i++)
        {
            long t = memoryCache[i].time;
            if (c == 0)
            {
                unloadIndices[0] = 0;
                unloadTimes[0] = t;
                c++;
            }
            else
            {
                int index = c;
                int index2 = index - 1;

                while (index2 >= 0)
                {
                    if (unloadTimes[index2] < t) break;

                    index2--;
                    index--;
                }

                if (index < countUnload)
                {
                    for (int j = countUnload - 1; j > index; j--)
                    {
                        unloadIndices[j] = unloadIndices[j - 1];
                        unloadTimes[j] = unloadTimes[j - 1];
                    }
                    unloadIndices[index] = i;
                    unloadTimes[index] = t;
                    if (c < countUnload) c++;
                }
            }
        }

        for (int i = 0; i < countUnload; i++)
        {
            int index = unloadIndices[i];
            MemoryCacheItem mci = memoryCache[index];
            if (mci.tileUsed) continue;

            _memoryCacheSize -= mci.size;
            mci.Dispose();
            memoryCache[index] = null;
        }

        int offset = 0;
        for (int i = 0; i < countMemoryItems; i++)
        {
            if (memoryCache[i] == null) offset++;
            else if (offset > 0) memoryCache[i - offset] = memoryCache[i];
        }

        countMemoryItems -= countUnload;
    }

    /// <summary>
    /// Location of the file cache
    /// </summary>
    public enum CacheLocation
    {
        /// <summary>
        /// Application.persistentDataPath
        /// </summary>
        persistentDataPath,
        /// <summary>
        /// Custom
        /// </summary>
        custom
    }

    private class FileCacheAtlas
    {
        const short ATLAS_VERSION = 1;

        private int _size;
        private FileCacheItem[] items;
        private int capacity = 256;
        private int count = 0;

        public int size
        {
            get { return _size; }
        }

        public FileCacheAtlas()
        {
            _size = 0;
            items = new FileCacheItem[capacity];
        }

        public void Add(OnlineMapsCache cache, OnlineMapsTile tile, byte[] bytes)
        {
#if ALLOW_FILECACHE
            StringBuilder filename = cache.GetShortTilePath(tile);
            string shortFilename = filename.ToString();
            if (Contains(shortFilename)) return;

            string fullFilename = cache.GetFullTilePath(shortFilename);

            OnlineMapsThreadManager.AddThreadAction(() =>
            {
                FileInfo fileInfo = new FileInfo(fullFilename);
                if (!Directory.Exists(fileInfo.DirectoryName)) Directory.CreateDirectory(fileInfo.DirectoryName);
                File.WriteAllBytes(fullFilename, bytes);
            });

            AddItem(shortFilename, bytes.Length);
            _size += bytes.Length;
#endif
        }

        private void AddItem(string filename, int size)
        {
            FileCacheItem item = new FileCacheItem(filename, size);
            if (capacity <= count)
            {
                capacity += 100;
                Array.Resize(ref items, capacity);
            }
            items[count++] = item;
        }

        public bool Contains(string filename)
        {
            int hash = filename.GetHashCode();
            for (int i = 0; i < count; i++)
            {
                if (items[i].hash == hash && items[i].filename == filename) return true;
            }
            return false;
        }

        public void DeleteOldItems(OnlineMapsCache cache)
        {
#if ALLOW_FILECACHE
            int countUnload = Mathf.RoundToInt(count * cache.fileCacheUnloadRate);
            if (countUnload <= 0) throw new Exception("Can not unload a negative number of items. Check fileCacheUnloadRate.");
            if (count < countUnload) countUnload = count;

            long[] unloadTimes = new long[countUnload];
            int[] unloadIndices = new int[countUnload];
            string[] unloadFiles = new string[countUnload];
            int c = 0;

            for (int i = 0; i < count; i++)
            {
                long t = items[i].time;
                if (c == 0)
                {
                    unloadIndices[0] = 0;
                    unloadTimes[0] = t;
                    c++;
                }
                else
                {
                    int index = c;
                    int index2 = index - 1;

                    while (index2 >= 0)
                    {
                        if (unloadTimes[index2] < t) break;

                        index2--;
                        index--;
                    }

                    if (index < countUnload)
                    {
                        for (int j = countUnload - 1; j > index; j--)
                        {
                            unloadIndices[j] = unloadIndices[j - 1];
                            unloadTimes[j] = unloadTimes[j - 1];
                        }
                        unloadIndices[index] = i;
                        unloadTimes[index] = t;
                        if (c < countUnload) c++;
                    }
                }
            }

            for (int i = 0; i < countUnload; i++)
            {
                int index = unloadIndices[i];
                _size -= items[index].size;
                string fullFilename = cache.GetFullTilePath(items[index].filename);
                unloadFiles[i] = fullFilename;
                items[index] = null;
            }

            int offset = 0;
            for (int i = 0; i < count; i++)
            {
                if (items[i] == null) offset++;
                else if (offset > 0) items[i - offset] = items[i];
            }

            count -= countUnload;

            OnlineMapsThreadManager.AddThreadAction(() =>
            {
                for (int i = 0; i < countUnload; i++)
                {
                    string fn = unloadFiles[i];
                    if (File.Exists(fn)) File.Delete(fn);
                }
            });
#endif
        }

        public void Load(OnlineMapsCache cache)
        {
#if ALLOW_FILECACHE
            StringBuilder builder = cache.GetFileCacheFolder();
            builder.Append("/").Append(atlasFilename);
            string filename = builder.ToString();

            if (!File.Exists(filename)) return;

            FileStream stream = new FileStream(filename, FileMode.Open);
            BinaryReader reader = new BinaryReader(stream);

            byte c1 = reader.ReadByte();
            byte c2 = reader.ReadByte();

            if (c1 == 'T' && c2 == 'C')
            {
                int cacheVersion = reader.ReadInt16();
                if (cacheVersion > 0)
                {
                    // For future versions
                }
            }
            else stream.Position = 0;

            _size = reader.ReadInt32();

            long l = stream.Length;
            while (stream.Position < l)
            {
                filename = reader.ReadString();
                int s = reader.ReadInt32();
                long time = reader.ReadInt64();
                FileCacheItem item = new FileCacheItem(filename, s, time);
                if (capacity <= count)
                {
                    capacity *= 2;
                    Array.Resize(ref items, capacity);
                }
                items[count++] = item;
            }

            (reader as IDisposable).Dispose();
#endif
        }

        public void Save(OnlineMapsCache cache)
        {
#if ALLOW_FILECACHE
            StringBuilder builder = cache.GetFileCacheFolder();
            builder.Append("/").Append(atlasFilename);
            string filename = builder.ToString();

            FileInfo fileInfo = new FileInfo(filename);
            if (!Directory.Exists(fileInfo.DirectoryName)) Directory.CreateDirectory(fileInfo.DirectoryName);

            FileCacheItem[] itemsCopy = new FileCacheItem[items.Length];
            items.CopyTo(itemsCopy, 0);

            OnlineMapsThreadManager.AddThreadAction(() =>
            {
                FileStream stream = new FileStream(filename, FileMode.Create);
                BinaryWriter writer = new BinaryWriter(stream);

                writer.Write((byte)'T');
                writer.Write((byte)'C');
                writer.Write(ATLAS_VERSION);

                writer.Write(_size);

                for (int i = 0; i < count; i++)
                {
                    FileCacheItem item = itemsCopy[i];
                    writer.Write(item.filename);
                    writer.Write(item.size);
                    writer.Write(item.time);
                }

                (writer as IDisposable).Dispose();
            });
#endif
        }
    }

    private class FileCacheItem
    {
        public int size;
        public string filename;
        public int hash;
        public long time;

        public FileCacheItem(string filename, int size): this(filename, size, DateTime.Now.Ticks)
        {
            
        }

        public FileCacheItem(string filename, int size, long time)
        {
            this.filename = filename;
            hash = filename.GetHashCode();
            this.size = size;
            this.time = time;
        }
    }

    private class MemoryCacheItem
    {
        public int size;
        public int hash;
        public string key;
        public long time;
        private OnlineMapsTile tile;

        public bool tileUsed
        {
            get { return tile.used; }
        }

        public MemoryCacheItem(string key, OnlineMapsTile tile)
        {
            this.key = key;
            hash = key.GetHashCode();
            size = 30000;
            time = DateTime.Now.Ticks;
            this.tile = tile;
            tile.Block(this);
        }

        public void Dispose()
        {
            tile.Unblock(this);
            tile = null;
            key = null;
        }

        public override string ToString()
        {
            return key;
        }
    }
}
 