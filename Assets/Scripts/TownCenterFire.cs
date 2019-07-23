using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownCenterFire : MonoBehaviour
{
    TownCenter myTownCenter;
    ParticleSystem myParticleSystem;

    bool isPlaying = false;

    void Awake()
    {
        myTownCenter = GetComponentInParent<TownCenter>();
        myParticleSystem = GetComponent<ParticleSystem>();
    }

    void Update()
    {

        if (!isPlaying && myTownCenter.Heatlh < 50f)
        {
            isPlaying = true;
            PlayFire();
        }
    }

    void PlayFire()
    {
        myParticleSystem.Play();
    }
}
