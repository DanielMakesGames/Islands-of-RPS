using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadUnitAnimator : MonoBehaviour
{
    SquadUnit mySquadUnit;
    Animator myAnimator;

    private void Awake()
    {
        mySquadUnit = GetComponentInParent<SquadUnit>();
        myAnimator = GetComponent<Animator>();

        mySquadUnit.OnAnimateIdle += MySquadUnit_OnAnimateIdle;
        mySquadUnit.OnAnimateMovement += MySquadUnit_OnAnimateMovement;
        mySquadUnit.OnAnimateAttack += MySquadUnit_OnAnimateAttack;
        mySquadUnit.OnAnimateDeath += MySquadUnit_OnAnimateDeath;
    }

    void MySquadUnit_OnAnimateIdle()
    {
        myAnimator.SetTrigger("Idle");
    }

    void MySquadUnit_OnAnimateMovement()
    {
        myAnimator.SetTrigger("Movement");
    }

    void MySquadUnit_OnAnimateAttack()
    {
        myAnimator.SetTrigger("Attack");
    }

    void MySquadUnit_OnAnimateDeath()
    {
        myAnimator.SetTrigger("Death");
    }

}
