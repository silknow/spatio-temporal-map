﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoardMarker : MonoBehaviour
{
    public Transform camTransform;

    Quaternion originalRotation;

    void Start()
    {
        originalRotation = transform.rotation;
    }
    
}