using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackedObjectClicker : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray,out hit))
            {
                if (hit.collider != null)
                {
                    Debug.Log("CLICK ON "+hit.collider.name);
                }
            }
        }
    }
}
