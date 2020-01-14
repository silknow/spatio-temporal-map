/*     INFINITY CODE 2013-2019      */
/*   http://www.infinity-code.com   */

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// Retrieves information from Google Maps Place Autocomplete API.\n
/// Place Autocomplete service is a web service that returns place predictions.\n
/// The request specifies a textual search string and optional geographic bounds.\n 
/// The service can be used to provide autocomplete functionality for text-based geographic searches, by returning places such as businesses, addresses and points of interest as a user types.\n
/// <strong>Requires Google Maps API key.</strong>\n
/// https://developers.google.com/places/documentation/autocomplete
/// </summary>
public class OnlineMapsGooglePlacesAutocomplete: OnlineMapsTextWebService
{
    protected OnlineMapsGooglePlacesAutocomplete()
    {

    }

    protected OnlineMapsGooglePlacesAutocomplete(string input, string key, string types, int offset, Vector2 lnglat, int radius, string language, string components)
    {
        _status = OnlineMapsQueryStatus.downloading;

        if (string.IsNullOrEmpty(key)) key = OnlineMapsKeyManager.GoogleMaps();

        StringBuilder url = new StringBuilder("https://maps.googleapis.com/maps/api/place/autocomplete/xml?sensor=false");
        url.Append("&input=").Append(OnlineMapsWWW.EscapeURL(input));
        url.Append("&key=").Append(key);

        if (lnglat != default(Vector2)) url.AppendFormat("&location={0},{1}", lnglat.y, lnglat.x);
        if (radius != -1) url.Append("&radius=").Append(radius);
        if (offset != -1) url.Append("&offset=").Append(offset);
        if (!string.IsNullOrEmpty(types)) url.Append("&types=").Append(types);
        if (!string.IsNullOrEmpty(components)) url.Append("&components=").Append(components);
        if (!string.IsNullOrEmpty(language)) url.Append("&language=").Append(language);

        www = new OnlineMapsWWW(url);
        www.OnComplete += OnRequestComplete;
    }

    /// <summary>
    /// Creates a new request to the Google Maps Place Autocomplete API.
    /// </summary>
    /// <param name="input">
    /// The text string on which to search. \n
    /// The Place Autocomplete service will return candidate matches based on this string and order results based on their perceived relevance.
    /// </param>
    /// <param name="key">
    /// Your application's API key. This key identifies your application for purposes of quota management. \n
    /// Visit the Google APIs Console to select an API Project and obtain your key. 
    /// </param>
    /// <param name="types">The types of place results to return.</param>
    /// <param name="offset">
    /// The position, in the input term, of the last character that the service uses to match predictions. \n
    /// For example, if the input is 'Google' and the offset is 3, the service will match on 'Goo'. \n
    /// The string determined by the offset is matched against the first word in the input term only. \n
    /// For example, if the input term is 'Google abc' and the offset is 3, the service will attempt to match against 'Goo abc'. \n
    /// If no offset is supplied, the service will use the whole term. \n
    /// The offset should generally be set to the position of the text caret.
    /// </param>
    /// <param name="lnglat">The point around which you wish to retrieve place information.</param>
    /// <param name="radius">
    /// The distance (in meters) within which to return place results. \n
    /// Note that setting a radius biases results to the indicated area, but may not fully restrict results to the specified area.
    /// </param>
    /// <param name="language">The language in which to return results.</param>
    /// <param name="components">
    /// A grouping of places to which you would like to restrict your results. \n
    /// Currently, you can use components to filter by country. \n
    /// The country must be passed as a two character, ISO 3166-1 Alpha-2 compatible country code. \n
    /// For example: components=country:fr would restrict your results to places within France.
    /// </param>
    /// <returns>Query instance to the Google API.</returns>
    public static OnlineMapsGooglePlacesAutocomplete Find(string input, string key, string types = null, int offset = -1, Vector2 lnglat = default(Vector2), int radius = -1, string language = null, string components = null)
    {
        return new OnlineMapsGooglePlacesAutocomplete(
            input,
            key,
            types,
            offset,
            lnglat, 
            radius, 
            language, 
            components);
    }

    /// <summary>
    /// Converts response into an array of results.
    /// </summary>
    /// <param name="response">Response of Google API.</param>
    /// <returns>Array of result.</returns>
    public static OnlineMapsGooglePlacesAutocompleteResult[] GetResults(string response)
    {
        try
        {
            OnlineMapsXML xml = OnlineMapsXML.Load(response);
            string status = xml.Find<string>("//status");
            if (status != "OK") return null;

            List<OnlineMapsGooglePlacesAutocompleteResult> results = new List<OnlineMapsGooglePlacesAutocompleteResult>();

            OnlineMapsXMLList resNodes = xml.FindAll("//prediction");

            foreach (OnlineMapsXML node in resNodes)
            {
                results.Add(new OnlineMapsGooglePlacesAutocompleteResult(node));
            }

            return results.ToArray();
        }
        catch (Exception exception)
        {
            Debug.Log(exception.Message + "\n" + exception.StackTrace);
        }

        return null;
    }
}