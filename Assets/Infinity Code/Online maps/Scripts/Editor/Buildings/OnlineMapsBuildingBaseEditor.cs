/*     INFINITY CODE 2013-2019      */
/*   http://www.infinity-code.com   */

using UnityEditor;

[CustomEditor(typeof(OnlineMapsBuildingBase), true)]
public class OnlineMapsBuildingBaseEditor:Editor
{
    private OnlineMapsBuildingBase building;

    private void OnEnable()
    {
        building = target as OnlineMapsBuildingBase;
    }

    public override void OnInspectorGUI()
    {
        OnlineMapsBuildingBase.MetaInfo[] metaInfo = building.metaInfo;
        if (metaInfo == null) EditorGUILayout.LabelField("Meta count: " + 0);
        else
        {
            int metaCount = metaInfo.Length;
            EditorGUILayout.LabelField("Meta count: " + metaCount);
            EditorGUI.BeginDisabledGroup(true);
            foreach (OnlineMapsBuildingBase.MetaInfo item in metaInfo) EditorGUILayout.TextField(item.title, item.info);
            EditorGUI.EndDisabledGroup();
        }
    }
}