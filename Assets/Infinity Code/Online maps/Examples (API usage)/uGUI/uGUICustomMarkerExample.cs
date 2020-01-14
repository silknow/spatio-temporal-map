/*     INFINITY CODE 2013-2019      */
/*   http://www.infinity-code.com   */

using UnityEngine;
using UnityEngine.UI;

namespace InfinityCode.OnlineMapsExamples
{
    /// <summary>
    /// Example of a marker instance for uGUICustomMarkerEngineExample.
    /// </summary>
    [AddComponentMenu("")]
    public class uGUICustomMarkerExample:MonoBehaviour
    {
        /// <summary>
        /// Longitude
        /// </summary>
        public double lng;

        /// <summary>
        /// Latitude
        /// </summary>
        public double lat;

        /// <summary>
        /// Reference to the TextField
        /// </summary>
        public Text textField;

        private string _text;

        /// <summary>
        /// Gets / sets the marker text
        /// </summary>
        public string text
        {
            get { return _text; }
            set
            {
                if (textField != null) textField.text = value;
                _text = value;
            }
        }

        /// <summary>
        /// Disposes the marker
        /// </summary>
        public void Dispose()
        {
            textField = null;
        }
    }
}