using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Squad : MonoBehaviour
{
    public delegate void SquadSelectedAction(Squad squad);
    public static event SquadSelectedAction OnSquadSelected;
    public static event SquadSelectedAction OnSquadDeselected;

    public delegate void SquadAnimationAction();
    public event SquadAnimationAction OnAnimateSquadPath;
    public event SquadAnimationAction OnAnimateSquadSelected;
    public event SquadAnimationAction OnAnimateSquadDeselected;
    public event SquadAnimationAction OnUpdateNavMeshAgents;

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
    const float movementSpeed = 2f;
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

    [SerializeField] GameObject SquadUnitGameObject = null;
    [SerializeField] Transform[] SquadPositions = null;

    List<SquadUnit> squadUnits;

    private void Awake()
    {
        islandGrid = FindObjectOfType<IslandGrid>();
        nodeLayerMask = LayerMask.GetMask("Node");
        squadUnits = new List<SquadUnit>();
    }

    void OnEnable()
    {
        InputManager.OnTouchBegin += OnTouchBegin;
        InputManager.OnTouchMove += OnTouchMove;
        InputManager.OnTouchEnd += OnTouchEnd;

        Squad.OnSquadSelected += Squad_OnSquadSelected;
        Squad.OnSquadDeselected += Squad_OnSquadDeselected;
    }

    void OnDisable()
    {
        InputManager.OnTouchBegin -= OnTouchBegin;
        InputManager.OnTouchMove -= OnTouchMove;
        InputManager.OnTouchEnd -= OnTouchEnd;

        Squad.OnSquadSelected -= Squad_OnSquadSelected;
        Squad.OnSquadDeselected -= Squad_OnSquadDeselected;
    }

    private void Start()
    {
        SetCurrentNode();

        for (int i = 0; i < SquadPositions.Length; ++i)
        {
            GameObject clone = Instantiate(SquadUnitGameObject);
            clone.transform.position = SquadPositions[i].position;
            clone.transform.rotation = SquadPositions[i].rotation;

            SquadUnit squadUnit = clone.GetComponent<SquadUnit>();
            squadUnit.InitializeSquadUnit(this, SquadPositions[i]);
            squadUnits.Add(squadUnit);
        }
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
                    OnAnimateSquadPath?.Invoke();
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
                            OnAnimateSquadSelected?.Invoke();
                        }
                    }
                    break;
                case SquadState.Selected:
                    if (path.Count > 0)
                    {
                        mySquadState = SquadState.Moving;
                        targetNode = PathfindingNode;

                        StartCoroutine(MoveToTargetCoroutine());
                        OnSquadDeselected?.Invoke(this);
                        OnAnimateSquadDeselected?.Invoke();
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
                            OnAnimateSquadDeselected.Invoke();
                        }
                    }
                    break;
                case SquadState.Moving:
                    break;
            }
            OnAnimateSquadPath?.Invoke();
        }
    }

    void Squad_OnSquadSelected(Squad squad)
    {
        if (squad != this)
        {
            switch (mySquadState)
            {
                case SquadState.Ready:
                    break;
                case SquadState.OnTapped:
                    mySquadState = SquadState.Ready;
                    break;
                case SquadState.Selected:
                case SquadState.OnTappedSelected:
                    mySquadState = SquadState.Ready;
                    OnAnimateSquadDeselected.Invoke();
                    OnSquadDeselected?.Invoke(this);
                    break;
                case SquadState.Moving:
                    break;
            }
        }
    }

    void Squad_OnSquadDeselected(Squad squad)
    {
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

    public void MoveToTarget(Node destinationNode)
    {
        islandGrid.ResetNodeValues();
        currentNode.visited = 0;
        islandGrid.SetNodeDistances(currentNode);
        targetNode = destinationNode;
        path = islandGrid.GetPath(destinationNode);
        StartCoroutine(MoveToTargetCoroutine());
    }

    IEnumerator MoveToTargetCoroutine()
    {
        Vector3 offSet = Vector3.up * 5f;

        for (int nodeIndex = 1; nodeIndex < path.Count; ++nodeIndex)
        {
            Vector3 startPosition = transform.position;
            Vector3 destination = path[nodeIndex].transform.position + offSet;
            float timer = 0f;

            while (timer < 1f)
            {
                timer += Time.deltaTime * movementSpeed;
                transform.position = Vector3.Lerp(startPosition, destination, timer);
                OnAnimateSquadPath?.Invoke();
                yield return null;
            }

            OnUpdateNavMeshAgents?.Invoke();
            path.RemoveAt(nodeIndex - 1);
            --nodeIndex;
        }

        mySquadState = SquadState.Ready;
        path.Clear();

        SetCurrentNode();
        targetNode = null;

        OnAnimateSquadPath?.Invoke();
    }
}
