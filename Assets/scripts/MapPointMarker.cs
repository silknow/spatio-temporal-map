using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPointMarker : MapPoint {

    OnlineMapsMarker3D marker3D;
    OnlineMapsMarker marker2D;

    protected Dictionary<string, List<OnlineMapsDrawingLine>> graphicRelationsPerProperty = new Dictionary<string, List<OnlineMapsDrawingLine>>();

    public MapPointMarker(OnlineMapsMarker3D marker) : base(marker.position.x,marker.position.y)
    {
        this.marker3D = marker;
        this.marker2D = OnlineMapsMarkerManager.CreateItem(new Vector2(marker.position.x, marker.position.y), "");
        this.initData();

    }


    protected void OnMarkerClick(OnlineMapsMarkerBase marker)
    {
        //Debug.Log("TOC TOC on "+marker.label);
        MapItemPopup.instance.OnMarkerClick(this);       
        //this.showRelations("technique");
    }

    protected void createCollider()
    {
        BoxCollider box = marker3D.instance.GetComponent<BoxCollider>();
        box.center = new Vector3(0.0f, 0.75f, 0.0f);
        box.size = new Vector3(0.75f, 0.75f, 0.75f);
    }

   public MapPointMarker(float longitud, float latitud, GameObject prefabObject, bool cluster): base(longitud, latitud)
    {
        //Debug.Log("Longi,Lat --> " + longitud+","+latitud);
        marker3D = OnlineMapsMarker3DManager.CreateItem(new Vector2(longitud, latitud),prefabObject);
        marker2D = OnlineMapsMarkerManager.CreateItem(new Vector2(longitud, latitud),"");

        this.cluster = cluster;

        this.initData();

        /*
        marker2D.OnClick += OnMarkerClick;
        marker3D.OnClick += OnMarkerClick;

        if (dimension == MapPoint.THREE_DIMENSION)
            marker2D.enabled = false;

        if (dimension == MapPoint.TWO_DIMENSION)
            marker3D.enabled = false;*/

    }

    private void initData()
    {
        marker2D.OnClick += OnMarkerClick;
        marker3D.OnClick += OnMarkerClick;

        marker2D.OnRollOver += OnRollOver;
        marker2D.OnRollOut += OnRollOut;

        marker3D.OnRollOver += OnRollOver;
        marker3D.OnRollOut += OnRollOut;


        if (!cluster)
            createCollider();
        else
        {
            marker2D.texture = Resources.Load("clusterMarker64px") as Texture2D;
            marker2D.scale *= .75f;
        }

        if (dimension == MapPoint.THREE_DIMENSION)
            marker2D.enabled = false;

        if (dimension == MapPoint.TWO_DIMENSION)
            marker3D.enabled = false;
    }

    
    protected override void graphicHide()
    {
        if (marker3D!=null)
            marker3D.enabled = false;
        //marker3D.prefab.SetActive(false);
        if (marker2D!=null)
            marker2D.enabled = false;
    }

    protected override void graphicShow()
    {
        if (marker3D != null && dimension == THREE_DIMENSION)
            marker3D.enabled = true;
        //marker3D.prefab.SetActive(true);
        if (marker2D != null && dimension == TWO_DIMENSION)
            marker2D.enabled = true;
    }

    protected override void updateGraphicsCoordinates()
    {
        if (marker3D != null)
            marker3D.SetPosition(getX(), getY());

        if (marker2D != null)
            marker2D.SetPosition(getX(), getY());

    }

    public OnlineMapsMarker3D getMarker3D()
    {
        return this.marker3D;
    }

    public OnlineMapsMarker getMarker2D()
    {
        return this.marker2D;
    }

    private  void OnPointClick()
    {
        Debug.Log("HOLA");
    }

    protected override void updateGraphicLabel()
    {
        if (marker3D != null)
        {
            marker3D.prefab.name = this.label;
            marker3D.label = this.label;
        }

        if (marker2D != null)
            marker2D.label = this.label;
    }

    private void OnRollOut(OnlineMapsMarkerBase marker)
    {
        // Remove a reference to marker
        //Debug.Log("Hover out " + this.getLabel());
    }

    private void OnRollOver(OnlineMapsMarkerBase marker)
    {
        // Make a reference to marker
        //Debug.Log("Hover on " + this.getLabel());

        //if (isCluster())
    }

    protected override  void updateGraphicRelations(string propertyName,bool show)
    {
        /*
        List<Vector2> points = new List<Vector2>();

        points.Add(from.getVector2());
        points.Add(to.getVector2());



        OnlineMapsDrawingLine oLine = new OnlineMapsDrawingLine(points, Color.blue);
        oLine.width = 1.0f;
        oLine.visible = true;
        OnlineMapsDrawingElementManager.AddItem(oLine);
        oLine.visible = false;
        oLine.name = "connection";*/
        RelationShip relation;

        if (relationsPerProperty.ContainsKey(propertyName))
        {
            relation = relationsPerProperty[propertyName];

            if (!graphicRelationsPerProperty.ContainsKey(propertyName))
            {
                List<OnlineMapsDrawingLine> lineList = new List<OnlineMapsDrawingLine>();

                foreach (MapPoint p in relation.getRelatedWith())
                {
                    List<Vector2> points = new List<Vector2>();

                    points.Add(this.getVector2());
                    points.Add(p.getVector2());

                    OnlineMapsDrawingLine oLine = new OnlineMapsDrawingLine(points, Color.blue);
                    oLine.width = 1.0f;
                    oLine.visible = true;
                    OnlineMapsDrawingElementManager.AddItem(oLine);
                    oLine.visible = !p.isFiltered();
                    lineList.Add(oLine);
                }

                graphicRelationsPerProperty.Add(propertyName, lineList);
            }
            else
            {
                int pos = 0;
                foreach (OnlineMapsDrawingLine line in graphicRelationsPerProperty[propertyName])
                {
                    if (show)
                        line.visible = relation.isVisibleRelationAt(pos);
                    else
                        line.visible = false;
                    pos++;
                }
            }           
        }
      
    }


    public override void removeGraphicRelations(string propertyName)
    {
        if (graphicRelationsPerProperty.ContainsKey(propertyName))
        {
            //Debug.Log("Esconde relaciones 2");
            List<OnlineMapsDrawingLine> lineList = graphicRelationsPerProperty[propertyName];
            //Debug.Log("Esconde relaciones 3 hay " + lineList.Count);

            foreach (OnlineMapsDrawingLine line in lineList)
            {
                //line.visible = false;
                OnlineMapsDrawingElementManager.RemoveItem(line, true);
                line.Dispose();
            }

            graphicRelationsPerProperty.Remove(propertyName);
        }
    }

    public override void graphicReset()
    {
        if (marker2D!=null)
            marker2D.Dispose();

        if (marker3D!=null)
            marker3D.Dispose();

        marker2D = null;
        marker3D = null;
    }



}
