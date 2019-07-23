using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTransportSmoke : MonoBehaviour
{
    EnemyTransport myEnemyTransport;
    ParticleSystem myParticleSystem;

    private void Awake()
    {
        myEnemyTransport = GetComponentInParent<EnemyTransport>();
        myParticleSystem = GetComponent<ParticleSystem>();

        myEnemyTransport.OnEnemyTransportLanded += OnEnemyTransportLanded;
    }

    void OnEnemyTransportLanded(Node landingNode)
    {
        myParticleSystem.Stop();

        myEnemyTransport.OnEnemyTransportLanded -= OnEnemyTransportLanded;
    }
}
