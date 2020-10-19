using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeWorldCanvas : MonoBehaviour
{
    
    private int _screenWidth;
    private int _screenHeight;

    private RectTransform _rectTransform;
    // Start is called before the first frame update
    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _screenWidth = Screen.width;
        _screenHeight = Screen.height;
        Invoke("ResizeCanvas", 0.5f);
    }
    private void Update()
    {
        if (_screenWidth != Screen.width || _screenHeight != Screen.height) ResizeCanvas();
    }

    private void ResizeCanvas()
    {
        var mapSize = OnlineMapsTileSetControl.instance.sizeInScene;
        print(mapSize);
        _rectTransform.sizeDelta = mapSize;
        _screenWidth = Screen.width;
        _screenHeight = Screen.height;
    }
}
