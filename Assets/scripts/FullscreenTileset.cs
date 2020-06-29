using UnityEngine;


    public class FullscreenTileset : MonoBehaviour
    {
        public Camera mapCamera;

        private int tilesetWidth;
        private int tilesetHeight;
        private OnlineMaps map;

        private void Start()
        {
            map = OnlineMaps.instance;

            tilesetWidth = (int)OnlineMapsTileSetControl.instance.sizeInScene.x;
            tilesetHeight = (int)OnlineMapsTileSetControl.instance.sizeInScene.y;

            UpdateSize();
        }

        private void Update()
        {
            UpdateSize();
        }

        private void UpdateSize()
        {
            float height = mapCamera.orthographicSize * 2;
            float width = height * Screen.width / Screen.height;

            int h = Mathf.RoundToInt(height);
            int w = Mathf.RoundToInt(width);

            int wOff = w % 256;
            int hOff = h % 256;

            if (wOff != 0) w = (w / 256 + 1) * 256;
            if (hOff != 0) h = (h / 256 + 1) * 256;
        

            if (w < 512) w = 512;
            if (h < 512) h = 512;

            if (w != tilesetWidth || h != tilesetHeight)
            {
                OnlineMapsTileSetControl.instance.Resize(w, h, w, h);
                map.transform.position = map.transform.position - new Vector3((tilesetWidth - w) / 2, 0, (tilesetHeight - h) / -2);

                tilesetWidth = w;
                tilesetHeight = h;
            }
        }
    }
