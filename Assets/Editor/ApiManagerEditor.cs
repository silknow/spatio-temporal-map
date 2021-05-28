using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEditor;

[CustomEditor(typeof(APIManager))]
public class ApiManagerEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var apm = (APIManager)target;
        if (GUILayout.Button("Test Damask"))
        {            
            apm.OnButtonTestClick();
           
        }
        if (GUILayout.Button("Test Spain Textiles"))
        {            
            apm.TestFranceTextiles();
           
        }
        if (GUILayout.Button("Test Load Textiles from HTML"))
        {
            apm.TestLoadTextilesFromHTML();
        }
        if (GUILayout.Button("Test Object Detail"))
        {            
            apm.TestObjectDetail();
           
        }
        if (GUILayout.Button("Flat Map"))
        {            
            apm.OnFlatMap();
           
        }
        if (GUILayout.Button("Toggle Language "))
        {            
            apm.ToggleLanguage();
           
        }
        if (GUILayout.Button("Apply Damask Filter"))
        {            
            SilkMap.instance.map.addFilter("technique", "Damask");
            SilkMap.instance.map.applyFilters();
        }
        if (GUILayout.Button("Remove Damask Filter"))
        {            
            SilkMap.instance.map.removeFilter("technique", "Damask");
            SilkMap.instance.map.applyFilters();
        }
        if (GUILayout.Button("Take Screenshot"))
        {            
            ScreenCapture.CaptureScreenshot(Application.streamingAssetsPath + "/"+ Time.realtimeSinceStartup + ".png", 4);
        }
    }
}
