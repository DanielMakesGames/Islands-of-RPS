using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

public class SquadUnit : MonoBehaviour
{
    protected Squad mySquad;
    protected Transform myTargetTransform;
    protected NavMeshAgent myNavMeshAgent;
    Renderer[] myRenderers;

    [SerializeField] Material HighLightMaterial = null;
    Material[] defaultMaterials = null;

    private void Awake()
    {
        myNavMeshAgent = GetComponent<NavMeshAgent>();
        myRenderers = GetComponentsInChildren<Renderer>();

        defaultMaterials = new Material[myRenderers.Length];
        for (int i = 0; i < myRenderers.Length; ++i)
        {
            defaultMaterials[i] = myRenderers[i].material;
        }
    }

    protected virtual void Start()
    {
        EnableNavMeshAgent();
    }

    public void InitializeSquadUnit(Squad squad, Transform targetTrasnform)
    {
        mySquad = squad;
        myTargetTransform = targetTrasnform;

        mySquad.OnUpdateNavMeshAgents += OnUpdateNavMeshAgents;
        mySquad.OnAnimateSquadSelected += OnAnimateSquadSelected;
        mySquad.OnAnimateSquadDeselected += OnAnimateSquadDeselected;
    }

    void OnUpdateNavMeshAgents()
    {
        myNavMeshAgent.SetDestination(myTargetTransform.position);
    }

    void OnAnimateSquadSelected()
    {
        for (int i = 0; i < myRenderers.Length; ++i)
        {
            myRenderers[i].material = HighLightMaterial;
        }
    }

    void OnAnimateSquadDeselected()
    {
        for (int i = 0; i < myRenderers.Length; ++i)
        {
            myRenderers[i].material = defaultMaterials[i];
        }
    }

    private void Update()
    {
    }

    public void EnableNavMeshAgent()
    {
        myNavMeshAgent.enabled = true;
    }
}
