using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadUnitAnimator : MonoBehaviour
{
    SquadUnit mySquadUnit;
    Animator myAnimator;

    Vector3 forward;
    const float speed = 4f;
    bool isDying = false;

    private void Awake()
    {
        mySquadUnit = GetComponentInParent<SquadUnit>();
        myAnimator = GetComponent<Animator>();

        mySquadUnit.OnAnimateIdle += MySquadUnit_OnAnimateIdle;
        mySquadUnit.OnAnimateMovement += MySquadUnit_OnAnimateMovement;
        mySquadUnit.OnAnimateAttack += MySquadUnit_OnAnimateAttack;
        mySquadUnit.OnAnimateDeath += MySquadUnit_OnAnimateDeath;
    }

    private void Start()
    {
        forward = transform.parent.forward;
        transform.forward = forward;
    }

    private void LateUpdate()
    {
        if (!isDying)
        {
            forward = Vector3.RotateTowards(forward, transform.parent.forward, Time.deltaTime * speed, 0f);
            transform.forward = forward;
        }
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
        forward = transform.parent.forward;
        transform.forward = forward;
        myAnimator.SetTrigger("Attack");
    }

    void MySquadUnit_OnAnimateDeath()
    {
        isDying = true;
        myAnimator.SetTrigger("Death");
    }

}
