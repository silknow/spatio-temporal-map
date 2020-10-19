/*         INFINITY CODE         */
/*   https://infinity-code.com   */

using UnityEngine;

namespace InfinityCode.OnlineMapsExamples
{
    /// <summary>
    /// Example of how to handle interaction events with buildings.
    /// </summary>
    [AddComponentMenu("Infinity Code/Online Maps/Examples (API Usage)/BuildingClickExample")]
    public class BuildingClickExample : MonoBehaviour
    {
        /// <summary>
        /// This method is called when click on building
        /// </summary>
        /// <param name="building">The building on which clicked</param>
        private void OnBuildingClick(OnlineMapsBuildingBase building)
        {
            Debug.Log("click: " + building.id);
        }

        /// <summary>
        /// This method is called when each building is created
        /// </summary>
        /// <param name="building">The building that was created</param>
        private void OnBuildingCreated(OnlineMapsBuildingBase building)
        {
            // Subscribe to interaction events
            building.OnPress += OnBuildingPress;
            building.OnRelease += OnBuildingRelease;
            building.OnClick += OnBuildingClick;
        }

        /// <summary>
        /// This method is called when press on building
        /// </summary>
        /// <param name="building">The building on which pressed</param>
        private void OnBuildingPress(OnlineMapsBuildingBase building)
        {
            Debug.Log("Press: " + building.id);
        }

        /// <summary>
        /// This method is called when release on building
        /// </summary>
        /// <param name="building">The building on which released</param>
        private void OnBuildingRelease(OnlineMapsBuildingBase building)
        {
            Debug.Log("Release: " + building.id);
        }

        /// <summary>
        /// This method is called when the script starts
        /// </summary>
        private void Start()
        {
            // Subscribe to the building creation event
            OnlineMapsBuildings.instance.OnBuildingCreated += OnBuildingCreated;
        }
    }
}