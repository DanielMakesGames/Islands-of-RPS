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
    [SerializeField] NavMeshLink[] northNavMeshLinks = null;

    Node eastNode;
    public Node EastNode
    {
        get { return eastNode; }
    }
    [SerializeField] NavMeshLink[] eastNavMeshLinks = null;

    Node southNode;
    public Node SouthNode
    {
        get { return southNode; }
    }
    [SerializeField] NavMeshLink[] southNavMeshLinks = null;

    Node westNode;
    public Node WestNode
    {
        get { return westNode;  }
    }
    [SerializeField] NavMeshLink[] westNavMeshLinks = null;

    public bool IsWalkable = true;
    public Squad currentSquad;

    public List<Node> Neighbours = new List<Node>();
    const float rayDistance = 10f;
    LayerMask nodeLayerMask;
    Vector3 nodeSurfaceOffset;
    const float meshLinkSize = 2f;

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
            Neighbours.Add(northNode);

            SetUpNavMeshLinks(northNavMeshLinks, hitNode);
        }
        else
        {
            DisableNavMeshLinks(northNavMeshLinks);
        }

        if (Physics.Raycast(transform.position, transform.right, out raycastHit,
            rayDistance, nodeLayerMask, QueryTriggerInteraction.Ignore))
        {
            Node hitNode = raycastHit.transform.GetComponent<Node>();
            eastNode = hitNode;
            Neighbours.Add(eastNode);

            SetUpNavMeshLinks(eastNavMeshLinks, hitNode);
        }
        else
        {
            DisableNavMeshLinks(eastNavMeshLinks);
        }

        if (Physics.Raycast(transform.position, -transform.forward, out raycastHit,
            rayDistance, nodeLayerMask, QueryTriggerInteraction.Ignore))
        {
            Node hitNode = raycastHit.transform.GetComponent<Node>();
            southNode = hitNode;
            Neighbours.Add(southNode);

            SetUpNavMeshLinks(southNavMeshLinks, hitNode);
        }
        else
        {
            DisableNavMeshLinks(southNavMeshLinks);
        }

        if (Physics.Raycast(transform.position, -transform.right, out raycastHit,
            rayDistance, nodeLayerMask, QueryTriggerInteraction.Ignore))
        {
            Node hitNode = raycastHit.transform.GetComponent<Node>();
            westNode = hitNode;
            Neighbours.Add(westNode);

            SetUpNavMeshLinks(westNavMeshLinks, hitNode);
        }
        else
        {
            DisableNavMeshLinks(westNavMeshLinks);
        }
    }

    void SetUpNavMeshLinks(NavMeshLink[] navMeshLinks, Node linkNode)
    {
        float heightDifference = linkNode.transform.position.y - transform.position.y;
        if (Mathf.Abs(heightDifference) < Mathf.Epsilon)
        {
            DisableNavMeshLinks(navMeshLinks);
        }
        else
        {
            for (int i = 0; i < navMeshLinks.Length; ++i)
            {
                Vector3 endPoint = new Vector3(navMeshLinks[i].endPoint.x, heightDifference, meshLinkSize);
                navMeshLinks[i].endPoint = endPoint;
            }
        }
    }

    void DisableNavMeshLinks(NavMeshLink[] navMeshLinks)
    {
        for (int i = 0; i < navMeshLinks.Length; ++i)
        {
            navMeshLinks[i].gameObject.SetActive(false);
        }
    }
}
