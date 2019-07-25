using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Squad/Filter/Squad Filter")]
public class OpponentTypeSquadFilter : ContextSquadFilter
{
    public LayerMask OpponentFilter;
    public Squad.SquadType RPSFilter;

    public override List<Transform> Filter(Squad agent, List<Transform> original)
    {
        List<Transform> filtered = new List<Transform>();

        for (int i = 0; i < original.Count; ++i)
        {
            if (OpponentFilter == (OpponentFilter | 1 << original[i].gameObject.layer))
            {
                if (original[i].GetComponent<Squad>().RPSType == RPSFilter)
                {
                    filtered.Add(original[i]);
                }
            }
        }

        return filtered;
    }
}
