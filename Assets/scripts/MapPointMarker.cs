using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPointMarker : MapPoint {

    OnlineMapsMarker3D marker3D;

    public MapPointMarker(OnlineMapsMarker3D marker) : base(marker.position.x,marker.position.y)
    {
        this.marker3D = marker;

       }

    protected override void graphicHide()
    {
        marker3D.enabled = false;
    }

    protected override void graphicShow()
    {
        marker3D.enabled = true;
    }

    public OnlineMapsMarker3D getMarker3D()
    {
        return this.marker3D;
    }

    private  void OnPointClick()
    {
        Debug.Log("HOLA");
    }
}
