/*         INFINITY CODE         */
/*   https://infinity-code.com   */

using System.Collections.Generic;
using UnityEngine;

namespace InfinityCode.OnlineMapsExamples
{
    /// <summary>
    /// Example of working with OnlineMapsJSON.
    /// </summary>
    [AddComponentMenu("Infinity Code/Online Maps/Examples (API Usage)/JSONExample")]
    public class JSONExample : MonoBehaviour
    {
        public void Start()
        {
            // Generate random data object
            Data data = Data.Generate();

            // Serialization an object
            OnlineMapsJSONItem json = Serialize(data);

            // Convert JSON object to a string
            string jsonString = json.ToString();
            Debug.Log(jsonString);

            // Parsing JSON string
            json = Parse(jsonString);

            // Full deserialization of JSON object
            FullDeserialization(json);

            // Full deserialization of JSON string
            FullDeserializationOfString(jsonString);

            // Using selectors
            UsingSelectors(json);

            // Partial deserialization of JSON object
            PartialDeserialization(json);

            // How to get the value of JSON elements.
            GetValues(json);

            // How to go through all the children
            UsingForEach(json);

            // Dynamic generations of JSON
            DynamicGeneration();
        }

        /// <summary>
        /// Serialize an object into JSON object
        /// </summary>
        /// <param name="data">The object to be serialized</param>
        /// <returns>JSON object</returns>
        private OnlineMapsJSONItem Serialize(Data data)
        {
            return OnlineMapsJSON.Serialize(data);
        }

        /// <summary>
        /// Parsing JSON string
        /// </summary>
        /// <param name="jsonString">JSON string</param>
        /// <returns>JSON object</returns>
        private OnlineMapsJSONItem Parse(string jsonString)
        {
            return OnlineMapsJSON.Parse(jsonString);
        }

        /// <summary>
        /// Full deserialization of JSON object into an object
        /// </summary>
        /// <param name="json">JSON object</param>
        private void FullDeserialization(OnlineMapsJSONItem json)
        {
            // Full deserialization
            Data data = json.Deserialize<Data>();
            Debug.Log(data.text);

            // Deserialization into object with alias
            Data2 data2 = json.Deserialize<Data2>();
            Debug.Log(data2.lItems[0].name);
        }

        /// <summary>
        /// Full deserialization of JSON string into an object
        /// </summary>
        /// <param name="jsonString">JSON string</param>
        private void FullDeserializationOfString(string jsonString)
        {
            Data data = OnlineMapsJSON.Deserialize<Data>(jsonString);
            Debug.Log(data.items.Length);
        }

        /// <summary>
        /// Using selectors
        /// </summary>
        /// <param name="json">JSON object</param>
        private void UsingSelectors(OnlineMapsJSONItem json)
        {
            // Get the text element that is the child of the current node.
            OnlineMapsJSONItem text = json["text"];
            Debug.Log(text.V<string>());

            // Gets the second element in the items array.
            OnlineMapsJSONItem item = json["items/1"];
            Debug.Log(item.ToString());

            // Gets the second element, the first element of the items array. In this case - x.
            OnlineMapsJSONItem x = json["items/0/x"];
            Debug.Log(x.V<int>());

            // Gets all the name elements in the items array.
            OnlineMapsJSONItem names = json["items/*/name"];
            Debug.Log(names.ToString());

            // Looks for id elements, in the child element of any nesting.
            OnlineMapsJSONItem ids = json["//id"];
            Debug.Log(ids.ToString());
        }

        /// <summary>
        /// Partial deserialization of JSON object
        /// </summary>
        /// <param name="json">JSON object</param>
        private void PartialDeserialization(OnlineMapsJSONItem json)
        {
            /*
             * Using OnlineMapsJSON you can select and deserialize only part of json. 
             * This is very useful, especially if you are working with files from 
             * third-party sources and you do not need all the data.
             */

            // Select and deserialize the items list.
            // It does not matter that in the original data it was an array.
            List<Item> items = json["items"].Deserialize<List<Item>>();
            Debug.Log(items.Count);

            // Select and deserialize all subItems
            OnlineMapsJSONItem item = json["//subItems"];
            Item2[] subItems = item.Deserialize<Item2[]>();
            Debug.Log(subItems.Length);
        }

        /// <summary>
        /// How to get the value of JSON elements.
        /// </summary>
        /// <param name="json">JSON object</param>
        private void GetValues(OnlineMapsJSONItem json)
        {
            // Get the value of the text element.
            string text = json["text"].Value<string>();

            // or in such a ways
            text = json.ChildValue<string>("text");
            text = json.V<string>("text");
            text = json["text"].V<string>();
            Debug.Log(text);

            // You can get: string, bool, float, double, int, long, byte, short.
            int x = json["listItems/0/x"].Value<int>();
            Debug.Log(x);

            // A value of any type can be read as a string.
            // In this case, y is int.
            string y = json["listItems/0/y"].Value<string>();
            Debug.Log(y);
        }

        /// <summary>
        /// How to go through all the children
        /// </summary>
        /// <param name="json">JSON object</param>
        private void UsingForEach(OnlineMapsJSONItem json)
        {
            // Go through all the elements in the items.
            foreach (OnlineMapsJSONItem item in json["items"])
            {
                Debug.Log(item.ChildValue<int>("x"));
            }

            // Very often you need to know the key of the element, you can do it this way.
            foreach (KeyValuePair<string, OnlineMapsJSONItem> pair in (json as OnlineMapsJSONObject).table)
            {
                Debug.Log(pair.Key);
            }
        }

        /// <summary>
        /// Dynamic generations of JSON
        /// </summary>
        private void DynamicGeneration()
        {
            // Create Object node
            OnlineMapsJSONObject rootNode = new OnlineMapsJSONObject();

            // Create and add value nodes
            rootNode.Add("text", new OnlineMapsJSONValue("Hello"));
            rootNode.Add("text2", new OnlineMapsJSONValue("World", OnlineMapsJSONValue.ValueType.STRING));

            // Create and add array nodes
            OnlineMapsJSONArray childs = new OnlineMapsJSONArray();
            rootNode.Add("childs", childs);
            childs.Add(new OnlineMapsJSONValue(255));

            // Combine nodes
            OnlineMapsJSONObject anotherNode = new OnlineMapsJSONObject();
            anotherNode.Add("text3", new OnlineMapsJSONValue("Another Node text"));
            anotherNode.Add("text", new OnlineMapsJSONValue("This value will be ignored in rootNode, because it already has a node with the text key."));
            rootNode.Combine(anotherNode);

            // Serialize an anonymous class, and combine it with rootNode
            rootNode.AppendObject(new
            {
                x = 123,
                y = 456,
                item = Item.Generate()
            });

            Debug.Log(rootNode.ToString());
        }

        public class Data
        {
            public Item[] items;
            public List<Item> listItems;
            public string text;

            public static Data Generate()
            {
                Data data = new Data();
                data.text = "Hello World";

                int countItems = Random.Range(5, 10);
                data.items = new Item[countItems];
                for (int i = 0; i < countItems; i++) data.items[i] = Item.Generate();

                int countListItems = Random.Range(3, 7);
                data.listItems = new List<Item>();
                for (int i = 0; i < countListItems; i++) data.listItems.Add(Item.Generate());

                return data;
            }
        }

        public class Data2
        {
            [OnlineMapsJSON.Alias("listItems")]
            public List<Item> lItems;
        }

        public class Item
        {
            public string name;
            public int x;
            public int y;
            public List<Item2> subItems;

            public static Item Generate()
            {
                Item item = new Item();
                item.x = Random.Range(0, 1000);
                item.y = Random.Range(-100, 100);
                item.name = item.x + "x" + item.y;

                int subItemsCount = Random.Range(3, 5);
                item.subItems = new List<Item2>();
                for (int i = 0; i < subItemsCount; i++) item.subItems.Add(Item2.Generate());

                return item;
            }
        }

        public struct Item2
        {
            public string id;
            public double r;
            public float g;
            public long b;

            public static Item2 Generate()
            {
                Item2 item = new Item2();
                item.r = Random.Range(0f, 1f);
                item.g = Random.Range(-100f, 100f);
                item.b = Random.Range(5, 10000);
                item.id = item.b + " item2";
                return item;
            }
        }
    }
}