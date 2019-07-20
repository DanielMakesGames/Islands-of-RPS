using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownCenter : MonoBehaviour
{
    public delegate void SpawnSquadAction(Squad newSquad);
    public static event SpawnSquadAction OnSpawnNewSquad;

    [SerializeField] GameObject RockSquad = null;
    [SerializeField] GameObject PaperSquad = null;
    [SerializeField] GameObject ScissorSquad = null;

    [SerializeField] Node frontLeftNode;
    [SerializeField] Node frontRightNode;

    LayerMask nodeLayerMask;
    const float rayDistance = 1f;
    const float rayOffset = 0.5f;

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

        if (frontLeftNode.currentSquad == null)
        {
            squad.MoveFromTownCenter(frontLeftNode);
        }
        else if (frontRightNode.currentSquad == null)
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
}
