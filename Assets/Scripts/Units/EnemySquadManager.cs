using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySquadManager : SquadManager
{
    public delegate void EnemySquadManagerAction();
    public static event EnemySquadManagerAction OnAllEnemiesDefeated;

    [SerializeField] EnemyWave[] EnemyWaves = null;
    int currentEnemyWaveIndex = 0;
    public EnemyWave CurrentEnemyWave
    {
        get { return EnemyWaves[currentEnemyWaveIndex]; }
    }

    int numberOfEnemySquads = 0;
    int currentEnemySquadDestroyed = 0;

    public SquadBehaviour CompositeRockSquadBehaviour;
    public SquadBehaviour CompositePaperSquadBehaviour;
    public SquadBehaviour CompositeScissorSquadBehaviour;

    [Range(1f, 20f)]
    public float NeighborRadius = 10f;
    [Range(0f, 1f)]
    public float AvoidanceRadiusMultiplier = 0.5f;

    [Range(0f, 10f)]
    public float SquadDecisionMakingDelay = 1f;

    float squareNeighborRadius;
    float avoidanceRadius;
    float squareAvoidanceRadius;
    public float SquareAvoidanceRadius
    {
        get { return squareAvoidanceRadius; }
    }

    TownCenter myTownCenter;

    protected override void Awake()
    {
        base.Awake();

        avoidanceRadius = NeighborRadius * AvoidanceRadiusMultiplier;
        squareNeighborRadius = NeighborRadius * NeighborRadius;
        squareAvoidanceRadius = squareNeighborRadius * AvoidanceRadiusMultiplier * AvoidanceRadiusMultiplier;
    }

    private void OnEnable()
    {
        GameplayManager.OnStartGameplay += StartGame;

        EnemySquad.OnEnemySquadDestroyed += OnEnemySquadDestroyed;

        TownCenter.OnTownCenterDestroyed += DisableSquadManager;
    }

    private void OnDisable()
    {
        GameplayManager.OnStartGameplay -= StartGame;

        EnemySquad.OnEnemySquadDestroyed -= OnEnemySquadDestroyed;

        TownCenter.OnTownCenterDestroyed -= DisableSquadManager;
    }

    private void StartGame()
    {
        myTownCenter = FindObjectOfType<TownCenter>();
        if (currentEnemyWaveIndex < EnemyWaves.Length)
        {
            InstantiateEnemyWave(currentEnemyWaveIndex);
        }
    }

    void InstantiateEnemyWave(int enemyWaveIndex)
    {
        for (int i = 0; i < EnemyWaves[enemyWaveIndex].EnemyBoats.Length; ++i)
        {
            ++numberOfEnemySquads;
            StartCoroutine(InstantiateEnemyBoat(
                EnemyWaves[enemyWaveIndex].EnemyBoats[i],
                EnemyWaves[enemyWaveIndex].SpawnLocations[i],
                EnemyWaves[enemyWaveIndex].Directions[i],
                EnemyWaves[enemyWaveIndex].SpawnTimes[i]));
        }
    }

    IEnumerator InstantiateEnemyBoat(GameObject enemyBoat, Vector3 spawnLocation,
        Vector3 direction, float spawnTime)
    {
        yield return new WaitForSeconds(spawnTime + 12f);

        GameObject clone = Instantiate(enemyBoat);
        clone.transform.position = spawnLocation;
        clone.transform.forward = direction;

        EnemySquad[] squads = clone.GetComponentsInChildren<EnemySquad>();
        for (int i = 0; i < squads.Length; ++i)
        {
            mySquads.Add(squads[i]);
            squads[i].SetSquadManager(this);
            squads[i].SetDecisionMakingDelay(SquadDecisionMakingDelay);
        }
    }

    private void Update()
    {
        UpdateSquads();
    }

    void UpdateSquads()
    {
        for (int i = 0; i < mySquads.Count; ++i)
        {

            if (mySquads[i] && mySquads[i].gameObject.activeInHierarchy)
            {
                if (mySquads[i].CurrentSquadState == Squad.SquadState.Ready)
                {
                    List<Transform> context = GetNearbyObjects(mySquads[i]);

                    Vector3 move = Vector3.zero;

                    switch (mySquads[i].RPSType)
                    {
                        case Squad.SquadType.Rock:
                            move = CompositeRockSquadBehaviour.CalculateMove(mySquads[i], context, this);
                            break;
                        case Squad.SquadType.Paper:
                            move = CompositePaperSquadBehaviour.CalculateMove(mySquads[i], context, this);
                            break;
                        case Squad.SquadType.Scissor:
                            move = CompositeScissorSquadBehaviour.CalculateMove(mySquads[i], context, this);
                            break;
                    }

                    if (move == Vector3.zero)
                    {
                        mySquads[i].MoveToTarget(
                            mySquads[i].IslandGrid.FindClosest(
                                mySquads[i].transform, myTownCenter.Neighbors));
                    }
                    else
                    {
                        //find the closest node to Move and move there.
                        List<Node> nearbyNodes = new List<Node>();
                        Collider[] nodeColliders = Physics.OverlapSphere(
                            mySquads[i].transform.position + move,
                            5f, LayerMask.GetMask("Node"), QueryTriggerInteraction.Ignore);

                        for (int j = 0; j < nodeColliders.Length; ++j)
                        {
                            Node nearbyNode = nodeColliders[j].GetComponent<Node>();
                            if (nearbyNode &&
                                nearbyNode.IsWalkable)
                            {
                                nearbyNodes.Add(nearbyNode);
                            }
                        }

                        //find closest node
                        if (nearbyNodes.Count > 0)
                        {
                            mySquads[i].MoveToTarget(
                                mySquads[i].IslandGrid.FindClosest(
                                    mySquads[i].transform, nearbyNodes));
                        }
                        else
                        {
                            mySquads[i].MoveToTarget(
                                mySquads[i].IslandGrid.FindClosest(
                                    mySquads[i].transform, myTownCenter.Neighbors));
                        }
                    }
                }
            }
            else
            {
                mySquads.Remove(mySquads[i]);
            }
        }
    }

    List<Transform> GetNearbyObjects(Squad squad)
    {
        List<Transform> context = new List<Transform>();
        Collider[] contextColliders = Physics.OverlapSphere(
            squad.transform.position + Vector3.up, NeighborRadius,
            Camera.main.cullingMask, QueryTriggerInteraction.Collide);

        for (int i = 0; i < contextColliders.Length; ++i)
        {
            if (contextColliders[i] != squad.SquadCollider)
            {
                context.Add(contextColliders[i].transform);
            }
        }

        return context;
    }

    void OnEnemySquadDestroyed()
    {
        ++currentEnemySquadDestroyed;
        if (currentEnemySquadDestroyed == numberOfEnemySquads)
        {
            OnAllEnemiesDefeated?.Invoke();
            DisableSquadManager();
        }
    }

    protected void DisableSquadManager()
    {
        this.enabled = false;
    }
}
