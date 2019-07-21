using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Squad Unit/Filter/Same Squad")]
public class SameSquadFilter : ContextUnitFilter
{
    public override List<Transform> Filter(SquadUnit squadUnit, List<Transform> original)
    {
        List<Transform> filtered = new List<Transform>();

        for (int i = 0; i < original.Count; ++i)
        {
            SquadUnit itemSquadUnit = original[i].GetComponent<SquadUnit>();
            if (itemSquadUnit != null && itemSquadUnit.CommandingSquad == squadUnit.CommandingSquad)
            {
                filtered.Add(original[i]);
            }
        }

        return filtered;
    }
}
