using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownCenter : MonoBehaviour
{
    [SerializeField] GameObject ScissorsSquad = null;

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
        ScissorsSquadButton.OnPressed += ScissorsSquadButton_OnPressed;
    }

    private void OnDisable()
    {
        ScissorsSquadButton.OnPressed -= ScissorsSquadButton_OnPressed;
    }

    private void Start()
    {
        SetCurrentNode();
    }

    void ScissorsSquadButton_OnPressed()
    {
        GameObject clone = Instantiate(ScissorsSquad);
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
