using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeIndicator : MonoBehaviour
{
    SpriteRenderer mySpriteRenderer;

    private void Awake()
    {
        mySpriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        SquadManager.OnEnterStrategyMode += OnEnterStrategyMode;
        SquadManager.OnExitStrategyMode += OnExitStrategyMode;
    }

    private void OnDisable()
    {
        SquadManager.OnEnterStrategyMode -= OnEnterStrategyMode;
        SquadManager.OnExitStrategyMode -= OnExitStrategyMode;
    }

    void OnEnterStrategyMode()
    {
        mySpriteRenderer.enabled = true;
    }

    void OnExitStrategyMode()
    {
        mySpriteRenderer.enabled = false;
    }
}
