using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Category", menuName = "Maps", order = 0)]
public class ScriptableCategory : ScriptableObject
{
    public Texture2D defaultIcon;
    public Texture2D multipleLocationIcon;

    public object Clone()
    {
        var result = CreateInstance<ScriptableCategory>();
        result.defaultIcon = defaultIcon;
        result.multipleLocationIcon = multipleLocationIcon;
        return result;
    }
}
