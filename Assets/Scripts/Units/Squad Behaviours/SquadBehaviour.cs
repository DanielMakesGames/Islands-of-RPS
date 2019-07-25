using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SquadBehaviour : ScriptableObject
{
    public abstract Vector3 CalculateMove(Squad squad,
        List<Transform> context, EnemySquadManager squadManager);
}
