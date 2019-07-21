using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Squad Unit/Behaviour/Attack Town Center")]
public class AttackTownCenterBehaviour : FilteredUnitBehaviour
{
    public override Vector3 CalculateMove(SquadUnit squadUnit, List<Transform> context, Squad squad)
    {
        List<Transform> filteredContext = (filter == null) ? context : filter.Filter(squadUnit, context);

        //if no neighbors, return no adjustment
        if (filteredContext.Count == 0)
        {
            return Vector3.zero;
        }

        //find closest unit to attack
        float closestSqrMagnitude = 100f;
        if (squad.SquareAttackRadius > closestSqrMagnitude)
        {
            closestSqrMagnitude = squad.SquareAttackRadius;
        }
        Transform closestTransform = null;
        for (int i = 0; i < filteredContext.Count; ++i)
        {
            float sqrMagnitude = Vector3.SqrMagnitude(filteredContext[i].position - squadUnit.transform.position);
            if (sqrMagnitude < closestSqrMagnitude)
            {
                closestSqrMagnitude = sqrMagnitude;
                closestTransform = filteredContext[i];
            }
        }

        if (closestTransform == null)
        {
            return Vector3.zero;
        }

        TownCenter targetTownCenter = closestTransform.GetComponent<TownCenter>();
        if (targetTownCenter != null)
        {
            squadUnit.Attack(targetTownCenter);
        }

        return Vector3.zero;
    }
}
