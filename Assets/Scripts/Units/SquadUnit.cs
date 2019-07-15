using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

public class SquadUnit : MonoBehaviour
{
    Squad mySquad;
    Transform myTargetTransform;
    NavMeshAgent myNavMeshAgent;

    private void Awake()
    {
        myNavMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        myNavMeshAgent.enabled = true;
    }

    public void InitializeSquadUnit(Squad squad, Transform targetTrasnform)
    {
        mySquad = squad;
        myTargetTransform = targetTrasnform;

        mySquad.OnUpdateNavMeshAgents += MySquad_OnUpdateNavMeshAgents;
    }

    void MySquad_OnUpdateNavMeshAgents()
    {
        myNavMeshAgent.SetDestination(myTargetTransform.position);
    }

    private void Update()
    {
    }
}
