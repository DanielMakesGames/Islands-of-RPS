using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBloodSplatter : MonoBehaviour
{
    SquadUnit mySquadUnit;

    private void Awake()
    {
        mySquadUnit = GetComponentInParent<SquadUnit>();
        mySquadUnit.OnAnimateReceiveDamage += OnAnimateReceiveDamage;
    }

    void OnAnimateReceiveDamage()
    {
        GameObject blood = ObjectPools.CurrentObjectPool.PlayerBloodPool.GetPooledObject();
        if (blood != null)
        {
            blood.transform.position = transform.position + Vector3.up * 1.5f;
            blood.SetActive(true);
        }
    }
}
