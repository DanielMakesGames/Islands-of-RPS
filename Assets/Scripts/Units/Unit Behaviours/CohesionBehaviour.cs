using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Squad Unit/Behaviour/Cohesion")]
public class CohesionBehaviour : FilteredUnitBehaviour
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
        Vector3 cohesionMove = Vector3.zero;
        for (int i = 0; i < filteredContext.Count; ++i)
        {
            cohesionMove += filteredContext[i].position;
        }
        cohesionMove /= filteredContext.Count;

        //create offset from agent position
        cohesionMove -= squadUnit.transform.position;
        cohesionMove.y = 0f;

        return cohesionMove;
    }
}

