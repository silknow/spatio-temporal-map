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
        if (GUILayout.Button("Test France Textiles"))
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
        if (GUILayout.Button("Stacked Map 3"))
        {            
            apm.OnStackedMap(3);
           
        }
        if (GUILayout.Button("Flat Map"))
        {            
            apm.OnFlatMap();
           
        }
    }
}
