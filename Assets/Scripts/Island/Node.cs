using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Node : MonoBehaviour
{
    public int visited = -1;

    Node northNode;
    public Node NorthNode
    {
        get { return northNode; }
    }
    [SerializeField] NavMeshLink northNavMeshLink = null;

    Node eastNode;
    public Node EastNode
    {
        get { return eastNode; }
    }
    [SerializeField] NavMeshLink eastNavMeshLink = null;

    Node southNode;
    public Node SouthNode
    {
        get { return southNode; }
    }
    [SerializeField] NavMeshLink southNavMeshLink = null;

    Node westNode;
    public Node WestNode
    {
        get { return westNode;  }
    }
    [SerializeField] NavMeshLink westNavMeshLink = null;

    public bool IsWalkable = true;

    List<Node> neighbours = new List<Node>();
    const float rayDistance = 10f;
    LayerMask nodeLayerMask;
    Vector3 nodeSurfaceOffset;

    void Awake()
    {
        nodeLayerMask = LayerMask.GetMask("Node");
        SetupNeighbour();
        nodeSurfaceOffset = new Vector3(0f, 5f, 0f);
    }

    void SetupNeighbour()
    {
        RaycastHit raycastHit;
        if (Physics.Raycast(transform.position, transform.forward, out raycastHit,
            rayDistance, nodeLayerMask, QueryTriggerInteraction.Ignore))
        {
            Node hitNode = raycastHit.transform.GetComponent<Node>();
            northNode = hitNode;
            neighbours.Add(northNode);

            Vector3 endPoint = new Vector3(0f, hitNode.transform.position.y - transform.position.y, 1f);
            northNavMeshLink.endPoint = endPoint;
        }
        else
        {
            northNavMeshLink.enabled = false;
        }

        if (Physics.Raycast(transform.position, transform.right, out raycastHit,
            rayDistance, nodeLayerMask, QueryTriggerInteraction.Ignore))
        {
            Node hitNode = raycastHit.transform.GetComponent<Node>();
            eastNode = hitNode;
            neighbours.Add(eastNode);

            Vector3 endPoint = new Vector3(0f, hitNode.transform.position.y - transform.position.y, 1f);
            eastNavMeshLink.endPoint = endPoint;
        }
        else
        {
            eastNavMeshLink.enabled = false;
        }

        if (Physics.Raycast(transform.position, -transform.forward, out raycastHit,
            rayDistance, nodeLayerMask, QueryTriggerInteraction.Ignore))
        {
            Node hitNode = raycastHit.transform.GetComponent<Node>();
            southNode = hitNode;
            neighbours.Add(southNode);

            Vector3 endPoint = new Vector3(0f, hitNode.transform.position.y - transform.position.y, 1f);
            southNavMeshLink.endPoint = endPoint;
        }
        else
        {
            southNavMeshLink.enabled = false;
        }

        if (Physics.Raycast(transform.position, -transform.right, out raycastHit,
            rayDistance, nodeLayerMask, QueryTriggerInteraction.Ignore))
        {
            Node hitNode = raycastHit.transform.GetComponent<Node>();
            westNode = hitNode;
            neighbours.Add(westNode);

            Vector3 endPoint = new Vector3(0f, hitNode.transform.position.y - transform.position.y, 1f);
            westNavMeshLink.endPoint = endPoint;
        }
        else
        {
            westNavMeshLink.enabled = false;
        }
    }
}
