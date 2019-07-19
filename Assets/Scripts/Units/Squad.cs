using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Squad : MonoBehaviour
{
    public delegate void SquadAnimationAction();
    public event SquadAnimationAction OnAnimateSquadPath;
    public event SquadAnimationAction OnAnimateSquadSelected;
    public event SquadAnimationAction OnAnimateSquadDeselected;

    public delegate void SquadMovementAction(Vector3 targetPosition);
    public event SquadMovementAction OnUpdateNavMeshAgents;

    public enum SquadState
    {
        Ready,
        OnTapped,
        Selected,
        OnTappedSelected,
        Moving
    }
    protected SquadState mySquadState = SquadState.Ready;

    protected Node currentNode;
    public Node CurrentNode
    {
        get { return currentNode; }
    }

    protected Node targetNode;
    public Node TargetNode
    {
        get { return targetNode; }
    }

    const float rayDistance = 1f;
    const float rayOffset = 0.5f;
    protected LayerMask nodeLayerMask;

    public Node PathfindingNode;
    public Node PreviousNode;

    protected IslandGrid islandGrid;
    public List<Node> path = new List<Node>();

    [SerializeField] GameObject SquadUnitGameObject = null;
    [SerializeField] Transform[] SquadPositions = null;

    protected List<SquadUnit> squadUnits;
    SquadManager mySquadManager;

    public UnitBehaviour CompositeUnitBehaviour;
    [Range(0f, 100f)]
    public float DriveFactor = 10f;
    [Range(1f, 100f)]
    public float MaxSpeed = 5f;
    [Range(1f, 20f)]
    public float NeighborRadius = 1.5f;
    [Range(0f, 1f)]
    public float AvoidanceRadiusMultiplier = 0.5f;

    float squareMaxSpeed;
    float squareNeighborRadius;
    float avoidanceRadius;
    float squareAvoidanceRadius;

    Vector3 nodePositionOffset = Vector3.up * 5f;

    protected virtual void Awake()
    {
        islandGrid = FindObjectOfType<IslandGrid>();
        squadUnits = new List<SquadUnit>();
        nodeLayerMask = LayerMask.GetMask("Node");

        squareMaxSpeed = MaxSpeed * MaxSpeed;
        squareNeighborRadius = NeighborRadius * NeighborRadius;
        avoidanceRadius = NeighborRadius * AvoidanceRadiusMultiplier;
        squareAvoidanceRadius = squareNeighborRadius * AvoidanceRadiusMultiplier * AvoidanceRadiusMultiplier;
    }

    private void Start()
    {
        SetCurrentNode();

        SpawnSquadUnits();
    }

    protected virtual void SpawnSquadUnits()
    {
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

    void SetCurrentNode()
    {
        if (Physics.Raycast(transform.position + transform.up * rayOffset, -transform.up, out RaycastHit raycastHit,
            rayDistance, nodeLayerMask, QueryTriggerInteraction.Ignore))
        {
            Node hitNode = raycastHit.transform.GetComponent<Node>();
            currentNode = hitNode;

            if (currentNode.currentSquad != null)
            {
                if (PreviousNode != null && currentNode.currentSquad.PreviousNode.currentSquad == null)
                {
                    currentNode.currentSquad.MoveToTarget(currentNode.currentSquad.PreviousNode);
                }
                else if (PreviousNode != null && PreviousNode.currentSquad == null)
                {
                    currentNode.currentSquad.MoveToTarget(PreviousNode);
                }
                else
                {
                    bool didMove = false;
                    for (int i = 0; i < currentNode.Neighbours.Count; ++i)
                    {
                        if (currentNode.Neighbours[i].currentSquad == null)
                        {
                            currentNode.currentSquad.MoveToTarget(currentNode.Neighbours[i]);
                            didMove = true;
                            break;
                        }
                    }
                    if (!didMove)
                    {
                        if (currentNode.currentSquad.PreviousNode != null)
                        {
                            currentNode.currentSquad.MoveToTarget(currentNode.currentSquad.PreviousNode);
                        }
                        else
                        {
                            currentNode.currentSquad.MoveToTarget(currentNode.Neighbours[0]);
                        }
                    }
                }
            }
            currentNode.currentSquad = this;
        }
    }

    public void MoveToTarget(Node destinationNode)
    {
        islandGrid.ResetNodeValues();
        currentNode.visited = 0;
        currentNode.currentSquad = null;
        islandGrid.SetNodeDistances(currentNode);
        targetNode = destinationNode;
        path = islandGrid.GetPath(destinationNode);

        OnUpdateNavMeshAgents?.Invoke(destinationNode.transform.position + nodePositionOffset);
        StartCoroutine(MoveToTargetCoroutine());

        //Tell each squad unit about it's destination and let them handle
        //the pathfinding
    }

    public void SetSquadManager(SquadManager squadManager)
    {
        mySquadManager = squadManager;
    }

    private IEnumerator MoveToTargetCoroutine()
    {
        for (int nodeIndex = 1; nodeIndex < path.Count; ++nodeIndex)
        {
            Vector3 startPosition = transform.position;
            Vector3 destination = path[nodeIndex].transform.position + nodePositionOffset;
            float timer = 0f;

            while (timer < 1f)
            {
                timer += Time.deltaTime * MaxSpeed * 0.2f;
                transform.position = Vector3.Lerp(startPosition, destination, timer);
                OnAnimateSquadPath?.Invoke();

                yield return null;
            }

            if (path.Count > 0)
            {
                PreviousNode = path[0];
                path.RemoveAt(nodeIndex - 1);
                --nodeIndex;
            }
            else
            {
                PreviousNode = null;
            }
        }

        mySquadState = SquadState.Ready;
        path.Clear();

        SetCurrentNode();
        targetNode = null;

        OnAnimateSquadPath?.Invoke();
    }

    private void Update()
    {
        for (int i = 0; i < squadUnits.Count; ++i)
        {
            if (squadUnits[i].gameObject.activeInHierarchy)
            {
                List<Transform> context = GetNearbyObjects(squadUnits[i]);

                Vector3 move = Vector3.zero;
                move = CompositeUnitBehaviour.CalculateMove(squadUnits[i], context, this);
                move *= DriveFactor;

                if (move.sqrMagnitude > squareMaxSpeed)
                {
                    move = move.normalized * MaxSpeed;
                }
                squadUnits[i].Move(move);
            }
            else
            {
                squadUnits.Remove(squadUnits[i]);
            }
        }
    }

    List<Transform> GetNearbyObjects(SquadUnit squadUnit)
    {
        List<Transform> context = new List<Transform>();
        Collider[] contextColliders = Physics.OverlapSphere(
            squadUnit.transform.position, NeighborRadius);

        for (int i = 0; i < contextColliders.Length; ++i)
        {
            if (contextColliders[i] != squadUnit.UnitCollider)
            {
                context.Add(contextColliders[i].transform);
            }
        }

        return context;
    }

    protected void AnimateSquadPath()
    {
        OnAnimateSquadPath?.Invoke();
    }

    protected void AnimateSquadSelected()
    {
        OnAnimateSquadSelected?.Invoke();
    }

    protected void AnimateSquadDeselected()
    {
        OnAnimateSquadDeselected?.Invoke();
    }

    protected void UpdateNavMeshAgents(Vector3 targetPosition)
    {
        OnUpdateNavMeshAgents?.Invoke(targetPosition);
    }
}
