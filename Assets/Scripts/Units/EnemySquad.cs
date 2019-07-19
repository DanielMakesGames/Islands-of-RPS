using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySquad : Squad
{
    public delegate void EnemySquadAction();

    EnemyTransport myEnemyTransport;

    protected override void Awake()
    {
        base.Awake();

        myEnemyTransport = GetComponentInParent<EnemyTransport>();
        myEnemyTransport.OnEnemyTransportLanded += OnEnemyTransportLanded;
    }

    private void OnEnable()
    {

    }

    private void OnDisable()
    {

    }

    void OnEnemyTransportLanded(Node landingNode)
    {
        transform.parent = null;
        for (int i = 0; i < squadUnits.Count; ++i)
        {
            squadUnits[i].transform.parent = null;
            squadUnits[i].EnableNavMeshAgent();
        }
        currentNode = landingNode;
        MoveToTarget(landingNode);
        AnimateSquadPath();
    }

    protected override void SpawnSquadUnits()
    {
        base.SpawnSquadUnits();

        for (int i = 0; i < squadUnits.Count; ++i)
        {
            squadUnits[i].transform.parent = transform;
        }
    }
}
