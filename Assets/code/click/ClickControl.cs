using UnityEngine;
using System.Collections.Generic;

public class ClickControl : MonoBehaviour
{

    public Vector3 LastGoodPoint { get; set; }

    bool valid = false;
    public bool validPoint { get { return valid; } private set{ valid = value;} }
    public GameObject thingClicked { get; set; }
    public Vector3 clickedPoint { get; set; }
    public List<string> limitToLayerNamed;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit infoOut;

        //limit to one layer
        var layermaskNum = 0;
        var layerMask = 0;
        foreach(var layername in limitToLayerNamed)
        {
            layermaskNum = LayerMask.NameToLayer(layername);
            layerMask |= 1 << layermaskNum;
        }
        

        if (Physics.Raycast(ray, out infoOut, 500, layerMask))
        {
            LastGoodPoint = infoOut.point;
            thingClicked = infoOut.collider.gameObject;
            clickedPoint = infoOut.point;
            valid = true;
        }
        else
        {
            valid = false;
        }
    }
}
