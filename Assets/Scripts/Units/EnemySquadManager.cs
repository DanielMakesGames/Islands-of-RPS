using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySquadManager : SquadManager
{
    public delegate void EnemySquadManagerAction();
    public static event EnemySquadManagerAction OnAllEnemiesDefeated;

    [SerializeField] EnemyWave[] EnemyWaves = null;

    int currentEnemyWaveIndex = 0;
    int numberOfEnemySquads = 0;
    int currentEnemySquadDestroyed = 0;

    public SquadBehaviour CompositeSquadBehaviour;
    [Range(1f, 20f)]
    public float NeighborRadius = 10f;

    TownCenter myTownCenter;

    private void OnEnable()
    {
        GameplayManager.OnGameplayStart += StartGame;

        EnemySquad.OnEnemySquadDestroyed += OnEnemySquadDestroyed;
    }

    private void OnDisable()
    {
        GameplayManager.OnGameplayStart -= StartGame;

        EnemySquad.OnEnemySquadDestroyed -= OnEnemySquadDestroyed;
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
        yield return new WaitForSeconds(spawnTime);

        GameObject clone = Instantiate(enemyBoat);
        clone.transform.position = spawnLocation;
        clone.transform.forward = direction;

        Squad[] squads = clone.GetComponentsInChildren<Squad>();
        for (int i = 0; i < squads.Length; ++i)
        {
            mySquads.Add(squads[i]);
            squads[i].SetSquadManager(this);
        }
    }

    private void Update()
    {
        for (int i = 0; i < mySquads.Count; ++i)
        {
            if (mySquads[i] && mySquads[i].gameObject.activeInHierarchy)
            {
                if (mySquads[i].CurrentSquadState == Squad.SquadState.OnTransport ||
                    mySquads[i].CurrentSquadState == Squad.SquadState.Moving)
                {
                    break;
                }
                List<Transform> context = GetNearbyObjects(mySquads[i]);

                Vector3 move = Vector3.zero;
                //move = CompositeSquadBehaviour.CalculateMove(mySquads[i], context, this);

                if (move == Vector3.zero)
                {
                    mySquads[i].MoveToTarget(
                        mySquads[i].IslandGrid.FindClosest(
                            mySquads[i].transform, myTownCenter.Neighbors));
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
        }
    }

}
