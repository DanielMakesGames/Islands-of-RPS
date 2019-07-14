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
        Squad.OnSquadSelected += OnSquadSelected;
        Squad.OnSquadDeselected += OnSquadDeselected;
    }

    private void OnDisable()
    {
        Squad.OnSquadSelected -= OnSquadSelected;
        Squad.OnSquadDeselected -= OnSquadDeselected;
    }

    void OnSquadSelected(Squad squad)
    {
        mySpriteRenderer.enabled = true;
    }

    void OnSquadDeselected(Squad squad)
    {
        mySpriteRenderer.enabled = false;
    }
}
