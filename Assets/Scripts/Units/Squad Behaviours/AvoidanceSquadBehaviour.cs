using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Squad/Behaviour/Avoidance")]
public class AvoidanceSquadBehaviour : FilteredSquadBehaviour
{
    public override Vector3 CalculateMove(Squad squad, List<Transform> context, EnemySquadManager squadManager)
    {
        List<Transform> filteredContext = (filter == null) ? context : filter.Filter(squad, context);

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
            if (Vector3.SqrMagnitude(filteredContext[i].position - squad.transform.position) < squadManager.SquareAvoidanceRadius)
            {
                ++nAvoid;
                avoidanceMove += (squad.transform.position - filteredContext[i].position);
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
