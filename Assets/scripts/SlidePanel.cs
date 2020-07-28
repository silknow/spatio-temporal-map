using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(RectTransform))]
public class SlidePanel : MonoBehaviour
{
    public bool visible = true;
    private RectTransform _rectTransform;
    private bool animate = false;
    public enum Direction
    {
        Ltr = 1,
        Rtl = -1
    }
    public Direction SlideDirection = Direction.Rtl;
    public LeanTweenType easeType = LeanTweenType.easeInCubic;
    public float animationTime = 0.5f;
    
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
       
    }
    public void TogglePanelAnimation(Action OnComplete)
    {
        if(animate)
            return;
        animate = true;
        var finalPos = visible ? 0f : (int)SlideDirection *  _rectTransform.sizeDelta.x;
        LeanTween.moveX(_rectTransform, finalPos, animationTime).setEase(easeType).setOnComplete(OnComplete);

    }
    private void ToggleVisible()
    {
        visible = !visible;
        animate = false;
    }

    public void ShowAndEnablePanel()
    {
        if (gameObject.activeInHierarchy)
            return;
        visible = true;
        gameObject.SetActive(true);
        var initPos =  - _rectTransform.sizeDelta.x;
        LeanTween.moveX(_rectTransform, initPos, 0f);
        TogglePanelAnimation(ToggleVisible);
    }
    public void HideAndDisablePanel()
    {
        if (!gameObject.activeInHierarchy)
            return;
        visible = false;
        var initPos =  0f;
        LeanTween.moveX(_rectTransform, initPos, 0f);
        TogglePanelAnimation(ToggleVisibleDisable);
    }

    private void ToggleVisibleDisable()
    {
        visible = !visible;
        animate = false;
        gameObject.SetActive(false);
    }
    
}
