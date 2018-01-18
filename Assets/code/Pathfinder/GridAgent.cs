using System.Collections.Generic;
using UnityEngine;

public class GridAgent : MonoBehaviour {
    private Grid mygrid;
    public Vector2 target;
    private List<Node> path;
    public float advanceDistance = .4f;

    public bool seeking = false;

    public List<Node> Path
    {
        get { return path; }
    }

    public Vector2? globalCurrentTargetCoords
    {
        get
        {
            if( path != null && path.Count != 0 && mygrid.Ready)
            {
                return path[0].GlobalPos;
            }
            else
            {
                return target;
            }
        }
    }

    // Use this for initialization
	void Start ()
	{
        //Identify current grid.
	    var maybeGrids = GameObject.FindObjectsOfType<Grid>();
	    foreach (var grid in maybeGrids)
	    {
	        var localPos = grid.transform.InverseTransformPoint(this.transform.position);
	        if (localPos.x >= 0 && localPos.x <= grid.width && localPos.y >= 0 && localPos.y <= grid.height)
	        {
	            mygrid = grid;
	            break;
	        }
	    }

	}
	
	// Update is called once per frame
    void Update()
    {
        var here = new Vector2(this.transform.position.x, this.transform.position.y);
        var there = new Vector2(target.x, target.y);
        bool changed = advanceDistance > (here - globalCurrentTargetCoords.GetValueOrDefault(here)).magnitude;

        if (target != null && seeking && changed)
        {
            path = mygrid.findPath(this, here, there);

        }
    }
}
