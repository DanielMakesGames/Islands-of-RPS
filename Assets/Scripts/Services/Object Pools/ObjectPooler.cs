using UnityEngine;
using System.Collections;

public class ObjectPooler : MonoBehaviour 
{
    [SerializeField] GameObject[] pooledObjects = null;
    public GameObject[] PooledObjects
    {
        get { return pooledObjects; }
    }
    int currentIterator = 0;
    
	void Awake() 
    {
        for (int i = 0; i < pooledObjects.Length; ++i)
        {
            pooledObjects[i].SetActive(false);
        }
	}

    public GameObject GetPooledObject() 
    {
        for (int i = 0; i < pooledObjects.Length; ++i)
        {
            if (!pooledObjects[currentIterator].activeInHierarchy)
            {
                int index = currentIterator;
                IncreaseIterator();
                return pooledObjects[index];
            }
            IncreaseIterator();

        }
        // print ("Out of pooled object: " + gameObject.name);
        return null;
	}

    void IncreaseIterator()
    {
        ++currentIterator;
        if (currentIterator >= pooledObjects.Length)
        {
            currentIterator = 0;
        }
    }
}
