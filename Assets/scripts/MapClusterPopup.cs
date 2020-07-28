using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapClusterPopup : MonoBehaviour
{
    public Transform content;

    public GameObject clusterItemPrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RemoveChildren()
    {
        foreach (var child in content.GetComponentsInChildren<Transform>())
        {
            if(child!= content) 
                Destroy(child.gameObject);
        }
    }

    public void AddClusterItem(string name, string id)
    {
        var item = Instantiate(clusterItemPrefab, content);
        item.GetComponentInChildren<Text>().text = name;
        item.GetComponentInChildren<ClusterPopupItem>().SetMarker(id);
    }
}
