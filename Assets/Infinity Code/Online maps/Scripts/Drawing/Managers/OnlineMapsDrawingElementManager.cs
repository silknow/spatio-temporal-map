/*     INFINITY CODE 2013-2019      */
/*   http://www.infinity-code.com   */

/// <summary>
/// This component manages drawing elements.
/// </summary>
public class OnlineMapsDrawingElementManager: OnlineMapsInteractiveElementManager<OnlineMapsDrawingElementManager, OnlineMapsDrawingElement>
{
    protected override void OnEnable()
    {
        base.OnEnable();

        _instance = this;
    }
}