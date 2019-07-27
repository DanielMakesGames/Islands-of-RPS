using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Squad Unit/Behaviour/Avoidance")]
public class AvoidanceBehaviour : FilteredUnitBehaviour
{
    public override Vector3 CalculateMove(SquadUnit squadUnit, List<Transform> context, Squad squad)
    {
        List<Transform> filteredContext = (filter == null) ? context : filter.Filter(squadUnit, context);

        //if no neighbors, return no adjustment
        if (filteredContext.Count == 0)
        {
            return Vector3.zero;
        }

        //add all points together and average
        Vector3 avoidanceMove = Vector3.zero;
        int nAvoid = 0;
        for (int i = 0; i < filteredContext.Count; ++i)
        {
            if (Vector3.SqrMagnitude(filteredContext[i].position - squadUnit.PathTransform.position) < squad.SquareAvoidanceRadius)
            {
                ++nAvoid;
                avoidanceMove += (squadUnit.PathTransform.position - filteredContext[i].position);
            }
        }
        if (nAvoid > 0)
        {
            avoidanceMove /= nAvoid;
        }
        avoidanceMove.y = 0f;

        return avoidanceMove;
    }
}