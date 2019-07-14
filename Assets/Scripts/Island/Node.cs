using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public int visited = -1;

    Node northNode;
    public Node NorthNode
    {
        get { return northNode; }
    }

    Node eastNode;
    public Node EastNode
    {
        get { return eastNode; }
    }

    Node southNode;
    public Node SouthNode
    {
        get { return southNode; }
    }

    Node westNode;
    public Node WestNode
    {
        get { return westNode;  }
    }

    public bool IsWalkable = true;

    List<Node> neighbours = new List<Node>();

    const float rayDistance = 1f;
    LayerMask layerMask;

    void Awake()
    {
        layerMask = LayerMask.GetMask("Tile");
        SetupNeighbour();
    }

    void SetupNeighbour()
    {
        RaycastHit raycastHit;
        if (Physics.Raycast(transform.position, transform.forward, out raycastHit,
            rayDistance, layerMask, QueryTriggerInteraction.Ignore))
        {
            Node hitNode = raycastHit.transform.GetComponent<Node>();
            if (hitNode != null)
            {
                northNode = hitNode;
                neighbours.Add(northNode);
            }
        }

        if (Physics.Raycast(transform.position, transform.right, out raycastHit,
            rayDistance, layerMask, QueryTriggerInteraction.Ignore))
        {
            Node hitNode = raycastHit.transform.GetComponent<Node>();
            if (hitNode != null)
            {
                eastNode = hitNode;
                neighbours.Add(eastNode);
            }
        }

        if (Physics.Raycast(transform.position, -transform.forward, out raycastHit,
            rayDistance, layerMask, QueryTriggerInteraction.Ignore))
        {
            Node hitNode = raycastHit.transform.GetComponent<Node>();
            if (hitNode != null)
            {
                southNode = hitNode;
                neighbours.Add(southNode);
            }
        }
        if (Physics.Raycast(transform.position, -transform.right, out raycastHit,
            rayDistance, layerMask, QueryTriggerInteraction.Ignore))
        {
            Node hitNode = raycastHit.transform.GetComponent<Node>();
            if (hitNode != null)
            {
                westNode = hitNode;
                neighbours.Add(westNode);
            }
        }
    }
}
