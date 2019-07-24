using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBloodSplatter : MonoBehaviour
{
    SquadUnit mySquadUnit;

    private void Awake()
    {
        mySquadUnit = GetComponentInParent<SquadUnit>();
        mySquadUnit.OnAnimateReceiveDamage += OnAnimateReceiveDamage;
    }

    void OnAnimateReceiveDamage()
    {
        GameObject blood = ObjectPools.CurrentObjectPool.EnemyBloodPool.GetPooledObject();
        if (blood != null)
        {
            blood.transform.position = transform.position + Vector3.up * 1.5f;
            blood.SetActive(true);
        }
    }
}
