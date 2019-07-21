using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Squad Unit/Behaviour/Follow Path")]
public class FollowPathBehaviour : UnitBehaviour
{
    public override Vector3 CalculateMove(SquadUnit squadUnit, List<Transform> context, Squad squad)
    {
        Vector3 followPathVector = Vector3.zero;

        // find if squad unit has a path
        if (squadUnit.NavMeshAgent.hasPath)
        {
            if (squadUnit.NavMeshAgent.path.corners.Length > 1)
            {
                followPathVector = squadUnit.NavMeshAgent.path.corners[1] - squadUnit.transform.position;
            }
        }

        return followPathVector;
    }
}
