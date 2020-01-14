/*     INFINITY CODE 2013-2019      */
/*   http://www.infinity-code.com   */

using UnityEditor;

[CustomEditor(typeof(OnlineMapsElevationManagerBase), true)]
public abstract class OnlineMapsElevationManagerBaseEditor : OnlineMapsFormattedEditor
{
    protected SerializedProperty bottomMode;
    protected SerializedProperty scale;
    protected SerializedProperty zoomRange;
    protected SerializedProperty lockYScale;
    protected SerializedProperty yScaleValue;

    protected override void CacheSerializedFields()
    {
        bottomMode = serializedObject.FindProperty("bottomMode");
        scale = serializedObject.FindProperty("scale");
        zoomRange = serializedObject.FindProperty("zoomRange");
        lockYScale = serializedObject.FindProperty("lockYScale");
        yScaleValue = serializedObject.FindProperty("yScaleValue");
    }

    protected override void GenerateLayoutItems()
    {
        base.GenerateLayoutItems();

        rootLayoutItem.Create(bottomMode);
        rootLayoutItem.Create(scale);
        rootLayoutItem.Create(zoomRange);
        rootLayoutItem.Create(lockYScale);
        rootLayoutItem.Create(yScaleValue).OnValidateDraw += () => lockYScale.boolValue;
    }
}