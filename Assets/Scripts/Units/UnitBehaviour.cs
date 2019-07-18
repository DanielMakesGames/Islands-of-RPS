using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitBehaviour : ScriptableObject
{
    public abstract Vector3 CalculateMove(SquadUnit squadUnit,
        List<Transform> context, SquadManager squadManager);
}
