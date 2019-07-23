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

    void Awake()
    {
        if (CurrentObjectPool == null)
        {
            CurrentObjectPool = this;
        }
    }
}
