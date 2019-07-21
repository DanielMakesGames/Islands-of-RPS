using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SquadUnit : MonoBehaviour
{
    public delegate void AnimationAction();
    public event AnimationAction OnAnimateIdle;
    public event AnimationAction OnAnimateMovement;
    public event AnimationAction OnAnimateAttack;
    public event AnimationAction OnAnimateDeath;

    protected Squad mySquad;
    public Squad CommandingSquad
    {
        get { return mySquad; }
    }

    protected NavMeshAgent myNavMeshAgent;
    public NavMeshAgent NavMeshAgent
    {
        get { return myNavMeshAgent; }
    }

    protected Collider myCollider;
    public Collider UnitCollider
    {
        get { return myCollider; }
    }

    protected Transform myTargetTransform;
    [SerializeField] Material highlightMaterial = null;
    Renderer[] myRenderers;
    Material[] defaultMaterials = null;
    Vector3 velocity;

    public enum DamageType
    {
        Rock,
        Paper,
        Scissors
    }
    [SerializeField] protected DamageType damageType;

    [SerializeField] float health = 100f;
    [SerializeField] protected float attackDamage = 50f;
    [SerializeField] protected float attackTime = 1f;

    [Range(0f, 1f)]
    [SerializeField] float rockDefense = 0f;
    [Range(0f, 1f)]
    [SerializeField] float paperDefense = 0f;
    [Range(0f, 1f)]
    [SerializeField] float scissorDefense = 0f;

    private void Awake()
    {
        myNavMeshAgent = GetComponent<NavMeshAgent>();
        myRenderers = GetComponentsInChildren<Renderer>();
        myCollider = GetComponentInChildren<Collider>();

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

        mySquad.OnAnimateSquadSelected += OnAnimateSquadSelected;
        mySquad.OnAnimateSquadDeselected += OnAnimateSquadDeselected;
    }

    private void Update()
    {
        if (myNavMeshAgent.enabled && !myNavMeshAgent.isOnOffMeshLink)
        {
            NavMeshPath path = new NavMeshPath();
            myNavMeshAgent.CalculatePath(myTargetTransform.position, path);
            myNavMeshAgent.path = path;
            myNavMeshAgent.isStopped = true;
        }
    }

    void OnAnimateSquadSelected()
    {
        for (int i = 0; i < myRenderers.Length; ++i)
        {
            myRenderers[i].material = highlightMaterial;
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
        velocity.y = 0f;

        if (velocity.sqrMagnitude > 1f)
        {
            transform.forward = velocity;
            OnAnimateMovement?.Invoke();
        }
        else
        {
            OnAnimateIdle?.Invoke();
        }

        if (myNavMeshAgent.enabled && !myNavMeshAgent.isOnOffMeshLink)
        {
            myNavMeshAgent.Move(velocity * Time.deltaTime);
        }
    }

    public void Attack(SquadUnit targetSquadUnit)
    {
        StartCoroutine(AttackAnimation(targetSquadUnit));
    }

    public void Attack(TownCenter targetTownCenter)
    {
        StartCoroutine(AttackAnimation(targetTownCenter));
    }

    public void ReceiveDamage(float damage, DamageType opponentDamageType)
    {
        switch (opponentDamageType)
        {
            case DamageType.Rock:
                health -= damage * (1f - rockDefense);
                break;
            case DamageType.Paper:
                health -= damage * (1f - paperDefense);
                break;
            case DamageType.Scissors:
                health -= damage * (1f - scissorDefense);
                break;
        }

        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        mySquad.OnAnimateSquadSelected -= OnAnimateSquadSelected;
        mySquad.OnAnimateSquadDeselected -= OnAnimateSquadDeselected;

        OnAnimateDeath?.Invoke();
        Destroy(gameObject, 1f);
    }

    protected virtual IEnumerator AttackAnimation(SquadUnit targetSquadUnit)
    {
        myNavMeshAgent.enabled = false;
        AnimateAttack();
        targetSquadUnit.ReceiveDamage(attackDamage, damageType);
        yield return new WaitForSeconds(attackTime);
        myNavMeshAgent.enabled = true;
    }

    protected virtual IEnumerator AttackAnimation(TownCenter targetTownCenter)
    {
        myNavMeshAgent.enabled = false;
        AnimateAttack();
        targetTownCenter.ReceiveDamage(attackDamage, damageType);
        yield return new WaitForSeconds(attackTime);
        myNavMeshAgent.enabled = true;
    }

    protected void AnimateAttack()
    {
        OnAnimateAttack?.Invoke();
    }
}
