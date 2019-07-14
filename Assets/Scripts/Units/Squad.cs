using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Squad : MonoBehaviour
{
    public delegate void SquadSelectedAction(Squad squad);
    public static event SquadSelectedAction OnSquadSelected;
    public static event SquadSelectedAction OnSquadDeselected;

    public enum SquadState
    {
        Ready,
        OnTapped,
        Selected,
        OnTappedSelected,
        Moving
    }
    SquadState mySquadState = SquadState.Ready;

    int myFingerId = InputManager.InactiveTouch;
    const float rayDistance = 1f;
    const float rayOffset = 0.5f;
    LayerMask nodeLayerMask;

    Node currentNode;
    public Node CurrentNode
    {
        get { return currentNode; }
    }

    Node targetNode;
    public Node TargetNode
    {
        get { return targetNode; }
    }

    public Node PathfindingNode;

    IslandGrid islandGrid;
    public List<Node> path = new List<Node>();

    private void Awake()
    {
        islandGrid = FindObjectOfType<IslandGrid>();
        nodeLayerMask = LayerMask.GetMask("Node");
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

    private void Start()
    {
        SetCurrentNode();
    }

    void OnTouchBegin(int fingerId, Vector3 tapPosition, RaycastHit hitInfo)
    {
        if (myFingerId == InputManager.InactiveTouch)
        {
            myFingerId = fingerId;

            switch (mySquadState)
            {
                case SquadState.Ready:
                    if (hitInfo.transform != null)
                    {
                        if (hitInfo.transform == transform)
                        {
                            mySquadState = SquadState.OnTapped;
                        }
                    }
                    break;
                case SquadState.OnTapped:
                    break;
                case SquadState.Selected:
                    if (hitInfo.transform != null)
                    {
                        if (hitInfo.transform == transform)
                        {
                            mySquadState = SquadState.OnTappedSelected;
                        }
                    }
                    break;
                case SquadState.OnTappedSelected:
                    break;
                case SquadState.Moving:
                    break;
            }
        }
    }

    void OnTouchMove(int fingerId, Vector3 tapPosition, Vector3 touchDelta, RaycastHit hitInfo)
    {
        if (myFingerId == fingerId)
        {
            switch (mySquadState)
            {
                case SquadState.Ready:
                    break;
                case SquadState.OnTapped:
                    break;
                case SquadState.Selected:
                case SquadState.OnTappedSelected:
                    if (hitInfo.transform != null)
                    {
                        if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Node"))
                        {
                            PathfindingNode = hitInfo.transform.GetComponent<Node>();
                            path = islandGrid.GetPath(PathfindingNode);
                        }
                        if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Squad"))
                        {
                            PathfindingNode = hitInfo.transform.GetComponent<Squad>().CurrentNode;
                            path = islandGrid.GetPath(PathfindingNode);
                        }
                    }
                    else
                    {
                        path.Clear();
                    }
                    break;
                case SquadState.Moving:
                    break;
            }
        }
    }

    void OnTouchEnd(int fingerId, Vector3 tapPosition, RaycastHit hitInfo)
    {
        if (myFingerId == fingerId)
        {
            myFingerId = InputManager.InactiveTouch;

            switch (mySquadState)
            {
                case SquadState.Ready:
                    break;
                case SquadState.OnTapped:
                    if (hitInfo.transform != null)
                    {
                        if (hitInfo.transform == transform)
                        {
                            mySquadState = SquadState.Selected;
                            islandGrid.ResetNodeValues();
                            currentNode.visited = 0;
                            islandGrid.SetNodeDistances(currentNode);

                            OnSquadSelected?.Invoke(this);
                        }
                    }
                    break;
                case SquadState.Selected:
                    if (path.Count > 0)
                    {
                        mySquadState = SquadState.Moving;
                        targetNode = PathfindingNode;

                        StartCoroutine(MoveToTarget());
                        OnSquadDeselected?.Invoke(this);
                    }
                    else
                    {
                        path.Clear();
                        targetNode = null;

                        OnSquadDeselected?.Invoke(this);
                    }
                    break;
                case SquadState.OnTappedSelected:
                    if (hitInfo.transform != null)
                    {
                        if (hitInfo.transform == transform)
                        {
                            mySquadState = SquadState.Ready;
                            path.Clear();
                            targetNode = null;

                            OnSquadDeselected?.Invoke(this);
                        }
                    }
                    break;
                case SquadState.Moving:
                    break;
            }
        }
    }

    void SetCurrentNode()
    {
        if (Physics.Raycast(transform.position + transform.up * rayOffset, -transform.up, out RaycastHit raycastHit,
            rayDistance, nodeLayerMask, QueryTriggerInteraction.Ignore))
        {
            Node hitNode = raycastHit.transform.GetComponent<Node>();
            currentNode = hitNode;
        }
    }

    IEnumerator MoveToTarget()
    {
        float timer = 0f;
        Vector3 destination = targetNode.transform.position + Vector3.up * 0.5f;

        while (timer < 1f)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, destination, timer);
            yield return null;
        }

        mySquadState = SquadState.Ready;
        path.Clear();
        targetNode = null;
    }
}
