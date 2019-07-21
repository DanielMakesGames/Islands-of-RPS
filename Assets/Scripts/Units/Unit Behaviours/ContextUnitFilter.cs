using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ContextUnitFilter : ScriptableObject
{
    public abstract List<Transform> Filter(SquadUnit agent, List<Transform> original);
}