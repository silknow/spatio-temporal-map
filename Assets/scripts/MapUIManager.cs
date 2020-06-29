using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapUIManager : Singleton<MapUIManager>
{
    public GameObject filterPanel;
    public GameObject detailsPanel;

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
        filterPanel.SetActive(true);
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
            detailsPanel.GetComponent<ObjectDetailPanel>().SetData(_selectedMarker.getURI());
            filterPanel.SetActive(false);
        }

    }
    public void UpdateFiltersPanel()
    {
        var propertyManager = SilkMap.instance.map.GetPropertyManager();
        //Reset Filters.
        SilkMap.instance.map.resetFilters();
        filterPanel.GetComponent<FiltersPanel>().SetProperties(propertyManager.GetFilteredProperties());
    }
    
    public void HideAllPanels()
    {
        _selectedMarker = null;
        detailsPanel.SetActive(false);
        filterPanel.SetActive(false);
    }
}
