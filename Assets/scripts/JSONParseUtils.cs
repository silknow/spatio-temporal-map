using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SilknowMap
{

[Serializable]
public class TimeElement
{
    public int from;
    public int to;
    public int century;

    public TimeElement(int @from, int to, int century)
    {
        this.@from = @from;
        this.to = to;
        this.century = century;
    }
}
[Serializable]
public class Category
{
    public string id;
    public string label;
}
[Serializable]
public class Location
{
    [JsonProperty("country")]
    [JsonConverter(typeof(SingleOrArrayConverter<string>))]
    public List<string> country;
    public string id;
    public string label;
    public string lat;
    public string @long;
    public string type;
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
    [JsonProperty("time")]
    [JsonConverter(typeof(SingleOrArrayConverter<string>))]
    public List<string> time;
    [JsonProperty("category")]
    [JsonConverter(typeof(SingleOrArrayConverter<Category>))]
    public List<Category> category;
}
[Serializable]
public class ManMadeObject
{
    public string id;
    [JsonProperty("label")]
    [JsonConverter(typeof(SingleOrArrayConverter<string>))]
    public List<string> label;
    public string identifier;
    [JsonProperty("description")]
    [JsonConverter(typeof(SingleOrArrayConverter<string>))]
    public List<string> description;
    public Production production;
    [JsonProperty("img")]
    [JsonConverter(typeof(SingleOrArrayConverter<string>))]
    public List<string> img;
    
}
[Serializable]
public class AdasilkResultPage
{
    public int pageNumber;
    public List<ManMadeObject> pageResults;
    
}
public class FloatConverter<T> : JsonConverter
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


public class SingleOrArrayConverter<T> : JsonConverter
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

}