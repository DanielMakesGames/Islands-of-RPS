using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ContextSquadFilter : ScriptableObject
{
    public abstract List<Transform> Filter(Squad agent, List<Transform> original);
}
