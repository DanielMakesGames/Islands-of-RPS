using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Squad Unit/Behaviour/Composite")]
public class CompositeUnitBehaviour : UnitBehaviour
{
    public UnitBehaviour[] Behaviours;
    public float[] Weights;

    public override Vector3 CalculateMove(SquadUnit squadUnit, List<Transform> context, Squad squad)
    {
        if (Weights.Length != Behaviours.Length)
        {
            Debug.LogError("Data mismatch in " + name, this);
            return Vector3.zero;
        }

        Vector3 move = Vector3.zero;

        for (int i = 0; i < Behaviours.Length; ++i)
        {
            Vector3 partialMove = Behaviours[i].CalculateMove(squadUnit, context, squad) * Weights[i];

            if (partialMove != Vector3.zero)
            {
                if (partialMove.sqrMagnitude > Weights[i] * Weights[i])
                {
                    partialMove = partialMove.normalized * Weights[i];
                }

                move += partialMove;
            }
        }

        return move;
    }
}
