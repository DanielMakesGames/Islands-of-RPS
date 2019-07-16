using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownCenter : MonoBehaviour
{
    [SerializeField] GameObject RockSquad = null;
    [SerializeField] GameObject PaperSquad = null;
    [SerializeField] GameObject ScissorSquad = null;

    Node currentNode;
    Node nextNode;

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

    private void Start()
    {
        SetCurrentNode();
    }

    void SpawnRockSquad()
    {
        GameObject clone = Instantiate(RockSquad);
        clone.transform.position = transform.position;
        clone.transform.rotation = transform.rotation;

        Squad squadClone = clone.GetComponent<Squad>();
        StartCoroutine(MoveSquadToTarget(squadClone, nextNode));
    }

    void SpawnPaperSquad()
    {
        GameObject clone = Instantiate(PaperSquad);
        clone.transform.position = transform.position;
        clone.transform.rotation = transform.rotation;

        Squad squadClone = clone.GetComponent<Squad>();
        StartCoroutine(MoveSquadToTarget(squadClone, nextNode));
    }

    void SpawnScissorSquad()
    {
        GameObject clone = Instantiate(ScissorSquad);
        clone.transform.position = transform.position;
        clone.transform.rotation = transform.rotation;

        Squad squadClone = clone.GetComponent<Squad>();
        StartCoroutine(MoveSquadToTarget(squadClone, nextNode));
    }

    void SetCurrentNode()
    {
        if (Physics.Raycast(transform.position + transform.up * rayOffset, -transform.up, out RaycastHit raycastHit,
            rayDistance, nodeLayerMask, QueryTriggerInteraction.Ignore))
        {
            Node hitNode = raycastHit.transform.GetComponent<Node>();
            currentNode = hitNode;
        }

        nextNode = currentNode.NorthNode;
        if (transform.forward == Vector3.forward)
        {
            nextNode = currentNode.NorthNode;
        }
        else if (transform.forward == Vector3.right)
        {
            nextNode = currentNode.EastNode;
        }
        else if (transform.forward == -Vector3.forward)
        {
            nextNode = currentNode.SouthNode;
        }
        else if (transform.forward == -Vector3.right)
        {
            nextNode = currentNode.WestNode;
        }
    }

    IEnumerator MoveSquadToTarget(Squad squad, Node target)
    {
        yield return null;
        squad.MoveToTarget(target);
    }
}
