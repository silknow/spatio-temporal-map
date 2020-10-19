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
    private CinemachineFollowZoom _cinemachineFollowZoom;
    public StackedMapScrollView stackedMapScrollView;

    private void Awake()
    {
        _cinemachineVirtual = GetComponent<CinemachineVirtualCamera>();
        _cinemachineFollowZoom = GetComponent<CinemachineFollowZoom>();
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
        var offsetSize = 0f;
        var even = true;
        mapInstances.Clear();
        foreach (var map in mapTileset)
        {
            var cubeTransform = map.GetComponent<StackedMapInstance>().GetCube();
            offsetSize = cubeTransform.localScale.x / 10f;
            
            map.localPosition+= new Vector3(0,offsetY,0);
            offsetY -= offsetSize;
            map.localRotation = Quaternion.identity * Quaternion.Euler(10f,180f,0);
            even = !even;
            mapInstances.Add(cubeTransform);
            _cinemachineFollowZoom.m_Width = cubeTransform.localScale.x;
           _cinemachineVirtual.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = new Vector3(0,0,cubeTransform.localScale.z);
        }
        stackedMapScrollView.ReGenerateUI(mapTileset.Select(s=>s.gameObject.name).ToList());
        OnIndexChange(0);
    }
}
