﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSquadManager : SquadManager
{
    public delegate void SquadManagerAction();
    public static event SquadManagerAction OnEnterStrategyMode;
    public static event SquadManagerAction OnExitStrategyMode;

    Squad activeSquad = null;

    private void OnEnable()
    {
        TownCenter.OnSpawnNewSquad += OnSpawnNewPlayerSquad;

        PlayerSquad.OnSquadSelected += OnSquadSelected;
        PlayerSquad.OnSquadDeselected += OnSquadDeselected;
    }

    private void OnDisable()
    {
        TownCenter.OnSpawnNewSquad -= OnSpawnNewPlayerSquad;

        PlayerSquad.OnSquadSelected -= OnSquadSelected;
        PlayerSquad.OnSquadDeselected -= OnSquadDeselected;
    }

    void OnSpawnNewPlayerSquad(Squad newSquad)
    {
        mySquads.Add(newSquad);
        newSquad.SetSquadManager(this);
    }

    void OnSquadSelected(Squad squad)
    {
        if (mySquads.Contains(squad))
        {
            if (activeSquad == null)
            {
                activeSquad = squad;
                OnEnterStrategyMode?.Invoke();
            }
            else
            {
                StopAllCoroutines();
                activeSquad = squad;
            }
        }
    }

    void OnSquadDeselected(Squad squad)
    {
        if (mySquads.Contains(squad))
        {
            if (activeSquad == squad)
            {
                StartCoroutine(ExitStrategyMode());
            }
        }
    }

    IEnumerator ExitStrategyMode()
    {
        yield return null;
        activeSquad = null;
        OnExitStrategyMode?.Invoke();
    }
}
