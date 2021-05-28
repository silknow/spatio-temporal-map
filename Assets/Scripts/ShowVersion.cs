using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Profiling.Memory.Experimental;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ShowVersion : MonoBehaviour
{
    // Start is called before the first frame update
    private int currentFPS;
    private Text uiText;
    [SerializeField]
    private Text fpsText;
    void Awake()
    {
        uiText = GetComponent<Text>();
        uiText.text = "v. " + Application.version;
    }
}
