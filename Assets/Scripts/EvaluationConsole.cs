using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class EvaluationConsole : Singleton<EvaluationConsole>
{
    [DllImport("__Internal")]
    private static extern void WriteToConsole(string str);

   
    public void AddLine(string newLine)
    {
        #if UNITY_EDITOR
            Debug.Log(newLine);
        #elif UNITY_WEBGL
            WriteToConsole(newLine);
        #endif
    }
}
