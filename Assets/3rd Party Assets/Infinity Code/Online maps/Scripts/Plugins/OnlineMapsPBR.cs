/*         INFINITY CODE         */
/*   https://infinity-code.com   */

using System;
using UnityEngine;

/// <summary>
/// Bridge for PBR (Physically Based Rendering)
/// </summary>
[Serializable]
[OnlineMapsPlugin("PBR Bridge", typeof(OnlineMapsControlBaseDynamicMesh))]
[AddComponentMenu("Infinity Code/Online Maps/Plugins/PBR Bridge")]
public class OnlineMapsPBR : MonoBehaviour
{
    /// <summary>
    /// Shader property for tiling.
    /// </summary>
    public string tilePropertyName = "_MainTexTile";

    /// <summary>
    /// Shader property for offset.
    /// </summary>
    public string offsetPropertyName = "_MainTexOffset";

    private void OnDrawTile(OnlineMapsTile tile, Material material)
    {
        Vector2 scale = material.mainTextureScale;
        Vector2 offset = material.mainTextureOffset;
        material.SetVector(tilePropertyName, scale);
        material.SetVector(offsetPropertyName, offset);
    }

    private void OnEnable()
    {
        OnlineMapsTileSetControl control = GetComponent<OnlineMapsTileSetControl>();
        control.OnDrawTile -= OnDrawTile;
        control.OnDrawTile += OnDrawTile;
    }
}