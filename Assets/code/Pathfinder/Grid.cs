using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Grid : MonoBehaviour
{

    public float density = 1.0f;
    public float width = 20;
    public float height = 20;

    public float Distance { get { return width / nodeCountHori; } }

    private int nodeCountVert = 0;
    private int nodeCountHori = 0;

    public List<String> colliderLayers;
    private int layerMask;

    private Node[,] _nodeArray;
    private bool showGuesses;
    public bool showGrid { get; set; }

    public bool ShowGuesses
    {
        get { return showGuesses; }
        set { showGuesses = value; }
    }

    public bool Ready
    {
        get { return ready; }
        set { ready = value; }
    }

    private bool ready = true;

    public List<Node> findPath(GridAgent agent, Vector2 startPoint, Vector2 endPoint)
    {
        if (!ready)
        {
            return null;
        }

        var startPointSet = getNodeGuesses(startPoint);
        Node startNode = null;
        if (startPointSet != null)
        {
            startNode = startPointSet[0];
        }

        var endPointSet = getNodeGuesses(endPoint);
        Node endNode = null;
        if (endPointSet != null)
        {
            endNode = endPointSet[0];
        }

        if (endNode == null || startNode == null)
            return null;

        List<Node> unprocessed = new List<Node>();
        foreach (var node in _nodeArray)
        {
            if (node.IsActive)
            {
                unprocessed.Add(node);
                if (!node.personalCalc.ContainsKey(agent))
                {
                    node.personalCalc[agent] = new SearchCalc();
                }
                node.personalCalc[agent].cost = node.cost;
                node.personalCalc[agent].h = Math.Abs(node.xindex - endNode.xindex) + Math.Abs(node.yindex - endNode.yindex);
                node.personalCalc[agent].accumulatedCost = Mathf.Infinity;
                node.personalCalc[agent].prev = null;
            }
            
        }

        List<Node> neighbors = new List<Node>();
        Node currentNode = startNode;
        startNode.personalCalc[agent].accumulatedCost = 0;
        while (currentNode != endNode && unprocessed.Any())
        {
            foreach (var currentNeighbor in currentNode.neighbors)
            {
                if (currentNeighbor.IsActive )
                {
                    var thisAccumulatedCost = currentNode.personalCalc[agent].accumulatedCost + currentNeighbor.cost +
                                              currentNeighbor.personalCalc[agent].h;
                    if(currentNeighbor.personalCalc[agent].accumulatedCost > thisAccumulatedCost)
                    {
                        currentNeighbor.personalCalc[agent].accumulatedCost =
                            thisAccumulatedCost;
                        currentNeighbor.personalCalc[agent].prev = currentNode;
                    }
                    


                    if (!neighbors.Contains(currentNeighbor))
                    {
                        int insertIndex = 0;

                        for (int i = 0; i < neighbors.Count; i++)
                        {
                            var currentNodeCost = currentNode.personalCalc[agent].accumulatedCost;
                            var otherCost = neighbors[i].personalCalc[agent].accumulatedCost;
                            if (currentNodeCost >= otherCost)
                            {
                                break;
                            }
                        }
                        if (!neighbors.Any() || insertIndex == -1)
                        {
                            neighbors.Add(currentNeighbor);
                        }
                        else
                        {
                            neighbors.Insert(insertIndex, currentNeighbor);
                        }
                    }

                }
            }

            neighbors.Remove(currentNode);
            if(neighbors.Any())
            {
                currentNode = neighbors.Last();
            }
            else
            {
                break;
            }

        }

        //pathfind failed
        if (currentNode != endNode)
        {
            return null;
        }

        List<Node> path = new List<Node>();
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.personalCalc[agent].prev;
        }

        path.Reverse();

        return path;

    }

    // Use this for initialization

    void Start()
    {
        buildLayerMasks();

        if (_nodeArray == null)
        {
            buildGraph();
        }
    }

    private void buildLayerMasks()
    {
        layerMask = 0;
        foreach (var layer in colliderLayers)
        {
            layerMask |= 1 << LayerMask.NameToLayer(layer);
        }
    }

    private void hookupNeighbors(Node[,] nodeSet)
    {
        int count = 0;
        foreach (var node in nodeSet)
        {
            if (node == null)
                count++;
        }

        for (int j = 0; j < nodeCountVert; j++)
        {
            for (int i = 0; i < nodeCountHori; i++)
            {
                if (nodeSet[i, j].IsActive)
                {
                    List<Node> neighbors = new List<Node>();
                    //up 
                    if (j - 1 >= 0)
                    {
                        neighbors.Add(nodeSet[i, j - 1]);
                    }
                    //upright
                    if (i + 1 < nodeCountHori && j - 1 >= 0)
                    {
                        neighbors.Add(nodeSet[i + 1, j - 1]);
                    }
                    //right
                    if (i + 1 < nodeCountHori)
                    {
                        neighbors.Add(nodeSet[i + 1, j]);
                    }
                    //rightdown
                    if (i + 1 < nodeCountHori && j + 1 < nodeCountVert)
                    {
                        neighbors.Add(nodeSet[i + 1, j + 1]);
                    }
                    //down
                    if (j + 1 < nodeCountVert)
                    {
                        neighbors.Add(nodeSet[i, j + 1]);
                    }
                    //down left
                    if (i - 1 >= 0 && j + 1 < nodeCountVert)
                    {
                        neighbors.Add(nodeSet[i - 1, j + 1]);
                    }
                    //left
                    if (i - 1 >= 0)
                    {
                        neighbors.Add(nodeSet[i - 1, j]);
                    }
                    //upleft
                    if (i - 1 >= 0 && j - 1 >= 0)
                    {
                        neighbors.Add(nodeSet[i - 1, j - 1]);
                    }

                    List<Node> goodNeighbors = new List<Node>();
                    foreach (var neighbor in neighbors)
                    {
                        if (neighbor.IsActive)
                        {
                        
                            var contactFilter2D = new ContactFilter2D();
                            contactFilter2D.layerMask = layerMask;
                            contactFilter2D.useLayerMask = true;
                            contactFilter2D.useTriggers = false;
                            var raycastHit2Ds = new RaycastHit2D[1];
                            var raycastHits = Physics2D.Raycast(nodeSet[i, j].GlobalPos,
                                neighbor.GlobalPos - nodeSet[i, j].GlobalPos, contactFilter2D, raycastHit2Ds, neighbor.GlobalPos.magnitude - nodeSet[i, j].GlobalPos.magnitude);
                            if (0 == raycastHits)
                            {
                                goodNeighbors.Add(neighbor);
                            }
                        }
                    }
                    nodeSet[i, j].neighbors = goodNeighbors.ToArray();

                }
            }
        }
    }

    // Update is called once per frame

    void Update () {
		
	}

    private void InitializeNodeArray()
    {
        nodeCountHori = Mathf.FloorToInt(width / (1/density)) + 1;
        nodeCountVert = Mathf.FloorToInt(height / (1/density)) + 1;

        _nodeArray = new Node[nodeCountHori, nodeCountVert ];


        for (int j = 0; j < nodeCountVert; j++)
        {
            for (int i = 0; i < nodeCountHori; i++)
            {
                _nodeArray[i, j] = new Node
                {
                    localX = i * width/ (nodeCountHori - 1),
                    localY = j * height/ (nodeCountVert - 1),
                    xindex = i,
                    yindex = j,
                    cost = 1,
                    personalCalc = new Dictionary<GridAgent, SearchCalc>()
                };

                _nodeArray[i, j].determineAccesibility(this, layerMask);

            }
        }
    }

    public void buildGraph()
    {
        buildLayerMasks();
        InitializeNodeArray();
        hookupNeighbors(_nodeArray);
    }

    public void OnDrawGizmos()
    {
        var gridCenter = new Vector3(this.transform.position.x + width/2, this.transform.position.y + height/2, 0);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(gridCenter, new Vector3(width, height));

        if (showGrid)
        {
            if (_nodeArray != null)
            {
                foreach (var node in _nodeArray)
                {
                    if (node.IsActive)
                    {
                        Gizmos.color = Color.blue;
                    }
                    else
                    {
                        Gizmos.color = Color.red;
                    }
                    var nodeGlobalPos = new Vector3(node.GlobalPos.x, node.GlobalPos.y);
                    Gizmos.DrawCube(nodeGlobalPos, new Vector3(.15f, .15f));

                    if (node != null && node.neighbors != null)
                    {
                        Gizmos.color = Color.green;
                        foreach (var neighbor in node.neighbors)
                        {
                            var neighborPos = new Vector3(neighbor.GlobalPos.x, neighbor.GlobalPos.y);
                            Gizmos.DrawRay(neighborPos, nodeGlobalPos - neighborPos);
                        }
                    }

                    if (node.personalCalc.Keys.Any())
                    {
                        var vector3 = new Vector3(node.GlobalPos.x, node.GlobalPos.y, 2);
                        
                        Handles.color = Color.red;
//                        Handles.Label(vector3, "cost:" + node.cost + " , accumulated Cost:" + node.personalCalc[node.personalCalc.Keys.First()].accumulatedCost);
//                        Handles.Label(vector3, " " + node.personalCalc[node.personalCalc.Keys.First()].accumulatedCost + "," + node.personalCalc[node.personalCalc.Keys.First()].h);
                        Handles.Label(vector3, node.xindex + "," + node.yindex);
                    }
                }

            }
        }

        if (ShowGuesses && _nodeArray != null)
        {
            var guiPointToWorldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
            List<Node> potentialNodes =  getNodeGuesses(new Vector2(guiPointToWorldRay.x, guiPointToWorldRay.y));

            if(potentialNodes != null)
            {
                Gizmos.color = Color.magenta;
                foreach (var node in potentialNodes)
                {
                    Gizmos.DrawCube(node.GlobalPos, new Vector3(.3f, .3f));
                }
            }
        }
    }

    /// <summary>
    /// Find the potential node that could refer to the point provided
    /// </summary>
    /// <param name="globalRequestPoint"></param>
    /// <returns>a list sorted by distance from the point</returns>
    public List<Node> getNodeGuesses(Vector2 globalRequestPoint)
    {

        //so the real trick here is to not sort through ALL of the points, because there could be alot
        //I actually think I can narrow it down by figuring out the local coordintates, and mapping that out to a zone of four or nine points
        //that should be just fine

        var localRequestedPoint = this.transform.InverseTransformPoint(globalRequestPoint);

        //Is this even in the zone of concern
        if ((localRequestedPoint.x < 0 || localRequestedPoint.x > width ||
            localRequestedPoint.y > height || localRequestedPoint.y < 0))
        {
            return null;
        }

        //if we are in the ballpark, I should be able to guess the indexes of concern
        int targetX = Mathf.FloorToInt((nodeCountHori ) * localRequestedPoint.x / width);
        int targetY = Mathf.FloorToInt((nodeCountVert ) * localRequestedPoint.y / height);

        bool leftEdge = targetX == 0;
        bool rightEdge = targetX == nodeCountHori -1;
        bool upEdge = targetY == 0;
        bool botEdge = targetY == nodeCountVert -1;

        List<Node> zone = new List<Node>();
        zone.Add(_nodeArray[targetX, targetY]);
        if (!upEdge)
        {
            zone.Add(_nodeArray[targetX, targetY -1]);
        }
        if (!botEdge)
        {
            zone.Add(_nodeArray[targetX, targetY + 1]);
        }
        if (!leftEdge)
        {
            zone.Add(_nodeArray[targetX -1, targetY]);
        }
        if (!rightEdge)
        {
            zone.Add(_nodeArray[targetX +1, targetY]);
        }
        if (!rightEdge && !upEdge)
        {
            zone.Add(_nodeArray[targetX +1, targetY -1]);
        }
        if (!rightEdge && !botEdge)
        {
            zone.Add(_nodeArray[targetX + 1, targetY + 1]);
        }
        if (!leftEdge && !upEdge)
        {
            zone.Add(_nodeArray[targetX -1, targetY - 1]);
        }
        if (!leftEdge && !botEdge)
        {
            zone.Add(_nodeArray[targetX - 1, targetY + 1]);
        }

        List<Node> guesses = zone.Where(n => n.IsActive).ToList();

        zone.Sort((x,y)=> (x.GlobalPos - globalRequestPoint).magnitude < (y.GlobalPos - globalRequestPoint).magnitude ? -1 : 1 );

        return guesses;
    }
}