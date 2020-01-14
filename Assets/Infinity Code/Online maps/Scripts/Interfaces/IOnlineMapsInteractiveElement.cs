/*     INFINITY CODE 2013-2019      */
/*   http://www.infinity-code.com   */

/// <summary>
/// Interface for interactive elements
/// </summary>
public interface IOnlineMapsInteractiveElement
{
    IOnlineMapsInteractiveElementManager manager { get; set; }

    void DestroyInstance();

    /// <summary>
    /// Dispose the current interactive item.
    /// </summary>
    void Dispose();
}