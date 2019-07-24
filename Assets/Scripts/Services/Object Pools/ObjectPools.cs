using UnityEngine;
using System.Collections;

public class ObjectPools : MonoBehaviour 
{
    public static ObjectPools CurrentObjectPool;

    [SerializeField] ObjectPooler audioSourcePool = null;
    public ObjectPooler AudioSourcePool
    {
        get { return audioSourcePool; }
    }

    [SerializeField] ObjectPooler rockProjectilePool = null;
    public ObjectPooler RockProjectilePool
    {
        get { return rockProjectilePool; }
    }

    [SerializeField] ObjectPooler enemyBloodPool = null;
    public ObjectPooler EnemyBloodPool
    {
        get { return enemyBloodPool; }
    }

    [SerializeField] ObjectPooler playerBloodPool = null;
    public ObjectPooler PlayerBloodPool
    {
        get { return playerBloodPool; }
    }

    [SerializeField] ObjectPooler rockSmokePool = null;
    public ObjectPooler RockSmokePool
    {
        get { return rockSmokePool; }
    }

    void Awake()
    {
        if (CurrentObjectPool == null)
        {
            CurrentObjectPool = this;
        }
    }
}
