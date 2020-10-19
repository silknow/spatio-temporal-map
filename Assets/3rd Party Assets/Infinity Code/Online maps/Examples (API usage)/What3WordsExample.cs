/*         INFINITY CODE         */
/*   https://infinity-code.com   */

using UnityEngine;

namespace InfinityCode.OnlineMapsExamples
{
    /// <summary>
    /// Example of work with What3Words
    /// </summary>
    [AddComponentMenu("Infinity Code/Online Maps/Examples (API Usage)/What3WordsExample")]
    public class What3WordsExample : MonoBehaviour
    {
        /// <summary>
        /// What3Words API key
        /// </summary>
        public string key;

        private string words = "";

        private void OnGUI()
        {
            words = GUILayout.TextField(words, GUILayout.ExpandWidth(true));
            if (GUILayout.Button("Forward"))
            {
                // Forward geocodes a 3 word address to a position, expressed as coordinates of latitude and longitude.
                OnlineMapsWhat3Words.Forward(key, words).OnComplete += OnForwardComplete;
            }
            if (GUILayout.Button("Suggestions"))
            {
                // Returns a list of 3 word addresses based on user input and other parameters.
                OnlineMapsWhat3Words.AutoSuggest(key, words, true, null, OnlineMaps.instance.position).OnComplete += OnSuggestComplete;
            }
            if (GUILayout.Button("Blends"))
            {
                // Returns a blend of the three most relevant 3 word address candidates 
                // for a given location, based on a full or partial 3 word address.
                // The specified 3 word address may either be a full 3 word address or 
                // a partial 3 word address containing the first 2 words in full and 
                // at least 1 character of the 3rd word.StandardBlend provides the search logic 
                // that powers the search box on map.what3words.com and in the what3words mobile apps.
                OnlineMapsWhat3Words.StandardBlend(key, words, true, null, OnlineMaps.instance.position).OnComplete += OnSuggestComplete;
            }
            if (GUILayout.Button("Grid"))
            {
                // Returns a section of the 3m x 3m what3words grid for a given area.
                OnlineMapsWhat3Words.Grid(key, OnlineMaps.instance.bounds).OnComplete += OnGridComplete;
            }
            if (GUILayout.Button("Languages"))
            {
                // Retrieves a list of the currently loaded and available 3 word address languages.
                OnlineMapsWhat3Words.GetLanguages(key).OnComplete += OnLanguagesComplete;
            }

        }

        private void Start()
        {
            OnlineMapsControlBase.instance.OnMapClick += OnMapClick;
        }

        private void OnMapClick()
        {
            // Reverse geocodes coordinates, expressed as latitude and longitude to a 3 word address.
            OnlineMapsWhat3Words.Reverse(key, OnlineMaps.instance.position).OnComplete += OnReverseComplete;
        }

        private void OnReverseComplete(string response)
        {
            Debug.Log(response);

            // Converts the response string from Forward or Reverse geocoding to result object.
            OnlineMapsWhat3WordsFRResult result = OnlineMapsWhat3Words.GetForwardReverseResult(response);
            words = result.words;

            Debug.Log(OnlineMapsJSON.Serialize(result).ToString());
        }

        private void OnForwardComplete(string response)
        {
            Debug.Log(response);

            // Converts the response string from Forward or Reverse geocoding to result object.
            OnlineMapsWhat3WordsFRResult result = OnlineMapsWhat3Words.GetForwardReverseResult(response);
            OnlineMaps.instance.position = result.geometry;

            Debug.Log(OnlineMapsJSON.Serialize(result).ToString());
        }

        private void OnSuggestComplete(string response)
        {
            Debug.Log(response);

            // Converts the response string from AutoSuggest or StandardBlend to result object.
            OnlineMapsWhat3WordsSBResult result = OnlineMapsWhat3Words.GetSuggestionsBlendsResult(response);

            Debug.Log(OnlineMapsJSON.Serialize(result).ToString());
        }

        private void OnGridComplete(string response)
        {
            Debug.Log(response);

            // Converts the response string from Grid to result object.
            OnlineMapsWhat3WordsGridResult result = OnlineMapsWhat3Words.GetGridResult(response);

            Debug.Log(OnlineMapsJSON.Serialize(result).ToString());
        }

        private void OnLanguagesComplete(string response)
        {
            Debug.Log(response);

            // Converts the response string from Get Languages to result object.
            OnlineMapsWhat3WordsLanguagesResult result = OnlineMapsWhat3Words.GetLanguagesResult(response);

            Debug.Log(OnlineMapsJSON.Serialize(result).ToString());
        }
    }
}