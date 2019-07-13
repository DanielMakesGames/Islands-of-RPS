using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public int GridX;
    public int GridY;

    public bool IsWalkable;
    public Vector3 Position;

    public Node Parent;

    public int GCost;
    public int HCost;
    public int FCost
    {
        get { return GCost + HCost; }
    }

    public Node(bool isWalkable, Vector3 position, int gridX, int gridY)
    {
        this.IsWalkable = isWalkable;
        this.Position = position;
        this.GridX = gridX;
        this.GridY = gridY;
    }

    /*public Node northNode;
    public Node eastNode;
    public Node southNode;
    public Node westNode;

    bool isEmpty = false;
    public bool IsEmpty
    {
        get { return isEmpty; }
    }

    List<Node> neighbours = new List<Node>();

    const float rayDistance = 1f;
    LayerMask layerMask;

    private void Awake()
    {
        layerMask = LayerMask.GetMask("Tile");
        SetupNeighbour();
    }

    public void SetupNeighbour()
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
    }*/
}
