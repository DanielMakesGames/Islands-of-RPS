using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Squad/Filter/LayerMask")]
public class LayerMaskSquadFilter : ContextSquadFilter
{
    public LayerMask mask;

    public override List<Transform> Filter(Squad agent, List<Transform> original)
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
