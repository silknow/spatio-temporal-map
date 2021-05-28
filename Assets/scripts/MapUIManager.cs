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
    public GameObject loadingDataGameObject;
    public GameObject helpWindowGameObject;
    public GameObject errorLoadingDataGameObject;
    public Text errorText;
    [Space]

    public GameObject UICanvas;
    public GameObject StackedMapCanvas;

    public GameObject FlatMapCamera;
    public GameObject StackedMapCamera;
    public GameObject StackedMapVirtualCamera;

    public GameObject ExitTimeVisualizationButton;
    public GameObject RelationsToggle;
    public GameObject FooterButtonBar;

    public MapViewingMode mapviewingMode = MapViewingMode.FLAT;
    private MapPointMarker _selectedMarker = null;

    [HideInInspector] public int selectedHelpTab;
    
    public void OnZoomIn()
    {
        OnlineMaps.instance.zoom += 1;
        Debug.Log("level " + SilkMap.instance.map.getLevel());
        //SilkMap.instance.map.SetDimension(3);
        //SilkMap.instance.map.showClusters();
    }

    public void OnZoomOut()
    {
        Debug.Log("level " + SilkMap.instance.map.getLevel());
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
        StackedMapManager.instance.UpdateSlicesRestrictions();
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
        HideLoadingData();
        HideHelpWindow();
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
        //UICanvas.SetActive(mapviewingMode == MapViewingMode.FLAT);

        if (mapviewingMode == MapViewingMode.FLAT)
            SilkMap.instance.removeSnapShots();

        if (mapviewingMode == MapViewingMode.STACKED)
        {
            StackedMapVirtualCamera.GetComponent<CameraFollowMap>().PopulateListOfMaps(maps.Select(m => m.transform).ToList());
        }
        StackedMapCamera.SetActive(mapviewingMode == MapViewingMode.STACKED);
        StackedMapCanvas.SetActive(mapviewingMode == MapViewingMode.STACKED);
        //Deactivate user interaction when mapmode is Stacked
        OnlineMapsCameraOrbit.instance.enabled = mapviewingMode == MapViewingMode.FLAT;
        OnlineMapsTileSetControl.instance.allowUserControl = mapviewingMode == MapViewingMode.FLAT;
        OnlineMapsTileSetControl.instance.allowZoom = mapviewingMode == MapViewingMode.FLAT;
        //Hide TimeVisualizationPanel
        if (mapviewingMode == MapViewingMode.STACKED && timeVisualizationPanelGameObject.activeSelf)
        {
            timeVisualizationPanelGameObject.SetActive(false);
        }
        
        //If MapviewingModed.STACKED -> disable all buttons except filters.
        
        ExitTimeVisualizationButton.SetActive(mapviewingMode == MapViewingMode.STACKED);
        RelationsToggle.SetActive(mapviewingMode != MapViewingMode.STACKED);

        foreach (var btn in FooterButtonBar.GetComponentsInChildren<Button>())
        {
            if (btn.gameObject.name != "Filter" && mapviewingMode == MapViewingMode.STACKED)
            {
                btn.interactable = false;
            }
            else
            {
                btn.interactable = true;
            }
        }
        
    }
    
    public void ShowProgressBar(int maxValue)
    {
        HideLoadingData();
        progressBarGameObject.SetActive(true);
        progressBarGameObject.GetComponentInChildren<Slider>().minValue = 0;
        progressBarGameObject.GetComponentInChildren<Slider>().maxValue = maxValue;
        progressBarGameObject.GetComponentInChildren<Slider>().value = 0;
    }
    public void UpdateProgressBar(string value)
    {
        //Debug.Log("Update progressbar Page: "+value);
        progressBarGameObject.GetComponentInChildren<Slider>().value = Int32.Parse(value);
    }
    public void HideProgressBar()
    {
        progressBarGameObject.SetActive(false);
    }
    public void ShowLoadingData()
    {
        loadingDataGameObject.SetActive(true);
    }
    public void HideLoadingData()
    {
        loadingDataGameObject.SetActive(false);
    }

    public void BlockTimeVisualizationPanel()
    {
        HideTimeVisualizationPanel();
        foreach (var btn in FooterButtonBar.GetComponentsInChildren<Button>())
        {
            btn.interactable = btn.gameObject.name != "Time";
        }

    }
    public void UnlockTimeVisualizationPanel()
    {
        foreach (var btn in FooterButtonBar.GetComponentsInChildren<Button>())
        {
            btn.interactable = true;
        }

    }

    public void ShowHelpWindow()
    {
        helpWindowGameObject.SetActive(true);
    }

    public void HideHelpWindow()
    {
        helpWindowGameObject.SetActive(false);
    }
    public void ShowErrorLoadingData(string text=null)
    {
        errorLoadingDataGameObject.SetActive(true);
        if (text != null)
            errorText.text = text;
    }
    public void HideErrorLoadingData()
    {
        errorLoadingDataGameObject.SetActive(false);
    }
}
