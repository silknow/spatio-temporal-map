using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using SilknowMap;
using Honeti;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class APIManager : Singleton<APIManager>
{

    [DllImport("__Internal")]
    private static extern void CancelLoadingData();


    public List<ManMadeObject> objectList;
    public string endpoint = "http://grlc.eurecom.fr/api-git/silknow/api/";
    public InitApp initAppRef;
    [SerializeField]
    public Dictionary<string, TimeElement> timeValues;

    private bool _sendingInfo = false;

    public void Awake()
    {

        endpoint = string.IsNullOrEmpty(PlayerPrefs.GetString("API_endpoint")) ? "http://grlc.eurecom.fr/api-git/silknow/api/" :PlayerPrefs.GetString("API_endpoint") ;
        
        timeValues = new Dictionary<string, TimeElement>();
        PopulateTimeDictionary();
    }

    private void PopulateTimeDictionary()
    {
        //AÑADIR SIGLOS 10 a 14 con strings localizados 
        //10th Century
        var tenth = new TimeElement(901,1000,10); 
        timeValues.Add("tenth century (dates CE)",tenth);
        //11th Century
        var eleventh = new TimeElement(1001,1100,11); 
        timeValues.Add("eleventh century (dates CE)",eleventh);
        //12th Century
        var twelfth = new TimeElement(1101,1200,12); 
        timeValues.Add("twelfth century (dates CE)",twelfth);
        //13th Century
        var thirteenth = new TimeElement(1201,1300,13); 
        timeValues.Add("thirteenth century (dates CE)",thirteenth);
        //14th Century
        var fourteenth = new TimeElement(1301,1400,14); 
        timeValues.Add("fourteenth century (dates CE)",fourteenth);
        
        //15th Century
        var fifteenth = new TimeElement(1401, 1500, 15); 
        timeValues.Add("fifteenth century (dates CE)",fifteenth);
        timeValues.Add("siglo quince",fifteenth);
        timeValues.Add("quindicesimo secolo",fifteenth);
        timeValues.Add("quinzième siècle",fifteenth);
        //16th Century
        var sixteenth = new TimeElement(1501, 1600, 16);
        timeValues.Add("sixteenth century (dates CE)",sixteenth);
        timeValues.Add("siglo dieciséis",sixteenth);
        timeValues.Add("sedicesimo secolo",sixteenth);
        timeValues.Add("seizième siècle",sixteenth);
        //17th Century
        var seventeenth = new TimeElement(1601,1700,17);
        timeValues.Add("seventeenth century (dates CE)",seventeenth);
        timeValues.Add("siglo diecisiete",seventeenth);
        timeValues.Add("diciassettesimo secolo",seventeenth);
        timeValues.Add("dix-septième siècle",seventeenth);
        //18th Century
        var eighteenth = new TimeElement(1701,1800,18);
        timeValues.Add("eighteenth century (dates CE)",eighteenth);
        timeValues.Add("siglo dieciocho",eighteenth);
        timeValues.Add("diciottesimo secolo",eighteenth);
        timeValues.Add("dix-huitième siècle",eighteenth);
        //19th Century
        var nineteenth = new TimeElement(1801,1900,19);
        timeValues.Add("nineteenth century (dates CE)",nineteenth);
        timeValues.Add("siglo diecinueve",nineteenth);
        timeValues.Add("diciannovesimo secolo",nineteenth);
        timeValues.Add("dix-neuvième siècle",nineteenth);
        //20th Century
        var twentieth = new TimeElement(1901,2000,20);
        timeValues.Add("twentieth century (dates CE)",twentieth);
        timeValues.Add("siglo veinte",twentieth);
        timeValues.Add("ventesimo secolo",twentieth);
        timeValues.Add("vingtième siècle",twentieth);
    }

    IEnumerator GetObjectList(string country = null,string technique = null, string time = null, string material = null)
    {
        Dictionary<string,string> queryParams = new Dictionary<string, string>();
        queryParams.Add("country",country);
        queryParams.Add("technique",technique);
        queryParams.Add("time",time);
        queryParams.Add("material",material);
        queryParams.Add("lang",I18N.instance.gameLang.ToString().ToLowerInvariant());
        
        string uri = endpoint;

        uri = String.Concat(uri, "obj_map"); 
        var firstElement = true;
        foreach (var param in queryParams)
        {
            if(string.IsNullOrEmpty(param.Value))
                continue;
            if (firstElement)
            {
                firstElement = false;
                uri = String.Concat(uri, "?" + param.Key + "=" + param.Value);
            }
            else
            {
                uri = String.Concat(uri, "&" + param.Key + "=" + param.Value);
            }
        }

        uri = String.Concat(uri, firstElement ? "?endpoint=http%3A%2F%2Fdata.silknow.org%2Fsparql" : "&endpoint=http%3A%2F%2Fdata.silknow.org%2Fsparql");
        
        
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();
            if (webRequest.isNetworkError)
            {
                Debug.Log(": Error: " + webRequest.error);
            }
            else
            {
                var response = webRequest.downloadHandler.text;
                List<ManMadeObject> manMadeObjectList;
                try
                {
                    manMadeObjectList = JsonConvert.DeserializeObject<List<ManMadeObject>>(response);
                    
                }
                catch (JsonReaderException e)
                {
                    print("errorParsing");
                    yield break;
                }

                if (manMadeObjectList.Count < 1)
                {
                    print("empty response");
                    yield break;
                }

                objectList = manMadeObjectList;
               
                initAppRef.LoadRestData(objectList);
                
            }
        }
    }
    public IEnumerator GetObjectDetail(string objectId, Action<string> callback = null)
    {
        if (string.IsNullOrEmpty(objectId))
            yield break;
        Dictionary<string,string> queryParams = new Dictionary<string, string>();
        queryParams.Add("id",objectId);
        queryParams.Add("lang",I18N.instance.gameLang.ToString());
        string uri = endpoint;
        uri = String.Concat(uri, "obj_detail"); 
        var firstElement = true;
        foreach (var param in queryParams)
        {
            if(string.IsNullOrEmpty(param.Value))
                continue;
            if (firstElement)
            {
                firstElement = false;
                uri = String.Concat(uri, "?" + param.Key + "=" + param.Value);
            }
            else
            {
                uri = String.Concat(uri, "&" + param.Key + "=" + param.Value);
            }
        }

        uri = String.Concat(uri, firstElement ? "?endpoint=http%3A%2F%2Fdata.silknow.org%2Fsparql" : "&endpoint=http%3A%2F%2Fdata.silknow.org%2Fsparql");

        //Debug.Log(uri);
        
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();
            if (webRequest.isNetworkError)
            {
                Debug.Log(": Error: " + webRequest.error);
            }
            else
            {
                var response = webRequest.downloadHandler.text;

                if (callback == null)
                    print(response);
                else
                    callback(response);
            }
        }
    }
    IEnumerator GetThesaurusConcept(string vocabularyId, string language ="en")
    {
        if (string.IsNullOrEmpty(vocabularyId))
            yield break;
        string uri = endpoint;
        uri = String.Concat(uri, "thesaurus"); 
        uri = String.Concat(uri, "?id=" + vocabularyId);
        uri = String.Concat(uri, "&lang=" + language);
        uri = String.Concat(uri, "&endpoint=http%3A%2F%2Fdata.silknow.org%2Fsparql");

        Debug.Log(uri);
        
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();
            if (webRequest.isNetworkError)
            {
                Debug.Log(": Error: " + webRequest.error);
            }
            else
            {
                var response = webRequest.downloadHandler.text;
                print(response);
            }
        }
    }

    public void OnButtonTestClick()
    {
        StartCoroutine(GetObjectList(technique:"damask"));
    }
    public void TestFranceTextiles()
    {
        StartCoroutine(GetObjectList(country:"Spain"));
    }
    public void TestLoadTextilesFromHTML()
    {
        var jsontest = Resources.Load("testFranceTextiles");
        TextAsset temp = jsontest as TextAsset;
        if (temp != null) LoadJSONFromHTML(temp.text);
    }
    public void TestObjectDetail()
    {
        StartCoroutine(GetObjectDetail("http://data.silknow.org/object/4fa6a1a1-0aab-333e-9846-3c04a114e236"));
    }

    public void LoadJSONFromHTML(string json)
    {
        var manMadeObjectList = JsonConvert.DeserializeObject<List<ManMadeObject>>(json);
        objectList = manMadeObjectList;
        initAppRef.LoadRestData(objectList);
    }
    
    public void StartDumpingJSON(string numberOfPages)
    {
        //print("StartDumpingJSON Total numberOfPages: "+numberOfPages);
        _sendingInfo = true;
        objectList.Clear();
        MapUIManager.instance.ShowProgressBar(Int32.Parse(numberOfPages));
    }

    public void AppendJSONData(string json)
    {
        if(!_sendingInfo)
            return;
        var manMadeObjectList = JsonConvert.DeserializeObject<List<ManMadeObject>>(json);
        //print("AppendJSONData Incoming JSON count: "+manMadeObjectList.Count);
        objectList  = objectList.Concat(manMadeObjectList).ToList();
        //print("AppendJSONData Total JSON count: "+objectList.Count);
    }

    public void EndDumpingJSON()
    {
        //print("EndDumpingJSON");
        _sendingInfo = false;
        initAppRef.LoadRestData(objectList);
        MapUIManager.instance.HideProgressBar();
        //print("EndDumpingJSON Total JSON count: "+objectList.Count);
        objectList.Clear();
    }
    public void CancelDumpingJSON()
    {
        //Debug.Log("Cancel Dumping JSON");
        _sendingInfo = false;
        MapUIManager.instance.HideProgressBar();
        objectList.Clear();
        CancelLoadingData();
    }
    
    
    IEnumerator CallToMapManager(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
     
    }
    
   
    public void OnFlatMap()
    {
        MapUIManager.instance.ToggleMapViewingMode();
        SilkMap.Instance.reactivate();
    }
    public void ToggleLanguage()
    {
        var lang = I18N.instance.gameLang == LanguageCode.EN ? "ES" : "EN";
        I18N.instance.setLanguage(lang);
        OnlineMaps.instance.language = I18N.instance.gameLang.ToString().ToLower(CultureInfo.InvariantCulture);
    }
    public void SetAPIEndpoint(string apiEndpoint)
    {
        if (!string.IsNullOrEmpty(apiEndpoint))
        {
            PlayerPrefs.SetString("API_endpoint",apiEndpoint);
            endpoint = apiEndpoint;
        }
       
    }
    public void SetLanguage(string lang)
    {
        if (!string.IsNullOrEmpty(lang))
        {
            I18N.instance.setLanguage(lang);
        }
       
    }

    
}

