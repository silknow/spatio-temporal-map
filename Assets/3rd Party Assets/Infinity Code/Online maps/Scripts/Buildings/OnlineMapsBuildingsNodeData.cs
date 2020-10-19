/*         INFINITY CODE         */
/*   https://infinity-code.com   */

using System.Collections.Generic;

/// <summary>
/// It contains a dictionary of nodes and way of a building contour.
/// </summary>
public class OnlineMapsBuildingsNodeData
{
    /// <summary>
    /// Way of a building contour.
    /// </summary>
    public OnlineMapsOSMWay way;

    /// <summary>
    /// Dictionary of nodes.
    /// </summary>
    public Dictionary<string, OnlineMapsOSMNode> nodes;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="way">Way of a building contour.</param>
    /// <param name="nodes">Dictionary of nodes.</param>
    public OnlineMapsBuildingsNodeData(OnlineMapsOSMWay way, Dictionary<string, OnlineMapsOSMNode> nodes)
    {
        this.way = way;
        this.nodes = nodes;
    }

    /// <summary>
    /// Disposes this object.
    /// </summary>
    public void Dispose()
    {
        way = null;
        nodes = null;
    }
}