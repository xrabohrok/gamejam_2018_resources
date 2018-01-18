using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public float localX;
    public float localY;

    public int cost;

    public int xindex;
    public int yindex;

    private Vector2 globalPos;

    public Dictionary<GridAgent, SearchCalc> personalCalc;

    public Vector2 GlobalPos
    {
        get { return globalPos; }
    }

    private bool isActive;
    public Node[] neighbors;


    public bool IsActive
    {
        get { return isActive; }
    }

    public void determineAccesibility(Grid grid, int layerMaks)
    {
        var globalHere = grid.gameObject.transform.TransformPoint(localX, localY, 0);
        globalPos = new Vector2(globalHere.x, globalHere.y);

        Collider2D disqualifier = Physics2D.OverlapPoint(GlobalPos, layerMaks);

        isActive = disqualifier == null;

    }
}

public class SearchCalc {
    public float cost = 1;
    public float accumulatedCost;
    public float h = 0;
    public Node prev;
}