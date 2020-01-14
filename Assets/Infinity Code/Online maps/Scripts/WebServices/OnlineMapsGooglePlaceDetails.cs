/*     INFINITY CODE 2013-2019      */
/*   http://www.infinity-code.com   */

using System.Text;

/// <summary>
/// A Place Details request returns more comprehensive information about the indicated place such as its complete address, phone number, user rating and reviews.\n
/// <strong>Requires Google Maps API key.</strong>\n
/// https://developers.google.com/places/webservice/details
/// </summary>
public class OnlineMapsGooglePlaceDetails : OnlineMapsTextWebService
{
    protected OnlineMapsGooglePlaceDetails()
    {

    }

    protected OnlineMapsGooglePlaceDetails(string key, string place_id, string reference, string language)
    {
        _status = OnlineMapsQueryStatus.downloading;

        if (string.IsNullOrEmpty(key)) key = OnlineMapsKeyManager.GoogleMaps();

        StringBuilder url = new StringBuilder("https://maps.googleapis.com/maps/api/place/details/xml?sensor=false&key=").Append(key);

        if (!string.IsNullOrEmpty(place_id)) url.Append("&placeid=").Append(place_id);
        if (!string.IsNullOrEmpty(reference)) url.Append("&reference=").Append(reference);
        if (!string.IsNullOrEmpty(language)) url.Append("&language=").Append(language);

        www = new OnlineMapsWWW(url);
        www.OnComplete += OnRequestComplete;
    }

    /// <summary>
    /// Gets details about the place by place_id.
    /// </summary>
    /// <param name="key">
    /// Your application's API key.\n
    /// This key identifies your application for purposes of quota management and so that places added from your application are made immediately available to your app.\n
    /// Visit the Google Developers Console to create an API Project and obtain your key.
    /// </param>
    /// <param name="place_id">A textual identifier that uniquely identifies a place, returned from a Place Search.</param>
    /// <param name="language">
    /// The language code, indicating in which language the results should be returned, if possible.\n
    /// Note that some fields may not be available in the requested language.
    /// </param>
    /// <returns>Query instance to the Google API.</returns>
    public static OnlineMapsGooglePlaceDetails FindByPlaceID(string key, string place_id, string language = null)
    {
        return new OnlineMapsGooglePlaceDetails(key, place_id, null, language);
    }

    /// <summary>
    /// Gets details about the place by reference.
    /// </summary>
    /// <param name="key">
    /// Your application's API key. \n
    /// This key identifies your application for purposes of quota management and so that places added from your application are made immediately available to your app.\n
    /// Visit the Google Developers Console to create an API Project and obtain your key.
    /// </param>
    /// <param name="reference">
    /// A textual identifier that uniquely identifies a place, returned from a Place Search.\n
    /// Note: The reference is now deprecated in favor of placeid.
    /// </param>
    /// <param name="language">
    /// The language code, indicating in which language the results should be returned, if possible.\n
    /// Note that some fields may not be available in the requested language.
    /// </param>
    /// <returns>Query instance to the Google API.</returns>
    public static OnlineMapsGooglePlaceDetails FindByReference(string key, string reference, string language = null)
    {
        return new OnlineMapsGooglePlaceDetails(key, null, reference, language);
    }

    /// <summary>
    /// Converts response into an result object.
    /// Note: The object may not contain all the available fields.\n
    /// Other fields can be obtained from OnlineMapsGooglePlaceDetailsResult.node.
    /// </summary>
    /// <param name="response">Response of Google API.</param>
    /// <returns>Result object or null.</returns>
    public static OnlineMapsGooglePlaceDetailsResult GetResult(string response)
    {
        try
        {
            OnlineMapsXML xml = OnlineMapsXML.Load(response);
            string status = xml.Find<string>("//status");
            if (status != "OK") return null;

            return new OnlineMapsGooglePlaceDetailsResult(xml["result"]);
        }
        catch
        {
        }

        return null;
    }
}