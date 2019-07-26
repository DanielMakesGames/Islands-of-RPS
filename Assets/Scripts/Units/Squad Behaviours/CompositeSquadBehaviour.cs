using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Squad/Behaviour/Composite")]
public class CompositeSquadBehaviour : SquadBehaviour
{
    public SquadBehaviour[] Behaviours;
    public float[] Weights;

    public override Vector3 CalculateMove(Squad squad, List<Transform> context, EnemySquadManager squadManager)
    {
        if (Weights.Length != Behaviours.Length)
        {
            Debug.LogError("Data mismatch in " + name, this);
            return Vector3.zero;
        }

        Vector3 move = Vector3.zero;
        float currentWeight = 0f;

        for (int i = 0; i < Behaviours.Length; ++i)
        {
            Vector3 partialMove = Behaviours[i].CalculateMove(squad, context, squadManager);
            if (partialMove != Vector3.zero)
            {
                if (Weights[i] > currentWeight)
                {
                    move = partialMove;
                    currentWeight = Weights[i];
                }
                else if (Mathf.Abs(Weights[i] - currentWeight) < Mathf.Epsilon)
                {
                    if (partialMove.sqrMagnitude < move.sqrMagnitude)
                    {
                        move = partialMove;
                    }
                }
            }
           
        }

        return move;
    }
}
