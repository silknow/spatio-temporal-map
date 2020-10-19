using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using SilknowMap;

public class ObjectDetailPanel : MonoBehaviour
{
    public GameObject prefabRelationProperty;

    public GameObject prefabNormalProperty;
    
    public GameObject prefabObjectImageItem;

    public Text labelText;

    public Transform propertiesContentView;
    public Transform imagesContentView;
    
    public GameObject propertiesPanel;
    public GameObject imagesPanel;

    public RawImage slideshowImage;
    
   
    private List<Texture2D> objectImages = new List<Texture2D>();

    private int currentImageIndex = -1;

    private int numberOfObjectImgs = 0;
    public int currentLoadedImgs = 0;

    
    
    public GameObject buttonNextImg,buttonPrevImg;
    
    
    public void SetData(string uri)
    {
        MapUIManager.instance.ShowLoadingData();
        APIManager.instance.StartCoroutine(APIManager.instance.GetObjectDetail(uri, ResponseCallback));
    }

    private void ResponseCallback(string data)
    {
        MapUIManager.instance.HideLoadingData();
        //Empty propertiew view;
        foreach (var child in propertiesContentView.GetComponentsInChildren<Transform>())
        {
            if(child!=propertiesContentView)
                Destroy(child.gameObject);
        }
        
        
        gameObject.SetActive(true);
        var obj = JsonConvert.DeserializeObject<List<ManMadeObject>>(data)[0];
        
        //Rellenar información mapPoint con datos de petición detalle
        var mapPoint = SilkMap.instance.map.getPointPerUri(obj.id);
        SilkMap.instance.map.GetPropertyManager().SetPropertyValue("description", mapPoint, obj.description);
        SilkMap.instance.map.GetPropertyManager().SetPropertyValue("img", mapPoint, obj.img);
        
        labelText.text = mapPoint.getLabel();
        var visibleProperties = SilkMap.instance.map.GetPropertyManager().GetVisibleProperties();
        var relatableProperties = SilkMap.instance.map.GetPropertyManager().GetRelatableProperties();
        foreach (var vProp in visibleProperties)
        {
            if (vProp.IsImage())
            {
                //SetImages(vProp);
                SetImagesSlideshow(vProp);
                continue;
            }

            var isRelatable = relatableProperties.Contains(vProp);
            var go = Instantiate(!isRelatable ? prefabNormalProperty : prefabRelationProperty,
                propertiesContentView);
            if(!isRelatable)
            {
                go.GetComponent<NormalPropertyItem>().SetPropertyData(vProp);
            }
            else
            {
                go.GetComponent<RelationshipPropertyItem>().SetPropertyData(vProp);
            }
        }
    }

    private void SetImagesSlideshow(Property imgProp)
    {
        objectImages.Clear();
        Resources.UnloadUnusedAssets();
        imagesPanel.SetActive(false);

        var listOfImageProperties = MapUIManager.instance.GetSelectedMarker().getPropertyValue(imgProp.GetName());

        currentLoadedImgs = 0;
        numberOfObjectImgs = listOfImageProperties.Count(img => img.Contains("silknow"));
        
        foreach (var imgUri in listOfImageProperties)
        {
            if (imgUri.Contains("silknow"))
            {
                StartCoroutine(downloadImageFromUri(imgUri, AddImageToSlideshow));
            }
        }
        
        buttonNextImg.SetActive(numberOfObjectImgs>1);
        buttonPrevImg.SetActive(numberOfObjectImgs>1);
    }
    private void AddImageToSlideshow(Texture2D tex)
    {
        if (objectImages == null)
            objectImages = new List<Texture2D>();
        objectImages.Add(tex);
        currentLoadedImgs++;
        if (objectImages.Count == 1)
        {
            slideshowImage.texture = tex;
            slideshowImage.GetComponent<AspectRatioFitter>().aspectRatio = (float)tex.width / tex.height;
            currentImageIndex = 0;
        }

        if (currentLoadedImgs == numberOfObjectImgs)
        {
            if(!imagesPanel.activeSelf)
                imagesPanel.SetActive(true);
        }
            
    }


    public void NextImageSlideshow()
    {
        if (objectImages.Count < 2)
            return;
        if (currentImageIndex < (objectImages.Count - 1))
        {
            currentImageIndex++;
        }
        else
        {
            currentImageIndex = 0;
        }

        var tex = objectImages[currentImageIndex];
        slideshowImage.texture = tex;
        slideshowImage.GetComponent<AspectRatioFitter>().aspectRatio = (float)tex.width / tex.height;
    }
    public void PreviousImageSlideshow()
    {
        if (objectImages.Count < 2)
            return;
        if (currentImageIndex > 0)
        {
            currentImageIndex--;
        }
        else
        {
            currentImageIndex = objectImages.Count -1;
        }

        var tex = objectImages[currentImageIndex];
        slideshowImage.texture = tex;
        slideshowImage.GetComponent<AspectRatioFitter>().aspectRatio = (float)tex.width / tex.height;
    }
    private void SetImages(Property imgProp)
    {
        //Clear all Images;
        foreach (var imgTransform in imagesContentView.GetComponentsInChildren<Transform>())
        {
            if (imgTransform != imagesContentView)
            {
                Destroy(imgTransform.GetComponentInChildren<RawImage>().texture);
                Destroy(imgTransform.gameObject);
            }
        }

        imagesPanel.SetActive(false);
        // Filter images from the silknow server
        //Download all images and set it to the images container
        foreach (var imgUri in MapUIManager.instance.GetSelectedMarker().getPropertyValue(imgProp.GetName()))
        {
            if(!imagesPanel.activeSelf)
                imagesPanel.SetActive(true);
            if(imgUri.Contains("silknow"))
                StartCoroutine(downloadImageFromUri(imgUri, CreateImageItem));
        }
    }
    private void CreateImageItem(Texture2D tex)
    {
        var img = Instantiate(prefabObjectImageItem, imagesContentView);
        img.GetComponent<RawImage>().texture = tex;
        img.GetComponent<AspectRatioFitter>().aspectRatio = (float)tex.width / tex.height;
    }
    IEnumerator downloadImageFromUri(string url, Action<Texture2D> callback)
    {
        
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        DownloadHandler handle = www.downloadHandler;
        //Send Request and wait
        yield return www.SendWebRequest();
        if (www.isHttpError || www.isNetworkError)
        {
            UnityEngine.Debug.Log("Error while Receiving: " + www.error);
        }
        else
        {

            //Load Image
            if (callback != null)
            {
                callback(DownloadHandlerTexture.GetContent(www));
            }
        }
    }
}
