/*     INFINITY CODE 2013-2019      */
/*   http://www.infinity-code.com   */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

#if UNITY_2018_3_OR_NEWER
using UnityEngine.Networking;
#endif

/// <summary>
/// The wrapper class for WWW.\n
/// It allows you to control requests.\n
/// To create is recommended to use OnlineMapsUtils.GetWWW.
/// </summary>
public class OnlineMapsWWW: CustomYieldInstruction, IDisposable
{
    #region Actions

    /// <summary>
    /// This event is occurs when URL is initialized, and allows you to modify it.
    /// </summary>
    public static Func<string, string> OnInit;

    /// <summary>
    /// Events to validate the request. Return false if you want to cancel the request.
    /// </summary>
    public static Predicate<string> OnValidate;

    /// <summary>
    /// Event that occurs when a request is completed.
    /// </summary>
    public Action<OnlineMapsWWW> OnComplete;

    #endregion

    #region Variables

    private static MonoBehaviour temponaryBehavour;

    private byte[] _bytes;
    private Dictionary<string, object> _customFields;
    private string _error;
    private string _id;
    private bool _isDone;
    private string _url;
    private MonoBehaviour currentCoroutineBehaviour;
    private bool isYield = false;
    private string responseHeadersString;
    private RequestType type;
    private IEnumerator waitResponse;

#if UNITY_2018_3_OR_NEWER
    private UnityWebRequest www;
#else
    private WWW www;
#endif

    #endregion

    #region Properties

    /// <summary>
    /// Gets / sets custom fields value by key
    /// </summary>
    /// <param name="key">Custom field key</param>
    /// <returns>Custom field value</returns>
    public object this[string key]
    {
        get
        {
            object val;
            return customFields.TryGetValue(key, out val) ? val : null;
        }
        set { customFields[key] = value; }
    }

    /// <summary>
    /// Returns the contents of the fetched web page as a byte array.
    /// </summary>
    public byte[] bytes
    {
        get
        {
            if (type == RequestType.www)
            {
#if UNITY_2018_3_OR_NEWER
                return www.downloadHandler.data;
#else
                return www.bytes;
#endif
            }
            return _bytes;
        }
    }

    /// <summary>
    /// The number of bytes downloaded by this query.
    /// </summary>
    public int bytesDownloaded
    {
        get
        {
            if (type == RequestType.www)
            {
#if UNITY_2018_3_OR_NEWER
                return (int)www.downloadedBytes;
#else
                return www.bytesDownloaded;
#endif
            }
            return _bytes != null ? _bytes.Length : 0;
        }
    }

    private static MonoBehaviour coroutineBehaviour
    {
        get
        {
            if (OnlineMaps.instance != null) return OnlineMaps.instance;
            if (temponaryBehavour == null)
            {
                GameObject go = new GameObject("__OnlineMapsWWW__");
                go.hideFlags = HideFlags.HideInHierarchy;
                temponaryBehavour = go.AddComponent<OnlineMapsWWWBehaviour>();
            }

            return temponaryBehavour;
        }
    }

    /// <summary>
    /// Gets customFields dictionary
    /// </summary>
    public Dictionary<string, object> customFields
    {
        get
        {
            if (_customFields == null) _customFields = new Dictionary<string, object>();
            return _customFields;
        }
    }

    /// <summary>
    /// Returns an error message if there was an error during the download.
    /// </summary>
    public string error
    {
        get
        {
            if (type == RequestType.www) return www.error;
            return _error;
        }
    }

    /// <summary>
    /// This property is true if the request has encountered an error.
    /// </summary>
    public bool hasError
    {
        get { return !string.IsNullOrEmpty(error); }
    }

    /// <summary>
    /// ID of query.
    /// </summary>
    public string id
    {
        get { return _id; }
    }

    /// <summary>
    /// Is the download already finished?
    /// </summary>
    public bool isDone
    {
        get
        {
            if (type == RequestType.www) return www != null && www.isDone;
            return _isDone;
        }
    }

    public override bool keepWaiting
    {
        get
        {
            isYield = true;
            return !isDone;
        }
    }

    /// <summary>
    /// Dictionary of headers returned by the request.
    /// </summary>
    public Dictionary<string, string> responseHeaders
    {
        get
        {
            if (!isDone) throw new UnityException("WWW is not finished downloading yet");

            if (type == RequestType.www)
            {
#if UNITY_2018_3_OR_NEWER
                return www.GetResponseHeaders();
#else
                return www.responseHeaders;
#endif
            }
            return ParseHTTPHeaderString(responseHeadersString);
        }
    }

    /// <summary>
    /// Returns the contents of the fetched web page as a string.
    /// </summary>
    public string text
    {
        get
        {
            if (type == RequestType.www)
            {
#if UNITY_2018_3_OR_NEWER
                return www.downloadHandler.text;
#else
                return www.text;
#endif
            }
            return _bytes != null ? GetTextEncoder().GetString(_bytes, 0, _bytes.Length) : null;
        }
    }

    /// <summary>
    /// The URL of this request.
    /// </summary>
    public string url
    {
        get { return _url; }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="url">URL of request</param>
    public OnlineMapsWWW(string url)
    {
        SetURL(url);
        type = RequestType.www;

        if (OnValidate != null && !OnValidate(url))
        {
            currentCoroutineBehaviour = coroutineBehaviour;
            currentCoroutineBehaviour.StartCoroutine(WaitCancel());
            return;
        }

#if UNITY_2018_3_OR_NEWER
        www = UnityWebRequest.Get(url);
        www.SendWebRequest();
#else
        www = new WWW(url);
#endif

        currentCoroutineBehaviour = coroutineBehaviour;
        currentCoroutineBehaviour.StartCoroutine(WaitResponse());
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="url">URL of request</param>
    public OnlineMapsWWW(StringBuilder url) : this(url.ToString())
    {
        
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="url">URL of request</param>
    /// <param name="type">Type of request</param>
    /// <param name="reqID">Request ID</param>
    public OnlineMapsWWW(string url, RequestType type, string reqID)
    {
        this.type = type;
        SetURL(url);

        if (OnValidate != null && !OnValidate(url))
        {
            currentCoroutineBehaviour = coroutineBehaviour;
            currentCoroutineBehaviour.StartCoroutine(WaitCancel());
            return;
        }

        _id = reqID;
        if (type == RequestType.www)
        {
#if UNITY_2018_3_OR_NEWER
            www = UnityWebRequest.Get(url);
            www.SendWebRequest();
#else
            www = new WWW(url);
#endif
            currentCoroutineBehaviour = coroutineBehaviour;
            currentCoroutineBehaviour.StartCoroutine(WaitResponse());
        }
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="www">WWW instance.</param>
#if UNITY_2018_3_OR_NEWER
    private OnlineMapsWWW(UnityWebRequest www)
#else
    private OnlineMapsWWW(WWW www)
#endif
    {
        SetURL(www.url);
        type = RequestType.www;

        if (OnValidate != null && !OnValidate(url))
        {
            currentCoroutineBehaviour = coroutineBehaviour;
            currentCoroutineBehaviour.StartCoroutine(WaitCancel());
            return;
        }

        this.www = www;
        waitResponse = WaitResponse();
        currentCoroutineBehaviour = coroutineBehaviour;
        currentCoroutineBehaviour.StartCoroutine(waitResponse);
    }

    /// <summary>
    /// Disposes of an existing object.
    /// </summary>
    public void Dispose()
    {
        if (www != null && !www.isDone) www.Dispose();
        www = null;
        _customFields = null;
        if (waitResponse != null) currentCoroutineBehaviour.StopCoroutine(waitResponse);
    }

    /// <summary>
    /// Escapes characters in a string to ensure they are URL-friendly.
    /// </summary>
    /// <param name="s">A string with characters to be escaped.</param>
    /// <returns>Escaped string.</returns>
    public static string EscapeURL(string s)
    {
#if UNITY_2018_3_OR_NEWER
        return UnityWebRequest.EscapeURL(s);
#else
        return WWW.EscapeURL(s);
#endif
    }

    private void Finish()
    {
        if (OnComplete != null) OnComplete(this);

        if (!hasError)
        {
#if UNITY_2018_3_OR_NEWER
            OnlineMapsLog.Info("Response: " + www.responseCode + " from " + url, OnlineMapsLog.Type.request);
#else
            string code;
            if (www.responseHeaders != null && www.responseHeaders.TryGetValue("STATUS", out code))
            {
                OnlineMapsLog.Info("Response: " + code + " from " + url, OnlineMapsLog.Type.request);
            }
#endif
        }
        else
        {
            OnlineMapsLog.Warning("Error: " + error + "\nfrom " + url, OnlineMapsLog.Type.request);
        }

        if (!isYield) Dispose();
    }

    private Encoding GetTextEncoder()
    {
        string str;
        if (responseHeaders.TryGetValue("CONTENT-TYPE", out str))
        {
            int index = str.IndexOf("charset", StringComparison.OrdinalIgnoreCase);
            if (index > -1)
            {
                int num2 = str.IndexOf('=', index);
                if (num2 > -1)
                {
                    char[] trimChars = { '\'', '"' };
                    string name = str.Substring(num2 + 1).Trim().Trim(trimChars).Trim();
                    int length = name.IndexOf(';');
                    if (length > -1)
                    {
                        name = name.Substring(0, length);
                    }
                    try
                    {
                        return Encoding.GetEncoding(name);
                    }
                    catch (Exception)
                    {
                        Debug.Log("Unsupported encoding: '" + name + "'");
                    }
                }
            }
        }
        return Encoding.UTF8;
    }

    /// <summary>
    /// Replaces the contents of an existing Texture2D with an image from the downloaded data.
    /// </summary>
    /// <param name="tex">An existing texture object to be overwritten with the image data.</param>
    public void LoadImageIntoTexture(Texture2D tex)
    {
        if (tex == null) throw new Exception("Texture is null");

        if (type == RequestType.www)
        {
#if UNITY_2018_3_OR_NEWER
            tex.LoadImage(bytes);
#else
            www.LoadImageIntoTexture(tex);
#endif
        }
        else tex.LoadImage(_bytes);
    }

    internal static Dictionary<string, string> ParseHTTPHeaderString(string input)
    {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();

        if (string.IsNullOrEmpty(input)) return dictionary;

        StringReader reader = new StringReader(input);
        int num = 0;
        while (true)
        {
            string str = reader.ReadLine();
            if (str == null)
            {
                return dictionary;
            }
            if ((num++ == 0) && str.StartsWith("HTTP"))
            {
                dictionary["STATUS"] = str;
            }
            else
            {
                int index = str.IndexOf(": ");
                if (index != -1)
                {
                    string str2 = str.Substring(0, index).ToUpper();
                    string str3 = str.Substring(index + 2);
                    dictionary[str2] = str3;
                }
            }
        }
    }

    /// <summary>
    /// Sets the contents and headers of the response for type = direct.
    /// </summary>
    /// <param name="responseHeadersString">Headers of response.</param>
    /// <param name="_bytes">Content of response.</param>
    public void SetBytes(string responseHeadersString, byte[] _bytes)
    {
        if (type == RequestType.www) throw new Exception("OnlineMapsWWW.SetBytes available only for type = direct.");

        this.responseHeadersString = responseHeadersString;
        this._bytes = _bytes;
        _isDone = true;
        Finish();
    }

    /// <summary>
    /// Sets the error for type = direct.
    /// </summary>
    /// <param name="errorStr">Error message.</param>
    public void SetError(string errorStr)
    {
        if (type == RequestType.www) throw new Exception("OnlineMapsWWW.SetError available only for type = direct.");
        _error = errorStr;
        _isDone = true;
        Finish();

    }

    private void SetURL(string url)
    {
#if UNITY_IOS
        url = url.Replace("|", "%7C");
#endif

#if UNITY_WEBGL && !UNITY_EDITOR
        if (OnlineMaps.instance.useProxy) 
        {
            if (url.Contains(".virtualearth.net")) url = OnlineMaps.instance.proxyURL + url;
        }
#endif
        if (OnInit != null) url = OnInit(url);
        OnlineMapsLog.Info(url, OnlineMapsLog.Type.request);
        _url = url;
    }

    private IEnumerator WaitCancel()
    {
        type = RequestType.direct;
        yield return null;

        _error = "Request canceled.";
        Finish();
    }

    private IEnumerator WaitResponse()
    {
        while (!www.isDone)
        {
            yield return null;
        }

        waitResponse = null;
        Finish();
    }

#if !UNITY_2018_3_OR_NEWER
    public static implicit operator OnlineMapsWWW(WWW val)
    {
        return new OnlineMapsWWW(val);
    }
#endif

#endregion

    /// <summary>
    /// Type of request.
    /// </summary>
    public enum RequestType
    {
        /// <summary>
        /// The request will be processed independently.\n
        /// Use OnlineMapsUtils.OnGetWWW to process of request.
        /// </summary>
        direct,

        /// <summary>
        /// The request will be processed using WWW or UnityWebClient class.
        /// </summary>
        www
    }
}