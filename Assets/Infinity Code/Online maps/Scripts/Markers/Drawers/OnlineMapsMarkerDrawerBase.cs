/*     INFINITY CODE 2013-2019      */
/*   http://www.infinity-code.com   */

/// <summary>
/// Base class for drawing markers
/// </summary>
public abstract class OnlineMapsMarkerDrawerBase
{
    protected OnlineMaps map;

    /// <summary>
    /// Dispose the current drawer
    /// </summary>
    public virtual void Dispose()
    {
        map = null;
    }
}