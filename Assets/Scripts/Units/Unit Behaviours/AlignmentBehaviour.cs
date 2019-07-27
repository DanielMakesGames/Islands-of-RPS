using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Squad Unit/Behaviour/Alignment")]
public class AlignmentBehaviour : FilteredUnitBehaviour
{
    public override Vector3 CalculateMove(SquadUnit squadUnit, List<Transform> context, Squad squad)
    {
        List<Transform> filteredContext = (filter == null) ? context : filter.Filter(squadUnit, context);

        //if no neighbors, maintain current direction
        if (filteredContext.Count == 0)
        {
            return squadUnit.PathTransform.forward;
        }

        //add all forwards together and average
        Vector3 alignmentMove = Vector3.zero;
        for (int i = 0; i < filteredContext.Count; ++i)
        {
            alignmentMove += filteredContext[i].forward;
        }
        alignmentMove /= filteredContext.Count;
        alignmentMove.y = 0f;

        return alignmentMove;
    }
}