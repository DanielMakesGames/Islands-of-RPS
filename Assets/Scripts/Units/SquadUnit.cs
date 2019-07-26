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
    public event AnimationAction OnAnimateReceiveDamage;

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

    const float minimumSqrSpeed = 25f;

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

    private void OnEnable()
    {
        ReturnToTitleButton.OnPressed += ReturnToTitleButtonOnButtonPress;
    }

    private void OnDisable()
    {
        ReturnToTitleButton.OnPressed -= ReturnToTitleButtonOnButtonPress;
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
        if (myNavMeshAgent.enabled && !myNavMeshAgent.isOnOffMeshLink)
        {
            NavMeshPath path = new NavMeshPath();
            myNavMeshAgent.CalculatePath(myTargetTransform.position, path);
            myNavMeshAgent.path = path;
            myNavMeshAgent.isStopped = true;

            this.velocity = velocity;
            velocity.y = 0f;

            if (velocity.sqrMagnitude > minimumSqrSpeed)
            {
                transform.forward = velocity;
                OnAnimateMovement?.Invoke();
            }
            else
            {
                OnAnimateIdle?.Invoke();
            }

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

    public void ReceiveDamage(Transform opponentTransform, float damage, DamageType opponentDamageType)
    {
        switch (opponentDamageType)
        {
            case DamageType.Rock:
                health -= damage * (1f - rockDefense);
                //animate rock hit
                //squish animation
                break;
            case DamageType.Paper:
                health -= damage * (1f - paperDefense);
                //animate paper hit
                //blinking animation
                break;
            case DamageType.Scissors:
                health -= damage * (1f - scissorDefense);
                StartCoroutine(ScissorHitAnimation(opponentTransform));
                //animate scissor hit
                //knock back animation
                break;
        }

        OnAnimateReceiveDamage?.Invoke();

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

        Vector3 start = transform.position + Vector3.up;
        Vector3 end = targetSquadUnit.transform.position;
        start.y = 0f;
        end.y = 0f;

        Vector3 direction = end - start;
        direction.Normalize();
        transform.forward = direction;

        AnimateAttack();
        targetSquadUnit.ReceiveDamage(transform, attackDamage, damageType);
        yield return new WaitForSeconds(attackTime);
        myNavMeshAgent.enabled = true;
    }

    protected virtual IEnumerator AttackAnimation(TownCenter targetTownCenter)
    {
        myNavMeshAgent.enabled = false;

        Vector3 start = transform.position + Vector3.up;
        Vector3 end = targetTownCenter.transform.position;
        start.y = 0f;
        end.y = 0f;

        Vector3 direction = end - start;
        direction.Normalize();
        transform.forward = direction;

        AnimateAttack();
        targetTownCenter.ReceiveDamage(attackDamage, damageType);
        yield return new WaitForSeconds(attackTime);
        myNavMeshAgent.enabled = true;
    }

    protected void AnimateAttack()
    {
        OnAnimateAttack?.Invoke();
    }

    void ReturnToTitleButtonOnButtonPress()
    {
        Destroy(gameObject);
    }

    IEnumerator ScissorHitAnimation(Transform opponentTransform)
    {
        yield return null;
    }

    IEnumerator PaperHitAnimation()
    {
        yield return null;
    }

    IEnumerator RockHitAnimation()
    {
        yield return null;
    }

    IEnumerator BlinkAnimation()
    {
        for (int blinks = 0; blinks < 3; ++ blinks)
        {
            for (int i = 0; i < myRenderers.Length; ++i)
            {
                myRenderers[i].enabled = false;
            }
            yield return null;
            for (int i = 0; i < myRenderers.Length; ++i)
            {
                myRenderers[i].enabled = true;
            }
            yield return null;
        }
    }
}
