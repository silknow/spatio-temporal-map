/*         INFINITY CODE         */
/*   https://infinity-code.com   */

using UnityEngine;
using Debug = UnityEngine.Debug;

/// <summary>
/// Map log manager
/// </summary>
[OnlineMapsPlugin("Log", typeof(OnlineMapsControlBase))]
[AddComponentMenu("Infinity Code/Online Maps/Plugins/Log")]
public class OnlineMapsLog: MonoBehaviour
{
    private static OnlineMapsLog _instance;
    private static bool missed = false;

    public bool showRequests = false;

    public static OnlineMapsLog instance
    {
        get
        {
            if (_instance == null && !missed)
            {
                _instance = FindObjectOfType<OnlineMapsLog>();
                missed = _instance == null;
            }

            return _instance;
        }
    }

    public static void Info(string message, Type type)
    {
        if (!ValidateType(type)) return;
        Debug.Log(message);
    }

    private void OnEnable()
    {
        _instance = this;
    }

    private static bool ValidateType(Type type)
    {
        if (instance == null) return false;
        switch (type)
        {
            case Type.request:
                return instance.showRequests;
            default:
                return false;
        }
    }

    public static void Warning(string message, Type type)
    {
        if (!ValidateType(type)) return;
        Debug.LogWarning(message);
    }

    public enum Type
    {
        request
    }
}