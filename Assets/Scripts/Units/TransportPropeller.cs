using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransportPropeller : MonoBehaviour
{
    EnemyTransport myEnemyTransport;
    bool isRotating = true;
    const float rotationSpeed = -420f;
    float currentRotationSpeed = rotationSpeed;

    private void Awake()
    {
        myEnemyTransport = GetComponentInParent<EnemyTransport>();
        myEnemyTransport.OnEnemyTransportLanded += OnEnemyTransportLanded;
    }

    void OnEnemyTransportLanded(Node landingNode)
    {
        isRotating = false;
        StartCoroutine(SlowDownRotation());
    }

    void Update()
    {
        if (isRotating)
        {
            transform.Rotate(0f, 0f, Time.deltaTime * rotationSpeed, Space.Self);
        }
    }

    IEnumerator SlowDownRotation()
    {
        float timer = 0f;

        while (timer < 1f)
        {
            timer += Time.deltaTime;
            currentRotationSpeed = Mathf.Lerp(rotationSpeed, 0f, timer);
            transform.Rotate(0f, 0f, Time.deltaTime * currentRotationSpeed, Space.Self);
            yield return null;
        }
    }
}
