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
        mySpriteRenderer.enabled = true;
    }

    void OnExitStrategyMode()
    {
        mySpriteRenderer.enabled = false;
    }
}
