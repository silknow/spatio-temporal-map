using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using SilknowMap;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class TestClustering2D : MonoBehaviour
{
    private ManMadeObject[] LoadJSONFromHTML()
    {
        var startTime = Stopwatch.StartNew();
        var jsontest = Resources.Load("parsed_results30000");
        TextAsset temp = jsontest as TextAsset;
        Debug.Log("TIEMPO CARGADO JSON "+startTime.ElapsedMilliseconds *0.001f);
        if (temp != null)
        {
            var manMadeObjectList = JsonConvert.DeserializeObject<ManMadeObject[]>(temp.text);
            Debug.Log("TIEMPO CARGADO Y PARSEADO JSON "+startTime.ElapsedMilliseconds *0.001f);
            return manMadeObjectList;
        }
        else
        {
            return null;
        }
    }
  
    private IEnumerator GenerateMarkers(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);

        Resources.UnloadUnusedAssets();
        
       
        var objectList = LoadJSONFromHTML();
        var startTime = Stopwatch.StartNew();
        var objectCount = objectList.Length; 
        
       
        for (var i = 0; i < objectCount; i++)
        {
            var obj = objectList[i];
            if (obj.production == null || obj.production.location == null || obj.production.location.Length == 0)
                continue;
            var lt = obj.production.location[0].lat;
            var lg = obj.production.location[0].@long;


            OnlineMapsMarker marker = OnlineMapsMarkerManager.CreateItem(lg, lt);
            marker.label = obj.label != null && obj.label.Length > 0 ? obj.label[0] : obj.identifier;
            
            marker.Init();

            marker.OnPositionChanged += delegate(OnlineMapsMarkerBase m)
            {
                Clustering2DMarkers.UpdateMarkerPosition(m as OnlineMapsMarker);
            };

            Clustering2DMarkers.Add(marker);
        }

        Debug.Log("Antes de UpdatePositions: " + (startTime.ElapsedMilliseconds * 0.001f) + " segundos");
        Clustering2DMarkers.UpdatePositions();
        
        Debug.Log("CARGAR TODOS LOS OBJETOS: " + (startTime.ElapsedMilliseconds *0.001f ) + " segundos");
  
    }

    private void Start()
    {
        StartCoroutine(GenerateMarkers(2f));
    }
    
}