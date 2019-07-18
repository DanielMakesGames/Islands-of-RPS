using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySquad : Squad
{
    protected override void SpawnSquadUnits()
    {
        base.SpawnSquadUnits();

        for (int i = 0; i < squadUnits.Count; ++i)
        {
            squadUnits[i].transform.parent = transform;
        }
    }
}
