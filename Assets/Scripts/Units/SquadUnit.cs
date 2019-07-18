using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SquadUnit : MonoBehaviour
{
    protected Squad mySquad;
    protected Transform myTargetTransform;
    protected NavMeshAgent myNavMeshAgent;
    protected Collider myCollider;
    public Collider UnitCollider
    {
        get { return myCollider; }
    }

    Renderer[] myRenderers;

    [SerializeField] Material HighLightMaterial = null;
    Material[] defaultMaterials = null;

    LayerMask unitLayerMask;
    Vector3 velocity;

    private void Awake()
    {
        myNavMeshAgent = GetComponent<NavMeshAgent>();
        myRenderers = GetComponentsInChildren<Renderer>();
        myCollider = GetComponentInChildren<Collider>();
        unitLayerMask = LayerMask.GetMask(
            new string[] { "Player Unit", "Enemy Unit" });

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

    public void EnableNavMeshAgent()
    {
        myNavMeshAgent.enabled = true;
    }

    public void Move(Vector3 velocity)
    {
        this.velocity = velocity;

        if (velocity.sqrMagnitude > 0f)
        {
            transform.forward = velocity;
        }

        velocity.y = 0f;
        myNavMeshAgent.Move(velocity * Time.deltaTime);
    }
}
