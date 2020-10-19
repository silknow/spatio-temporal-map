/*         INFINITY CODE         */
/*   https://infinity-code.com   */

/* Special thanks to Brian Chasalow for his help in developing this script. */

using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OnlineMapsRawImageTouchForwarder : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    public RawImage image;
    public OnlineMaps map;
    public RenderTexture targetTexture;

    private OnlineMapsTileSetControl control;

#if !UNITY_EDITOR
    private Vector2 pointerPos = Vector2.zero;
#endif

    protected Camera worldCamera
    {
        get
        {
            if (image.canvas == null || image.canvas.renderMode == RenderMode.ScreenSpaceOverlay) return null;
            return image.canvas.worldCamera;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
#if !UNITY_EDITOR
        pointerPos = eventData.position;
#endif
    }

    private void OnDrawTooltip(GUIStyle style, string text, Vector2 position)
    {
        RectTransform t = image.rectTransform;

        if (targetTexture == null)
        {
            position.x /= Screen.width / t.sizeDelta.x;
            position.y /= Screen.height / t.sizeDelta.y;
        }
        else
        {
            position.x /= targetTexture.width / t.sizeDelta.x;
            position.y /= targetTexture.height / t.sizeDelta.y;
        }

        position -= t.sizeDelta / 2;

        Vector3 pos = (Vector3)position + image.transform.position;

        position = RectTransformUtility.WorldToScreenPoint(worldCamera, pos);

        GUIContent tip = new GUIContent(text);
        Vector2 size = style.CalcSize(tip);
        GUI.Label(new Rect(position.x - size.x / 2 - 5, Screen.height - position.y - size.y - 20, size.x + 10, size.y + 5), text, style);
    }

    private Vector2 OnGetInputPosition()
    {
#if UNITY_EDITOR
        return ProcessTouch(Input.mousePosition);
#else
        return ProcessTouch(pointerPos);
#endif
    }

    private Vector2[] OnGetMultitouchInputPositions()
    {
        Vector2[] touches = Input.touches.Select(t => t.position).ToArray();
        for (int i = 0; i < touches.Length; i++)
        {
            touches[i] = ProcessTouch(touches[i]);
        }
        return touches;
    }

    private int OnGetTouchCount()
    {
#if UNITY_EDITOR
        return Input.GetMouseButton(0) ? 1 : 0;
#else
        return Input.touchCount;
#endif
    }

    public void OnPointerDown(PointerEventData eventData)
    {
#if !UNITY_EDITOR
        pointerPos = eventData.position;
#endif
    }

    private Vector2 ProcessTouch(Vector2 inputTouch)
    {
        Vector2 pos = inputTouch;

        RectTransform t = image.rectTransform;
        Vector2 sizeDelta = t.rect.size;
        if ((int)sizeDelta.x == 0 || (int)sizeDelta.y == 0) return Vector2.zero;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(image.rectTransform, pos, worldCamera, out pos)) return Vector2.zero;

        pos += sizeDelta / 2.0f;

        if (targetTexture == null)
        {
            pos.x *= Screen.width / sizeDelta.x;
            pos.y *= Screen.height / sizeDelta.y;
        }
        else
        {
            pos.x *= targetTexture.width / sizeDelta.x;
            pos.y *= targetTexture.height / sizeDelta.y;
        }

        return pos;
    }

    private void Start()
    {
        if (map == null) map = OnlineMaps.instance;
        control = map.control as OnlineMapsTileSetControl;

        control.OnGetInputPosition += OnGetInputPosition;
        control.OnGetMultitouchInputPositions += OnGetMultitouchInputPositions;
        control.OnGetTouchCount += OnGetTouchCount;

        map.notInteractUnderGUI = false;
        control.checkScreenSizeForWheelZoom = false;

        OnlineMapsGUITooltipDrawer.OnDrawTooltip += OnDrawTooltip;
    }
}
