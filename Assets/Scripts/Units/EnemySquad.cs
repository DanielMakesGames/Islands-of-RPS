﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySquad : Squad
{
    public delegate void EnemySquadAction();
    public static event EnemySquadAction OnEnemySquadDestroyed;

    EnemyTransport myEnemyTransport;

    protected override void Awake()
    {
        base.Awake();

        mySquadState = SquadState.OnTransport;
        myEnemyTransport = GetComponentInParent<EnemyTransport>();
        myEnemyTransport.OnEnemyTransportLanded += OnEnemyTransportLanded;
    }

    private void OnEnable()
    {

    }

    private void OnDisable()
    {

    }

    protected override void SpawnSquadUnits()
    {
        base.SpawnSquadUnits();

        for (int i = 0; i < squadUnits.Count; ++i)
        {
            squadUnits[i].transform.parent = transform;
        }
    }

    protected override void SetCurrentNode()
    {
        if (Physics.Raycast(transform.position + transform.up * rayOffset, -transform.up, out RaycastHit raycastHit,
            rayDistance, nodeLayerMask, QueryTriggerInteraction.Ignore))
        {
            Node hitNode = raycastHit.transform.GetComponent<Node>();
            currentNode = hitNode;

            if (currentNode.CurrentEnemySquad != null)
            {
                if (currentNode.CurrentEnemySquad.PreviousNode != null && currentNode.CurrentEnemySquad.PreviousNode.CurrentEnemySquad == null)
                {
                    currentNode.CurrentEnemySquad.MoveToTarget(currentNode.CurrentEnemySquad.PreviousNode);
                }
                else if (PreviousNode != null && PreviousNode.CurrentEnemySquad == null)
                {
                    currentNode.CurrentEnemySquad.MoveToTarget(PreviousNode);
                }
                else
                {
                    bool didMove = false;
                    for (int i = 0; i < currentNode.Neighbours.Count; ++i)
                    {
                        if (currentNode.Neighbours[i].CurrentEnemySquad == null)
                        {
                            currentNode.CurrentEnemySquad.MoveToTarget(currentNode.Neighbours[i]);
                            didMove = true;
                            break;
                        }
                    }
                    if (!didMove)
                    {

                        currentNode.CurrentEnemySquad.MoveToTarget(currentNode.Neighbours[
                            Random.Range(0, currentNode.Neighbours.Count)]);
                    }
                }
            }
            currentNode.CurrentEnemySquad = this;
        }
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

    public override void MoveToTarget(Node destinationNode)
    {
        mySquadState = Squad.SquadState.Moving;

        islandGrid.ResetEnemyNodeValues();
        currentNode.EnemyVisited = 0;
        currentNode.CurrentEnemySquad = null;
        islandGrid.SetEnemyNodeDistances(currentNode);
        targetNode = destinationNode;
        path = islandGrid.GetEnemyPath(destinationNode);

        UpdateNavMeshAgents(destinationNode.transform.position + nodePositionOffset);
        StopAllCoroutines();
        StartCoroutine(MoveToTargetCoroutine());
    }

    protected override void Die()
    {
        OnEnemySquadDestroyed?.Invoke();
        base.Die();
    }
}