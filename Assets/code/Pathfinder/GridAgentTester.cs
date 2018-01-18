using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(GridAgent))]
public class GridAgentTester : MonoBehaviour
{

    public Transform target;
    private GridAgent agent;

    // Use this for initialization
    void Start()
    {
        agent = GetComponent<GridAgent>();
        agent.target = target.transform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnDrawGizmos()
    {
        if (target != null)
        {
            Gizmos.color = Color.white;
            if (agent != null && agent.Path != null)
            {
                foreach (var node in agent.Path)
                {
                    var vectora = new Vector3(node.GlobalPos.x, node.GlobalPos.y);
                    var vector3 = new Vector3(node.GlobalPos.x, node.GlobalPos.y, 1.4f);
                    Gizmos.DrawSphere(vectora, .2f);
                    Handles.color = Color.red;
//                    Handles.Label(vector3, "cost:" + node.cost + " , accumulated Cost:" + node.personalCalc[agent].accumulatedCost );
                }
            }
        }
    }
}
