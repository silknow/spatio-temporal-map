/*     INFINITY CODE 2013-2019      */
/*   http://www.infinity-code.com   */

using UnityEditor;

public abstract class OnlineMapsTiledElevationManagerEditor : OnlineMapsElevationManagerBaseEditor
{
    public SerializedProperty zoomOffset;

    protected override void CacheSerializedFields()
    {
        base.CacheSerializedFields();

        zoomOffset = serializedObject.FindProperty("zoomOffset");
    }

    protected override void GenerateLayoutItems()
    {
        base.GenerateLayoutItems();

        rootLayoutItem.Create(zoomOffset);
    }
}