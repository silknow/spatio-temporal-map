using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ArcTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public GameObject tooltipElement;

    private RectTransform _rect;
    public void Start()
    {
        _rect = tooltipElement.GetComponent<RectTransform>();
        GetComponent<Image>().alphaHitTestMinimumThreshold = .1f;
    }
    

    public void OnPointerEnter(PointerEventData eventData)
    {
        print("entro");
        //_rect.localPosition = eventData.position += Vector2.up * 2.0f;

        tooltipElement.GetComponent<Text>().text = GetComponent<RelationshipArcProperty>().sizeOfRelation.ToString();
        
        tooltipElement.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        print("salgo");
        tooltipElement.SetActive(false);
    }
}
