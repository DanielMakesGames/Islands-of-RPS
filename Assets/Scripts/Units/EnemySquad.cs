using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySquad : Squad
{
    public delegate void EnemySquadAction();
    public static event EnemySquadAction OnEnemySquadDestroyed;

    EnemyTransport myEnemyTransport;
    float decisionMakingDelay = 0f;

    protected override void Awake()
    {
        base.Awake();

        mySquadState = SquadState.OnTransport;
        myEnemyTransport = GetComponentInParent<EnemyTransport>();
        myEnemyTransport.OnEnemyTransportLanded += OnEnemyTransportLanded;
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

        CompositeUnitBehaviour.Weights[0] = movementWeight;
        targetNode = landingNode;
        transform.position = targetNode.transform.position + nodePositionOffset;
        mySquadState = SquadState.Ready;

        path.Clear();
        SetCurrentNode();
        targetNode = null;
    }

    public override void MoveToTarget(Node destinationNode)
    {
        if (currentNode == destinationNode)
        {
            return;
        }
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

    protected override IEnumerator MoveToTargetCoroutine()
    {
        yield return StartCoroutine(base.MoveToTargetCoroutine());

        mySquadState = SquadState.Moving;
        yield return new WaitForSeconds(decisionMakingDelay);
        mySquadState = SquadState.Ready;
    }

    public void SetDecisionMakingDelay(float delay)
    {
        decisionMakingDelay = delay;
    }

    protected override void Die()
    {
        OnEnemySquadDestroyed?.Invoke();
        base.Die();
    }

    protected override void DisableSquad()
    {
        if (transform.parent == null)
        {
            Destroy(gameObject);
        }
        else
        {
            myEnemyTransport.OnEnemyTransportLanded -= OnEnemyTransportLanded;
            this.enabled = false;
        }
    }
}