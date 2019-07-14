using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    IslandGrid islandGrid;
    public Node StartNode;
    public Node PathfindingNode;
    public Node TargetNode;

    int myFingerId = InputManager.InactiveTouch;
    public List<Node> path = new List<Node>();

    enum PathfindingState
    {
        Start,
        Pathfinding,
        End
    }
    PathfindingState myPathfindingState = PathfindingState.Start;

    private void Awake()
    {
        islandGrid = GetComponent<IslandGrid>();
    }

    void OnEnable()
    {
        InputManager.OnTouchBegin += OnTouchBegin;
        InputManager.OnTouchMove += OnTouchMove;
        InputManager.OnTouchEnd += OnTouchEnd;
    }

    void OnDisable()
    {
        InputManager.OnTouchBegin -= OnTouchBegin;
        InputManager.OnTouchMove -= OnTouchMove;
        InputManager.OnTouchEnd -= OnTouchEnd;
    }

    void Start()
    {
        islandGrid.ResetNodeValues();
    }

    void OnTouchBegin(int fingerId, Vector3 tapPosition, RaycastHit hitInfo)
    {
        if (hitInfo.transform == null)
        {
            return;
        }
        if (myFingerId == InputManager.InactiveTouch)
        {
            myFingerId = fingerId;

            if (myPathfindingState == PathfindingState.Start)
            {
                if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Tile"))
                {
                    StartNode = hitInfo.transform.GetComponent<Node>();
                }
            }
            else if (myPathfindingState == PathfindingState.Pathfinding)
            {
                if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Tile"))
                {
                    PathfindingNode = hitInfo.transform.GetComponent<Node>();
                }
            }
        }
    }

    void OnTouchMove(int fingerId, Vector3 tapPosition, Vector3 touchDelta, RaycastHit hitInfo)
    {
        if (myPathfindingState == PathfindingState.Pathfinding)
        {
            if (hitInfo.transform == null)
            {
                return;
            }
            if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Tile"))
            {
                PathfindingNode = hitInfo.transform.GetComponent<Node>();
                path = islandGrid.GetPath(PathfindingNode);
            }
        }
    }

    void OnTouchEnd(int fingerId, Vector3 tapPosition, RaycastHit hitInfo)
    {
        if (myFingerId == fingerId)
        {
            myFingerId = InputManager.InactiveTouch;

            if (myPathfindingState == PathfindingState.Start)
            {
                if (hitInfo.transform == null)
                {
                    StartNode = null;
                    return;
                }
                if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Tile"))
                {
                    Node hitNode = hitInfo.transform.GetComponent<Node>();
                    if (hitNode == StartNode)
                    {
                        islandGrid.ResetNodeValues();
                        StartNode = hitInfo.transform.GetComponent<Node>();
                        StartNode.visited = 0;
                        islandGrid.SetNodeDistances(StartNode);

                        myPathfindingState = PathfindingState.Pathfinding;
                    }
                }
            }
            else if (myPathfindingState == PathfindingState.Pathfinding)
            {
                if (hitInfo.transform == null)
                {
                    return;
                }
                if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Tile"))
                {
                    Node hitNode = hitInfo.transform.GetComponent<Node>();
                    if (hitNode == StartNode)
                    {
                        myPathfindingState = PathfindingState.Start;
                        path.Clear();
                    }
                    else
                    {
                        TargetNode = hitNode;
                        myPathfindingState = PathfindingState.Start;
                    }
                }
            }

        }
    }
}
