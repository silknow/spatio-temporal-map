using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{

    public GameObject button2Times;
    public GameObject button3Times;
    public GameObject button4Times;
    public GameObject buttonReloj;
    public GameObject buttonBackTime;

    public GameObject lupaMas;
    public GameObject lupaMenos;
    public GameObject camara;

    public void displayTimeButtons()
    {
        if (button2Times != null && button3Times != null && button4Times != null)
            if (button2Times.activeInHierarchy && button3Times.activeInHierarchy && button4Times.activeInHierarchy)
            {
                button2Times.SetActive(false);
                button3Times.SetActive(false);
                button4Times.SetActive(false);
            }
            else
            {
                button2Times.SetActive(true);
                button3Times.SetActive(true);
                button4Times.SetActive(true);
            }
    }

    public void onBackTime()
    {
        if (buttonReloj != null)
            buttonReloj.SetActive(true);

        buttonBackTime.SetActive(false);

        SilkMap.Instance.reactivate();
    }

    public void onButton2Times()
    {
        if (buttonBackTime!=null)
            buttonBackTime.SetActive(true);

        if (button2Times != null && button3Times != null && button4Times != null)
            if (button2Times.activeInHierarchy && button3Times.activeInHierarchy && button4Times.activeInHierarchy)
            {
                button2Times.SetActive(false);
                button3Times.SetActive(false);
                button4Times.SetActive(false);
            }

        //SilkMap.Instance.createPlane(2);
    }

    public void onButton3Times()
    {
        if (buttonBackTime != null)
            buttonBackTime.SetActive(true);

        if (button2Times != null && button3Times != null && button4Times != null)
            if (button2Times.activeInHierarchy && button3Times.activeInHierarchy && button4Times.activeInHierarchy)
            {
                button2Times.SetActive(false);
                button3Times.SetActive(false);
                button4Times.SetActive(false);
            }

        //SilkMap.Instance.createPlane(3);
    }

    public void onButton4Times()
    {
        if (buttonBackTime != null)
            buttonBackTime.SetActive(true);

        if (button2Times != null && button3Times != null && button4Times != null)
            if (button2Times.activeInHierarchy && button3Times.activeInHierarchy && button4Times.activeInHierarchy)
            {
                button2Times.SetActive(false);
                button3Times.SetActive(false);
                button4Times.SetActive(false);
            }

        //SilkMap.Instance.createPlane(3);
    }

    public void onButtonLupaMas()
    {

        if (SilkMap.Instance.timeSliceDisplay)
            SilkMap.Instance.mapCamera.transform.Translate(new Vector3(0.0f, 0.0f, 1.0f));
        else
            OnlineMaps.instance.zoom = OnlineMaps.instance.zoom + 1;
    }

    public void onButtonLupaMenos()
    {
        if (SilkMap.Instance.timeSliceDisplay)
            SilkMap.Instance.mapCamera.transform.Translate(new Vector3(0.0f, 0.0f, -1.0f));
        else
            OnlineMaps.instance.zoom = OnlineMaps.instance.zoom - 1;
    }

    public void onButtonCamara()
    {
        if (SilkMap.instance.map.GetDimension() == 3)
        {
            SilkMap.instance.map.SetDimension(2);
            Debug.Log("Cambiando a dimension 2");
        }

        else
        {
            Debug.Log("Cambiando a dimension 3");
            SilkMap.instance.map.SetDimension(3);
        }

        Dictionary<Vector2, List<MapPoint>> groups = SilkMap.instance.map.getGroups();

        Debug.Log("Hay " + groups.Keys.Count + " grupos ");

        /*
        List<string> filteredPropNameList = SilkMap.instance.map.GetPropertyManager().GetFilteredPropertiesName();

        Debug.Log("Hay " + filteredPropNameList.Count + " propiedades para filtrar");

        foreach (string s in filteredPropNameList)
        {
            Debug.Log("La propiedad " + s + " tiene los siguientes posibles valores:");
            List<string> propValues = SilkMap.instance.map.GetPropertyManager().GetPropertyByName(s).getPossibleValues();
            foreach (string s1 in propValues)
                Debug.Log("Valor propiedad : " + s1);
        }*/
    }

}
