using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudManager : MonoBehaviour
{
    ParticleSystem[] myParticleSystems;

    private void Awake()
    {
        myParticleSystems = GetComponentsInChildren<ParticleSystem>();

        StopAllParticleSystems();
    }

    private void OnEnable()
    {
        PlayButton.OnPressed += PlayAllParticleSystems;
        ReturnToTitleButton.OnPressed += StopAllParticleSystems;
    }

    private void OnDisable()
    {
        PlayButton.OnPressed -= PlayAllParticleSystems;
        ReturnToTitleButton.OnPressed -= StopAllParticleSystems;
    }

    void PlayAllParticleSystems()
    {
        for (int i = 0; i < myParticleSystems.Length; ++i)
        {
            myParticleSystems[i].Play();
        }
    }

    void StopAllParticleSystems()
    {
        for (int i = 0; i < myParticleSystems.Length; ++i)
        {
            myParticleSystems[i].Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
}
