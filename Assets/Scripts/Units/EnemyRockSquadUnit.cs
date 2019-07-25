using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRockSquadUnit : RockSquadUnit
{
    protected override void Start()
    {
    }

    protected override IEnumerator AttackAnimation(TownCenter targetSquadUnit)
    {
        myNavMeshAgent.enabled = false;

        GameObject clone = ObjectPools.CurrentObjectPool.RockProjectilePool.GetPooledObject();
        if (clone != null)
        {
            Vector3 start = transform.position + Vector3.up;
            Vector3 end = targetSquadUnit.transform.position;
            clone.SetActive(true);

            RockProjectile projectile = clone.GetComponent<RockProjectile>();
            projectile.SetDestination(start, targetSquadUnit.transform,
                attackDamage, damageType);

            start.y = 0f;
            end.y = 0f;

            Vector3 direction = end - start;
            direction.Normalize();
            transform.forward = direction;
        }

        AnimateAttack();
        yield return new WaitForSeconds(attackTime);
        myNavMeshAgent.enabled = true;
    }
}
