using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    [Serializable]
    public class Location
    {
        public List<string> country { get; set; }
        public string id { get; set; }
        public string type { get; set; }
        public string label { get; set; }
        public float lat { get; set; }
        public float @long { get; set; }
    }
    [Serializable]
    public class Category
    {
        public string id;
        public string label;
    }
    [Serializable]
    public class Production
    {
        public string id { get; set; }
        public List<string> material { get; set; }
        public List<Location> location { get; set; }
        public List<string> technique { get; set; }
        public List<string> time { get; set; }
        public Category category { get; set; }
    }
    [Serializable]
    public class ManMadeObjectMod
    {
        public string id { get; set; }
        public string label { get; set; }
        public string identifier { get; set; }
        public Production production { get; set; }
    }
     
    [Serializable]
    public class RootObject
    {
        public List<ManMadeObjectMod> manMadeObjects;
    }
    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.Items;
        }

        public static string ToJson<T>(T[] array)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper);
        }

        public static string ToJson<T>(T[] array, bool prettyPrint)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
    }

}