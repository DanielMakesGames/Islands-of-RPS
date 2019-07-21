using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Squad Unit/Filter/LayerMask")]
public class LayerMaskFilter : ContextUnitFilter
{
    public LayerMask mask;

    public override List<Transform> Filter(SquadUnit agent, List<Transform> original)
    {
        List<Transform> filtered = new List<Transform>();

        for (int i = 0; i < original.Count; ++i)
        {
            if (mask == (mask | 1 << original[i].gameObject.layer))
            {
                filtered.Add(original[i]);
            }
        }

        return filtered;
    }
}

