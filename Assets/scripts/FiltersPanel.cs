using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FiltersPanel : MonoBehaviour
{
    public Transform body;

    public GameObject propertyRowPrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetProperties(List<Property> properties)
    {
        // Clear body children
        foreach (Transform child in body) {
            Destroy(child.gameObject);
        }

        foreach (Property prop in properties)
        {
            var propRow = Instantiate(propertyRowPrefab, body);
            propRow.GetComponent<FilteredPropertyRow>().property = prop;
        }
    }

    public void ClearFilters()
    {
        foreach (var toggle in body.GetComponentsInChildren<Toggle>())
        {
            toggle.isOn = false;
        }
    }
}
