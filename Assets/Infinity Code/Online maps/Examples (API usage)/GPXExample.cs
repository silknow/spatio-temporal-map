/*     INFINITY CODE 2013-2019      */
/*   http://www.infinity-code.com   */

using System.Text;
using UnityEngine;

#if !UNITY_WEBGL
using System.IO;
#endif

namespace InfinityCode.OnlineMapsExamples
{
    /// <summary>
    /// Example of work with GPX.
    /// </summary>
    [AddComponentMenu("Infinity Code/Online Maps/Examples (API Usage)/GPXExample")]
    public class GPXExample : MonoBehaviour
    {
        /// <summary>
        /// Creates a new GPX object.
        /// </summary>
        private void CreateNewGPX()
        {
            // Creates a new GPX object.
            OnlineMapsGPXObject gpx = new OnlineMapsGPXObject("Infinity Code");

            // Creates a meta.
            OnlineMapsGPXObject.Meta meta = gpx.metadata = new OnlineMapsGPXObject.Meta();
            meta.author = new OnlineMapsGPXObject.Person
            {
                email = new OnlineMapsGPXObject.EMail("support", "infinity-code.com"),
                name = "Infinity Code"
            };

            // Creates a bounds
            meta.bounds = new OnlineMapsGPXObject.Bounds(30, 10, 40, 20);

            // Creates a copyright
            meta.copyright = new OnlineMapsGPXObject.Copyright("Infinity Code")
            {
                year = 2016
            };

            // Creates a links
            meta.links.Add(new OnlineMapsGPXObject.Link("http://infinity-code.com/products/online-maps")
            {
                text = "Product Page"
            });

            // Creates a waypoints
            gpx.waypoints.AddRange(new[]
            {
                new OnlineMapsGPXObject.Waypoint(31, 12)
                {
                    description = "Point 1",
                },
                new OnlineMapsGPXObject.Waypoint(35, 82)
                {
                    description = "Point 2"
                }
            });

            // Creates a waypoints extensions
            foreach (OnlineMapsGPXObject.Waypoint wpt in gpx.waypoints)
            {
                OnlineMapsXML ext = wpt.extensions = new OnlineMapsXML("extensions");
                ext.Create("myField", wpt.description + "_1");
            }

            // Log GPX XML object
            Debug.Log(gpx);
        }

        /// <summary>
        /// Load GPX data from the file.
        /// </summary>
        private void LoadData()
        {
#if !UNITY_WEBGL
            string filename = "test.gpx";
            if (File.Exists(filename))
            {
                // Load data string
                string data = File.ReadAllText(filename, Encoding.UTF8);

                // Trying to to load GPX.
                OnlineMapsGPXObject gpx = OnlineMapsGPXObject.Load(data);

                // Log GPX XML object
                Debug.Log(gpx);
            }
#endif
        }

        private void Start()
        {
            // Load GPX data from the file.
            LoadData();

            // Creates a new GPX object.
            CreateNewGPX();
        }
    }
}