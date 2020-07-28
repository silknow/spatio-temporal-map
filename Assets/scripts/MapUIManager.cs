using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MapUIManager : Singleton<MapUIManager>
{
    [Serializable]
    public enum MapViewingMode
    {
        FLAT,
        STACKED
    }
    
    public GameObject filterPanelGameObject;
    public GameObject timeVisualizationPanelGameObject;
    public GameObject detailsPanelGameObject;
    public GameObject timeSliderGameObject;
    public GameObject progressBarGameObject;
    [Space]

    public GameObject UICanvas;
    public GameObject StackedMapCanvas;

    public GameObject FlatMapCamera;
    public GameObject StackedMapCamera;
    public GameObject StackedMapVirtualCamera;

    public MapViewingMode mapviewingMode = MapViewingMode.FLAT;
    
    private MapPointMarker _selectedMarker = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnZoomIn()
    {
        OnlineMaps.instance.zoom += 1;
    }

    public void OnZoomOut()
    {
        OnlineMaps.instance.zoom -= 1;
    }
    public void OnAboutClick()
    {
        OnlineMaps.instance.zoom -= 1;
    }

    public void ShowFiltersPanel()
    {
        filterPanelGameObject.GetComponent<SlidePanel>().ShowAndEnablePanel();
    }
    public void HideFiltersPanel()
    {
        filterPanelGameObject.GetComponent<SlidePanel>().HideAndDisablePanel();
    }
    public void ShowTimeVisualizationPanel()
    {
        timeVisualizationPanelGameObject.SetActive(true);
    }
    public void HideTimeVisualizationPanel()
    {
        
        timeVisualizationPanelGameObject.SetActive(false);
    }
    public void ToggleFiltersPanel()
    {
        if (filterPanelGameObject.activeInHierarchy)
        {
            HideFiltersPanel();
        }
        else
        {
            ShowFiltersPanel();
        }
    }
    public void SetSelectedMarker(MapPointMarker marker)
    {
        _selectedMarker = marker;
    }
    public MapPointMarker GetSelectedMarker()
    {
        return _selectedMarker;
    }
    public void ShowDetailsPanel()
    {
        if (_selectedMarker != null)
        {
            detailsPanelGameObject.GetComponent<ObjectDetailPanel>().SetData(_selectedMarker.getURI());
            HideFiltersPanel();
            HideTimeVisualizationPanel();
            
        }

    }

    public void OnLoadedData()
    {
        HideAllPanels();
        UpdateFiltersPanel();
        UpdateTimeSlider();
    }
    public void UpdateFiltersPanel()
    {
        var propertyManager = SilkMap.instance.map.GetPropertyManager();
        //Reset Filters.
        SilkMap.instance.map.resetFilters();
        filterPanelGameObject.GetComponent<FiltersPanel>().SetProperties(propertyManager.GetFilteredProperties());
    }
    
    public void HideAllPanels()
    {
        _selectedMarker = null;
        detailsPanelGameObject.SetActive(false);
        HideTimeVisualizationPanel();
        HideTimeSlider();
        HideFiltersPanel();
    }
    public void ShowTimeSlider()
    {
        if (timeSliderGameObject != null)
        {
            timeSliderGameObject.GetComponent<TimeSlider>().initSliderOnShow();
            timeSliderGameObject.SetActive(true);
            HideTimeVisualizationPanel();
        }

    }

    public void HideTimeSlider()
    {
        timeSliderGameObject.SetActive(false);
        SilkMap.instance.map.removeTimeFrame();
    }

    public void UpdateTimeSlider()
    {
        var propertyManager = SilkMap.instance.map.GetPropertyManager();
        var prop = propertyManager.GetPropertyByName("time");
        timeSliderGameObject.GetComponent<TimeSlider>().SetPropertyValues(prop);
    }

    public void ToggleMapViewingMode(List<GameObject> maps = null)
    {
        mapviewingMode = mapviewingMode == MapViewingMode.FLAT ? MapViewingMode.STACKED : MapViewingMode.FLAT;
        FlatMapCamera.SetActive(mapviewingMode == MapViewingMode.FLAT);
        UICanvas.SetActive(mapviewingMode == MapViewingMode.FLAT);
        if (mapviewingMode == MapViewingMode.STACKED)
        {
            StackedMapVirtualCamera.GetComponent<CameraFollowMap>().PopulateListOfMaps(maps.Select(m => m.transform).ToList());
        }
        StackedMapCamera.SetActive(mapviewingMode == MapViewingMode.STACKED);
        StackedMapCanvas.SetActive(mapviewingMode == MapViewingMode.STACKED);
    }
    
    public void ShowProgressBar(int maxValue)
    {
        progressBarGameObject.SetActive(true);
        progressBarGameObject.GetComponentInChildren<Slider>().minValue = 0;
        progressBarGameObject.GetComponentInChildren<Slider>().maxValue = maxValue;
        progressBarGameObject.GetComponentInChildren<Slider>().value = 0;
    }
    public void UpdateProgressBar(string value)
    {
        Debug.Log("Update progressbar Page: "+value);
        progressBarGameObject.GetComponentInChildren<Slider>().value = Int32.Parse(value);
    }
    public void HideProgressBar()
    {
        progressBarGameObject.SetActive(false);
    }
}
