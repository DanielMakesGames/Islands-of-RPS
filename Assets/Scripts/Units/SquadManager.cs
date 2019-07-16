using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadManager : MonoBehaviour
{
    public delegate void SquadManagerAction();
    public static event SquadManagerAction OnEnterStrategyMode;
    public static event SquadManagerAction OnExitStrategyMode;

    Squad activeSquad = null;

    private void OnEnable()
    {
        Squad.OnSquadSelected += OnSquadSelected;
        Squad.OnSquadDeselected += OnSquadDeselected;
    }

    private void OnDisable()
    {
        Squad.OnSquadSelected -= OnSquadSelected;
        Squad.OnSquadDeselected -= OnSquadSelected;
    }

    void OnSquadSelected(Squad squad)
    {
        if (activeSquad == null)
        {
            activeSquad = squad;
            OnEnterStrategyMode?.Invoke();
        }
        else
        {
            activeSquad = squad;
        }
    }

    void OnSquadDeselected(Squad squad)
    {
        if (activeSquad == squad)
        {
            activeSquad = null;
            OnExitStrategyMode?.Invoke();
        }
    }
}
