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
        Moving,
        OnTransport
    }
    protected SquadState mySquadState = SquadState.Ready;
    public SquadState CurrentSquadState
    {
        get { return mySquadState; }
    }

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

    protected const float rayDistance = 1f;
    protected const float rayOffset = 0.5f;
    protected LayerMask nodeLayerMask;

    public Node PathfindingNode;
    public Node PreviousNode;

    protected IslandGrid islandGrid;
    public IslandGrid IslandGrid
    {
        get { return islandGrid; }
    }

    public List<Node> path = new List<Node>();

    [SerializeField] GameObject SquadUnitGameObject = null;
    [SerializeField] Transform[] SquadPositions = null;

    protected List<SquadUnit> squadUnits;
    SquadManager mySquadManager;

    public CompositeUnitBehaviour CompositeUnitBehaviour;
    [Range(0f, 100f)]
    public float DriveFactor = 10f;
    [Range(1f, 100f)]
    public float MaxSpeed = 5f;
    [Range(1f, 30f)]
    public float NeighborRadius = 1.5f;
    [Range(0f, 1f)]
    public float AvoidanceRadiusMultiplier = 0.5f;
    [Range(0f, 1f)]
    public float AttackRadiusMultiplier = 0.5f;

    float squareMaxSpeed;
    float squareNeighborRadius;
    float avoidanceRadius;
    float squareAvoidanceRadius;
    public float SquareAvoidanceRadius
    {
        get { return squareAvoidanceRadius; }
    }
    float squareAttackRadius;
    public float SquareAttackRadius
    {
        get { return squareAttackRadius; }
    }

    protected Vector3 nodePositionOffset = Vector3.up * 5f;

    Collider myCollider;
    public Collider SquadCollider
    {
        get { return myCollider; }
    }

    const float sqrReadyDistance = 26f;
    const float movementWeight = 10f;

    protected virtual void Awake()
    {
        islandGrid = FindObjectOfType<IslandGrid>();
        squadUnits = new List<SquadUnit>();
        nodeLayerMask = LayerMask.GetMask("Node");
        myCollider = GetComponent<Collider>();

        squareMaxSpeed = MaxSpeed * MaxSpeed;
        squareNeighborRadius = NeighborRadius * NeighborRadius;
        avoidanceRadius = NeighborRadius * AvoidanceRadiusMultiplier;
        squareAvoidanceRadius = squareNeighborRadius * AvoidanceRadiusMultiplier * AvoidanceRadiusMultiplier;
        squareAttackRadius = squareNeighborRadius * AttackRadiusMultiplier;
    }

    private void OnEnable()
    {
        ReturnToTitleButton.OnPressed += ReturnToTitleButtonOnButtonPress;
    }

    private void OnDisable()
    {
        ReturnToTitleButton.OnPressed -= ReturnToTitleButtonOnButtonPress;
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

    protected virtual void SetCurrentNode()
    {
        if (Physics.Raycast(transform.position + transform.up * rayOffset, -transform.up, out RaycastHit raycastHit,
            rayDistance, nodeLayerMask, QueryTriggerInteraction.Ignore))
        {
            Node hitNode = raycastHit.transform.GetComponent<Node>();
            currentNode = hitNode;
        }
    }

    public virtual void MoveToTarget(Node destinationNode)
    {
        transform.position = targetNode.transform.position + nodePositionOffset;
        mySquadState = SquadState.Ready;
        path.Clear();

        SetCurrentNode();
        targetNode = null;
    }

    public virtual void MoveFromTownCenter(Node destinationNode)
    {
    }

    public void SetSquadManager(SquadManager squadManager)
    {
        mySquadManager = squadManager;
    }

    protected IEnumerator MoveToTargetCoroutine()
    {
        CompositeUnitBehaviour.Weights[0] = movementWeight;
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

        transform.position = targetNode.transform.position + nodePositionOffset;
        mySquadState = SquadState.Ready;
        path.Clear();

        SetCurrentNode();
        targetNode = null;

        OnAnimateSquadPath?.Invoke();
    }

    void ReturnToNormalMovementWeight()
    {
        if (mySquadState == SquadState.Ready)
        {
            bool areUnitsReady = true;
            for (int i = 0; i < squadUnits.Count; ++i)
            {
                if (Vector3.SqrMagnitude(transform.position - squadUnits[i].transform.position) > sqrReadyDistance)
                {
                    areUnitsReady = false;
                    break;
                }
            }

            if (areUnitsReady)
            {
                CompositeUnitBehaviour.Weights[0] = 1f;
            }
        }
    }

    private void Update()
    {
        for (int i = 0; i < squadUnits.Count; ++i)
        {
            if (squadUnits[i] && squadUnits[i].gameObject.activeInHierarchy)
            {
                if (squadUnits[i].NavMeshAgent.enabled)
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
            }
            else
            {
                squadUnits.Remove(squadUnits[i]);

                if (squadUnits.Count <= 0)
                {
                    Die();
                }
            }
        }
        ReturnToNormalMovementWeight();
    }

    List<Transform> GetNearbyObjects(SquadUnit squadUnit)
    {
        List<Transform> context = new List<Transform>();
        Collider[] contextColliders = Physics.OverlapSphere(
            squadUnit.transform.position + Vector3.up, NeighborRadius);

        for (int i = 0; i < contextColliders.Length; ++i)
        {
            if (contextColliders[i] != squadUnit.UnitCollider)
            {
                context.Add(contextColliders[i].transform);
            }
        }

        return context;
    }

    protected virtual void Die()
    {
        if (mySquadManager != null)
        {
            mySquadManager.RemoveSquad(this);
        }
        Destroy(gameObject);
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

    void ReturnToTitleButtonOnButtonPress()
    {
        Destroy(gameObject);
    }
}
