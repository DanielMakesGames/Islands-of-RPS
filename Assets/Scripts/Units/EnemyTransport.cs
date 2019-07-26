using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTransport : MonoBehaviour
{
    public delegate void EnemyTransportAction(Node landingNode);
    public event EnemyTransportAction OnEnemyTransportLanded;

    const float movementSpeed = 5f;
    const float rayDistance = 200f;
    const float nodeScale = 10f;
    LayerMask nodeLayerMask;

    Node landingNode;

    private void Awake()
    {
        nodeLayerMask = LayerMask.GetMask("Node");
    }

    private void OnEnable()
    {
        ReturnToTitleButton.OnPressed += ReturnToTitleButtonOnButtonPress;
    }

    private void OnDisable()
    {
        ReturnToTitleButton.OnPressed -= ReturnToTitleButtonOnButtonPress;
    }

    private void Start()
    {
        if (FindDestinationNode())
        {
            StartCoroutine(TravelToLandingNode());
        }
    }

    bool FindDestinationNode()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit raycastHit,
                rayDistance, nodeLayerMask, QueryTriggerInteraction.Ignore))
        {
            landingNode = raycastHit.transform.GetComponent<Node>();
            return true;
        }
        else
        {
            Debug.Log("Transport unable to find island: " + gameObject.name);
            return false;
        }
    }

    IEnumerator TravelToLandingNode()
    {
        Vector3 destinationPosition = landingNode.transform.position - transform.forward * nodeScale;

        while (transform.position != destinationPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position,
                destinationPosition, Time.deltaTime * movementSpeed);

            yield return null;
        }

        OnEnemyTransportLanded?.Invoke(landingNode);
    }

    void ReturnToTitleButtonOnButtonPress()
    {
        Destroy(gameObject);
    }
}
