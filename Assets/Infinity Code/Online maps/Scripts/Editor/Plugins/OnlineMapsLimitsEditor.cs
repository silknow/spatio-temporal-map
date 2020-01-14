/*     INFINITY CODE 2013-2019      */
/*   http://www.infinity-code.com   */

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(OnlineMapsLimits))]
public class OnlineMapsLimitsEditor:Editor
{
    private SerializedProperty pMinZoom;
    private SerializedProperty pMaxZoom;
    private SerializedProperty pMinLatitude;
    private SerializedProperty pMaxLatitude;
    private SerializedProperty pMinLongitude;
    private SerializedProperty pMaxLongitude;
    private SerializedProperty pUseZoomRange;
    private SerializedProperty pUsePositionRange;
    private SerializedProperty pPositionRangeType;

    private void CacheFields()
    {
        pMinZoom = serializedObject.FindProperty("minZoom");
        pMaxZoom = serializedObject.FindProperty("maxZoom");
        pMinLatitude = serializedObject.FindProperty("minLatitude");
        pMaxLatitude = serializedObject.FindProperty("maxLatitude");
        pMinLongitude = serializedObject.FindProperty("minLongitude");
        pMaxLongitude = serializedObject.FindProperty("maxLongitude");
        pUseZoomRange = serializedObject.FindProperty("useZoomRange");
        pUsePositionRange = serializedObject.FindProperty("usePositionRange");
        pPositionRangeType = serializedObject.FindProperty("positionRangeType");
    }

    private void DrawPositionRangeGUI()
    {
        EditorGUILayout.BeginHorizontal();
        pUsePositionRange.boolValue = EditorGUILayout.Toggle(pUsePositionRange.boolValue, GUILayout.Width(10));

        EditorGUI.BeginDisabledGroup(!pUsePositionRange.boolValue);
        EditorGUILayout.LabelField("Position Range");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.PropertyField(pMinLatitude);
        EditorGUILayout.PropertyField(pMaxLatitude);
        EditorGUILayout.PropertyField(pMinLongitude);
        EditorGUILayout.PropertyField(pMaxLongitude);
        EditorGUILayout.PropertyField(pPositionRangeType);

        if (pMinLatitude.floatValue < -90) pMinLatitude.floatValue = -90;
        else if (pMinLatitude.floatValue > 90) pMinLatitude.floatValue = 90;

        if (pMaxLatitude.floatValue < -90) pMaxLatitude.floatValue = -90;
        else if (pMaxLatitude.floatValue > 90) pMaxLatitude.floatValue = 90;

        if (pMinLongitude.floatValue < -180) pMinLongitude.floatValue = -180;
        else if (pMinLongitude.floatValue > 180) pMinLongitude.floatValue = 180;

        if (pMaxLongitude.floatValue < -180) pMaxLongitude.floatValue = -180;
        else if (pMaxLongitude.floatValue > 180) pMaxLongitude.floatValue = 180;

        EditorGUI.EndDisabledGroup();
    }

    private void DrawZoomRangeGUI()
    {
        EditorGUILayout.BeginHorizontal();
        pUseZoomRange.boolValue = EditorGUILayout.Toggle(pUseZoomRange.boolValue, GUILayout.Width(10));
        float min = pMinZoom.intValue;
        float max = pMaxZoom.intValue;
        EditorGUI.BeginChangeCheck();
        EditorGUI.BeginDisabledGroup(!pUseZoomRange.boolValue);

        EditorGUILayout.MinMaxSlider(new GUIContent("Zoom Range (" + min + "-" + max + ")"), ref min, ref max, OnlineMaps.MINZOOM, OnlineMaps.MAXZOOM);
        EditorGUI.EndDisabledGroup();
        if (EditorGUI.EndChangeCheck())
        {
            pMinZoom.intValue = Mathf.RoundToInt(min);
            pMaxZoom.intValue = Mathf.RoundToInt(max);
        }
        EditorGUILayout.EndHorizontal();
    }

    private void OnEnable()
    {
        CacheFields();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawZoomRangeGUI();
        DrawPositionRangeGUI();
        serializedObject.ApplyModifiedProperties();
    }
}