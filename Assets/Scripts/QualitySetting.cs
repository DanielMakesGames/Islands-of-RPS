using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QualitySetting : MonoBehaviour
{
    static QualitySetting Singleton;

    private void Awake()
    {
        if (Singleton != null)
        {
            Destroy(gameObject);
            return;
        }

        Singleton = this;
        if (transform.parent == null)
        {
            DontDestroyOnLoad(gameObject);
        }

        Application.targetFrameRate = 60;
    }
}
