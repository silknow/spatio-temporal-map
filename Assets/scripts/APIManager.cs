using System;
using System.Collections;
using System.Collections.Generic;
using Honeti;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Networking;
using Object = UnityEngine.Object;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[Serializable]
public class TimeElement
{
    public string language;
    public string value;
}
[Serializable]
public class Location
{
    [JsonProperty("country")]
    [JsonConverter(typeof(SingleOrArrayConverter<string>))]
    public List<string> country;
    public string id;
    /*[JsonProperty("label")]
    [JsonConverter(typeof(SingleOrArrayConverter<string>))]
    public List<string> label;*/
    public string label;
    
    public string lat;
    
    public string @long;
}
[Serializable]
public class Production
{
    public string id;
    [JsonProperty("location")]
    [JsonConverter(typeof(SingleOrArrayConverter<Location>))]
    public List<Location> location;
    [JsonProperty("material")]
    [JsonConverter(typeof(SingleOrArrayConverter<string>))]
    public List<string> material;
    [JsonProperty("technique")]
    [JsonConverter(typeof(SingleOrArrayConverter<string>))]
    public List<string> technique;
    //[JsonProperty(PropertyName = "time")]
    //[CanBeNull]public string singleTime;
    //[JsonProperty(PropertyName = "time")]
    //[CanBeNull]public TimeElement[] arrayTime;
    //public TimeCoded[] time;
}
[Serializable]
public class ManMadeObject
{
    public string id;
    public string label;
    public string identifier;
    [JsonProperty("description")]
    [JsonConverter(typeof(SingleOrArrayConverter<string>))]
    public List<string> description;
    public Production production;
    [JsonProperty("img")]
    [JsonConverter(typeof(SingleOrArrayConverter<string>))]
    public List<string> img;
    
}
class FloatConverter<T> : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return (objectType == typeof(float));
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        JToken token = JToken.Load(reader);
        return token.ToObject<T>();
    }

    public override bool CanWrite
    {
        get { return false; }
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}
class SingleOrArrayConverter<T> : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return (objectType == typeof(List<T>));
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        JToken token = JToken.Load(reader);
        if (token.Type == JTokenType.Array)
        {
            return token.ToObject<List<T>>();
        }
        return new List<T> { token.ToObject<T>() };
    }

    public override bool CanWrite
    {
        get { return false; }
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}

public class APIManager : Singleton<APIManager>
{

    public List<ManMadeObject> misObjetos;
    public string endpoint = "http://grlc.eurecom.fr/api/silknow/api/";
    public InitApp initAppRef;
    
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
                misObjetos = manMadeObjectList;
                initAppRef.LoadRestData(misObjetos);
                
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
        StartCoroutine(GetObjectList(country:"France"));
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
        misObjetos = manMadeObjectList;
        initAppRef.LoadRestData(misObjetos);
    }
    
    IEnumerator CallToMapManager(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
     
    }
    
    public void OnStackedMap(int levels)
    {
        SilkMap.Instance.createPlane(3);
    }
    public void OnFlatMap()
    {
        SilkMap.Instance.reactivate();
    }
}

