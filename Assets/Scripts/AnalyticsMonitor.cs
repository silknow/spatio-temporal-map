using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Networking;

public class AnalyticsMonitor : Singleton<AnalyticsMonitor>
{
    public string server;
    private bool sendAnalytics = false;
    void Start()
    {
        Analytics.enabled = false;
        #if UNITY_EDITOR
        sendAnalytics = true;
        Analytics.enabled = true;
        #endif
    }

    public void SetAnalyticsStatus(bool value)
    {
        Analytics.enabled = value;
        sendAnalytics = value;
    }

    public void sendEvent(string name, Dictionary<string, object> dictionary)
    {
        if (!enabled || !sendAnalytics) 
            return;
        Dictionary<string,string> dict2 = new Dictionary<string, string>();
        dictionary["sessionId"] = AnalyticsSessionInfo.sessionId;
        dictionary["platform"] = Application.platform.ToString();
        dictionary["event"]=name;
        foreach (var entry in dictionary)
        {
            dict2[entry.Key] = entry.Value.ToString();
        }
        StartCoroutine(UploadToServer(dict2));
    }
    IEnumerator UploadToServer( Dictionary<string,string> eventDictionary)
    {
        using (UnityWebRequest www = UnityWebRequest.Post(server, eventDictionary))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ConnectionError )
            {
                Debug.Log(www.error);
            }
            else
            {
                //Debug.Log("Analytics upload complete!");
            }
        }
    }
}
