using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Squad Unit/Behaviour/Follow Path")]
public class FollowPathBehaviour : UnitBehaviour
{
    public override Vector3 CalculateMove(SquadUnit squadUnit, List<Transform> context, Squad squad)
    {
        Vector3 followPathVector = Vector3.zero;

        followPathVector = squadUnit.TargetTransform.position - squadUnit.PathTransform.position;

        return followPathVector;
    }
}
