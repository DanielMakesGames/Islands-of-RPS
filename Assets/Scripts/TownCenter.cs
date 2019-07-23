using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownCenter : MonoBehaviour
{
    public delegate void TownCenterAction();
    public static event TownCenterAction OnTownCenterDestroyed;

    public delegate void SpawnSquadAction(Squad newSquad);
    public static event SpawnSquadAction OnSpawnNewSquad;

    [SerializeField] GameObject TownCenterExplosion;

    [SerializeField] GameObject RockSquad = null;
    [SerializeField] GameObject PaperSquad = null;
    [SerializeField] GameObject ScissorSquad = null;

    [SerializeField] Node frontLeftNode = null;
    [SerializeField] Node frontRightNode = null;

    public List<Node> Neighbors;

    LayerMask nodeLayerMask;
    const float rayDistance = 1f;
    const float rayOffset = 0.5f;

    float health = 100f;
    public float Heatlh
    {
        get { return health; }
    }

    [Range(0f, 1f)]
    [SerializeField] float rockDefense = 0f;
    [Range(0f, 1f)]
    [SerializeField] float paperDefense = 0f;
    [Range(0f, 1f)]
    [SerializeField] float scissorDefense = 0f;

    private void Awake()
    {
        nodeLayerMask = LayerMask.GetMask("Node");
    }

    private void OnEnable()
    {
        RockSquadButton.OnPressed += SpawnRockSquad;
        PaperSquadButton.OnPressed += SpawnPaperSquad;
        ScissorSquadButton.OnPressed += SpawnScissorSquad;
    }

    private void OnDisable()
    {
        RockSquadButton.OnPressed -= SpawnRockSquad;
        PaperSquadButton.OnPressed -= SpawnPaperSquad;
        ScissorSquadButton.OnPressed -= SpawnScissorSquad;
    }

    void SpawnRockSquad()
    {
        GameObject clone = Instantiate(RockSquad);
        clone.transform.position = transform.position;
        clone.transform.rotation = transform.rotation;

        Squad squadClone = clone.GetComponent<Squad>();
        StartCoroutine(MoveSquadToTarget(squadClone, frontLeftNode));

        OnSpawnNewSquad?.Invoke(squadClone);
    }

    void SpawnPaperSquad()
    {
        GameObject clone = Instantiate(PaperSquad);
        clone.transform.position = transform.position;
        clone.transform.rotation = transform.rotation;

        Squad squadClone = clone.GetComponent<Squad>();
        StartCoroutine(MoveSquadToTarget(squadClone, frontLeftNode));

        OnSpawnNewSquad?.Invoke(squadClone);
    }

    public void ReceiveDamage(float damage, SquadUnit.DamageType damageType)
    {
        switch (damageType)
        {
            case SquadUnit.DamageType.Rock:
                health -= damage * (1f - rockDefense);
                break;
            case SquadUnit.DamageType.Paper:
                health -= damage * (1f - paperDefense);
                break;
            case SquadUnit.DamageType.Scissors:
                health -= damage * (1f - scissorDefense);
                break;
        }

        if (health <= 0)
        {
            TownCenterDestroyed();
        }
    }

    void SpawnScissorSquad()
    {
        GameObject clone = Instantiate(ScissorSquad);
        clone.transform.position = transform.position;
        clone.transform.rotation = transform.rotation;

        Squad squadClone = clone.GetComponent<Squad>();
        StartCoroutine(MoveSquadToTarget(squadClone, frontLeftNode));

        OnSpawnNewSquad?.Invoke(squadClone);
    }

    IEnumerator MoveSquadToTarget(Squad squad, Node target)
    {
        yield return null;

        if (frontLeftNode.CurrentPlayerSquad == null)
        {
            squad.MoveFromTownCenter(frontLeftNode);
        }
        else if (frontRightNode.CurrentPlayerSquad == null)
        {
            squad.MoveFromTownCenter(frontRightNode);
        }
        else
        {
            if (Random.Range(0, 2) == 0)
            {
                squad.MoveFromTownCenter(frontLeftNode);
            }
            else
            {
                squad.MoveFromTownCenter(frontRightNode);
            }
        }
    }

    void TownCenterDestroyed()
    {
        GameObject clone = Instantiate(TownCenterExplosion);
        clone.transform.position = transform.position;
        OnTownCenterDestroyed?.Invoke();
        Destroy(gameObject);
    }
}
