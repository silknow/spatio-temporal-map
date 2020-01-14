/*     INFINITY CODE 2013-2019      */
/*   http://www.infinity-code.com   */

using UnityEngine;

/// <summary>
/// Result of Google Maps Elevation query.
/// </summary>
public class OnlineMapsGoogleElevationResult
{
    /// <summary>
    /// Elevation of the location in meters.
    /// </summary>
    public float elevation;

    /// <summary>
    /// Position for which elevation data is being computed. \n
    /// Note that for path requests, the set of location elements will contain the sampled points along the path.
    /// </summary>
    public Vector2 location;

    /// <summary>
    /// Maximum distance between data points from which the elevation was interpolated, in meters. \n
    /// This property will be missing if the resolution is not known. \n
    /// Note that elevation data becomes more coarse (larger resolution values) when multiple points are passed. \n
    /// To obtain the most accurate elevation value for a point, it should be queried independently.
    /// </summary>
    public float resolution;

    public OnlineMapsGoogleElevationResult(){}

    public OnlineMapsGoogleElevationResult(OnlineMapsXML node)
    {
        elevation = node.Get<float>("elevation");
        resolution = node.Get<float>("resolution");

        OnlineMapsXML locationNode = node["location"];
        location = new Vector2(locationNode.Get<float>("lng"), locationNode.Get<float>("lat"));
    }
}