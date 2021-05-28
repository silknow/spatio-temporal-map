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
        if (GUILayout.Button("Load Dataset 300 "))
        {            
            apm.LoadDataset(300);
           
        }
        if (GUILayout.Button("Load Dataset 3000 "))
        {            
            apm.LoadDataset(3000);
           
        }
        if (GUILayout.Button("Load Dataset 15000"))
        {            
            apm.LoadDataset(15000);
           
        }
        if (GUILayout.Button("Load Dataset 30000 "))
        {            
            apm.LoadDataset(30000);
           
        }
        if (GUILayout.Button("Flat Map"))
        {            
            apm.OnFlatMap();
           
        }
        if (GUILayout.Button("Toggle Language "))
        {            
            apm.ToggleLanguage();
           
        }
       
        if (GUILayout.Button("Load JSON FROM STREAM"))
        {
            apm.LoadJSONFromStream();

        }
        if (GUILayout.Button("Take Screenshot"))
        {            
            ScreenCapture.CaptureScreenshot(Application.streamingAssetsPath + "/"+ Time.realtimeSinceStartup + ".png", 4);
        }

    }
}
