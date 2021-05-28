
using System;
using UnityEngine;
using UnityEngine.UI;

    public class MapItemPopup : Singleton<MapItemPopup>
    {
        /// <summary>
        /// Root canvas
        /// </summary>
        public Canvas canvas;

        /// <summary>
        /// Popup for a single object
        /// </summary>
        public GameObject itemTitlePopup;
        
        /// <summary>
        /// Popup for a cluster
        /// </summary>
        public GameObject clusterPopup;

        /// <summary>
        /// Title text
        /// </summary>
        public Text title;
        
        /// <summary>
        /// Reference to active marker
        /// </summary>
        private MapPointMarker targetMarker;

        private GameObject selectedPopup;
        
        private Vector2 prevPosition = Vector2.one;
        
        
        /// <summary>
        /// This method is called by clicking on the map
        /// </summary>
        private void OnMapClick()
        {
            // Remove active marker reference
            targetMarker = null;
            
            // Hide the popup
            selectedPopup?.SetActive(false);
            
            // Remove active popup reference
            selectedPopup = null;
        }

        /// <summary>
        /// This method is called by clicking on the marker
        /// </summary>
        /// <param name="marker">The marker on which clicked</param>
        public void OnMarkerClick(MapPointMarker marker)
        {
           
            targetMarker = marker;
            //print("OnMarker click on "+targetMarker.getLabel());
            // Set active marker reference
            if(targetMarker.isGroupPoint())
            {
                
                // Show the popup
                itemTitlePopup.SetActive(false);
                selectedPopup = clusterPopup;

                var mcp = selectedPopup.GetComponent<MapClusterPopup>();
                mcp.RemoveChildren();
                //llamar a getfilteredclusteredpoints;
                foreach (var clusterPoint in targetMarker.getClusteredPointsNoFiltered())
                {
                    mcp.AddClusterItem(clusterPoint.getLabel(),clusterPoint.getURI());
                    //print("hola");
                }
                
                
                selectedPopup.SetActive(true);
               
            }
            else if (!targetMarker.isCluster())
            {
                // Show the popup
                clusterPopup.SetActive(false);
                selectedPopup = itemTitlePopup;
                selectedPopup.SetActive(true);
                
                // Set title and address
                title.text = marker.getLabel();
                MapUIManager.instance.SetSelectedMarker(targetMarker);
                
                selectedPopup.GetComponent<RelationsLegendPopup>().ClearRows();
                selectedPopup.GetComponent<RelationsLegendPopup>().PopulateLegend();

                
            }
            
            else
            {
                targetMarker = null;
            }

            UpdatePopupPosition();
        }

        /// <summary>
        /// Start is called on the frame when a script is enabled just before any of the Update methods are called the first time.
        /// </summary>
        private void Start()
        {
            // Subscribe to events of the map 
            //
            //
            OnlineMaps.instance.OnChangePosition += UpdatePopupPosition;
            OnlineMaps.instance.OnChangeZoom += UpdatePopupPosition;
            OnlineMapsControlBase.instance.OnMapClick += OnMapClick;
/*
            if (OnlineMapsControlBaseDynamicMesh.instance != null)
            {
                OnlineMapsControlBaseDynamicMesh.instance.OnMeshUpdated += UpdatePopupPosition;
            }

            if (OnlineMapsCameraOrbit.instance != null)
            {
                OnlineMapsCameraOrbit.instance.OnCameraControl += UpdatePopupPosition;
            }
            */

            // Initial hide popup
            itemTitlePopup.SetActive(false);
        }

        /// <summary>
        /// Updates the popup position
        /// </summary>
        private void UpdatePopupPosition()
        {
            // If no marker is selected then exit.
            if (targetMarker == null) return;

            var selectedMarker = targetMarker.getDimension() == MapPoint.THREE_DIMENSION
                ? targetMarker.getMarker3D() as OnlineMapsMarkerBase
                : targetMarker.getMarker2D();
            
            // PABLO - REVISAR 
            var isGroupPoints = false;
            if(targetMarker.getGridCluster() != null)
                isGroupPoints = targetMarker.getGridCluster().isGroupPoints();
            // Hide the popup if the marker is outside the map view
            if (!selectedMarker.inMapView)
            {
                if (selectedPopup.activeSelf)
                {
                    selectedPopup.SetActive(false);
                    return;
                }

            }
            else if (!selectedPopup.activeSelf) selectedPopup.SetActive(true);

            // Convert the coordinates of the marker to the screen position.
            Vector2 screenPosition = OnlineMapsControlBase.instance.GetScreenPosition(selectedMarker.position);
            if (Vector2.Distance(prevPosition, screenPosition) > 1f)
            {
                prevPosition = screenPosition;
            }
            else
            {
                prevPosition = screenPosition;
                return;
            }
            //Debug.Log(screenPosition);
            // Add marker height
            //screenPosition.y += selectedMarker.height;

            // Get a local position inside the canvas.
            Vector2 point;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, screenPosition, null, out point);

            // Set local position of the popup
            (selectedPopup.transform as RectTransform).localPosition = point;
        }

        public void HidePopup()
        {
            if (selectedPopup != null)
            {
                selectedPopup.SetActive(false);
                targetMarker = null;
            }
        }

    }
