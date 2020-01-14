/*     INFINITY CODE 2013-2019      */
/*   http://www.infinity-code.com   */

using UnityEditor;

[CustomEditor(typeof(OnlineMapsControlBase2D), true)]
public abstract class OnlineMapsControlBase2DEditor<T> : OnlineMapsControlBaseEditor<T>
    where T: OnlineMapsControlBase2D
{

}