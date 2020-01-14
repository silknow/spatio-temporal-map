/*     INFINITY CODE 2013-2019      */
/*   http://www.infinity-code.com   */

using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(OnlineMapsEasyTouchConnector))]
public class OnlineMapsEasyTouchConnectorEditor : Editor
{
#if EASYTOUCH
    private OnlineMapsCameraOrbit cameraOrbit;
    private OnlineMapsEasyTouchConnector connector;

    private void OnEnable()
    {
        connector = target as OnlineMapsEasyTouchConnector;
        cameraOrbit = connector.GetComponent<OnlineMapsCameraOrbit>();
    }
#endif

    public override void OnInspectorGUI()
    {
#if !EASYTOUCH
        if (GUILayout.Button("Enable EasyTouch"))
        {
            if (EditorUtility.DisplayDialog("Enable EasyTouch", "You have EasyTouch in your project?", "Yes, I have EasyTouch", "Cancel"))
            {
                OnlineMapsEditor.AddCompilerDirective("EASYTOUCH");
            }
        }
#else 
        EditorGUILayout.HelpBox("This component does not require configuration.", MessageType.Info);
        if (cameraOrbit == null)
        {
            EditorGUILayout.HelpBox("To use twist and tilt gestures, add Online Maps Camera Orbit component.", MessageType.Warning);
            if (GUILayout.Button("Add Camera Orbit"))
            {
                connector.gameObject.AddComponent<OnlineMapsCameraOrbit>();
            }
        }
#endif
    }
}