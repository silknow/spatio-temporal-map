using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RelationshipArcProperty : MonoBehaviour
{
    public Color baseColor { get => GetComponentInChildren<Image>().color;
        set
        {
            var shadedColor = value;
            shadedColor.a = 0.5f;
            GetComponentsInChildren<Image>()[1].color = value;
            GetComponent<Image>().color = shadedColor;
        }
    }

    public float  totalSizeOfProperty { get => GetComponent<Image>().fillAmount; set => GetComponent<Image>().fillAmount = value; }
    public float  sizeOfRelation { get => GetComponentsInChildren<Image>()[1].fillAmount; set => GetComponentsInChildren<Image>()[1].fillAmount = value; }
}
