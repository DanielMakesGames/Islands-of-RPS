using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitTargetPath : MonoBehaviour
{
    [SerializeField] bool isDebugging = false;

    LineRenderer myLineRenderer;
    NavMeshAgent myNavMeshAgent;
    MeshRenderer myMeshRenderer;

    void Awake()
    {
        myLineRenderer = GetComponent<LineRenderer>();
        myNavMeshAgent = GetComponentInParent<NavMeshAgent>();
        myMeshRenderer = GetComponent<MeshRenderer>();

        if (isDebugging)
        {
            myLineRenderer.enabled = true;
            myMeshRenderer.enabled = true;
        }
        else
        {
            myLineRenderer.enabled = false;
            myMeshRenderer.enabled = false;
        }
    }

    private void Update()
    {
        if (myNavMeshAgent)
        {
            if (isDebugging)
            {
                myLineRenderer.positionCount = myNavMeshAgent.path.corners.Length;
                myLineRenderer.SetPositions(myNavMeshAgent.path.corners);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
