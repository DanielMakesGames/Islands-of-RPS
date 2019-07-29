using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeIndicator : MonoBehaviour
{
    SpriteRenderer mySpriteRenderer;
    Node myNode;

    private void Awake()
    {
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        myNode = GetComponentInParent<Node>();
    }

    private void OnEnable()
    {
        PlayerSquadManager.OnEnterStrategyMode += OnEnterStrategyMode;
        PlayerSquadManager.OnExitStrategyMode += OnExitStrategyMode;
    }

    private void OnDisable()
    {
        PlayerSquadManager.OnEnterStrategyMode -= OnEnterStrategyMode;
        PlayerSquadManager.OnExitStrategyMode -= OnExitStrategyMode;
    }

    void OnEnterStrategyMode()
    {
        mySpriteRenderer.enabled = myNode.IsWalkable;
    }

    void OnExitStrategyMode()
    {
        mySpriteRenderer.enabled = false;
    }
}
