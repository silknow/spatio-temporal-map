using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;

public class CameraFollowMap : MonoBehaviour
{
    public List<Transform> mapInstances;

    private CinemachineVirtualCamera _cinemachineVirtual;
    public StackedMapScrollView stackedMapScrollView;

    private void Awake()
    {
        _cinemachineVirtual = GetComponent<CinemachineVirtualCamera>();
        if (mapInstances != null && mapInstances.Count > 0)
        {
            _cinemachineVirtual.Follow = mapInstances[0];
            _cinemachineVirtual.LookAt = mapInstances[0];
        }
    }

    public void OnIndexChange(int index)
    {
        //print("ON INDEX CHANGE "+index);
        if (index >= 0 && index < mapInstances.Count)
        {
            _cinemachineVirtual.Follow = mapInstances[index];
            _cinemachineVirtual.LookAt = mapInstances[index];
        }
    }

    public void PopulateListOfMaps(List<Transform> mapTileset)
    {
        var offsetY = 0f;
        var even = true;
        
        foreach (var map in mapTileset)
        {
            map.localPosition+= new Vector3(0,offsetY,0);
            offsetY -= 8f;
            map.localRotation = Quaternion.identity * Quaternion.Euler(even ? 10f :0f,180f,0);
            even = !even;
        }
        mapInstances.Clear();
        mapInstances = mapTileset;
        stackedMapScrollView.ReGenerateUI(mapTileset.Select(s=>s.gameObject.name).ToList());
    }
}
