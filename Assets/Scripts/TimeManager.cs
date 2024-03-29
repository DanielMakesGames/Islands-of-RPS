﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    const float animationSpeed = 4f;

    private void OnEnable()
    {
        PlayerSquadManager.OnEnterStrategyMode += OnEnterStrategyMode;
        PlayerSquadManager.OnExitStrategyMode += OnExitStrategyMode;

        TownCenter.OnTownCenterDestroyed += NormalTime;
        EnemySquadManager.OnAllEnemiesDefeated += NormalTime;
    }

    private void OnDisable()
    {
        PlayerSquadManager.OnEnterStrategyMode -= OnEnterStrategyMode;
        PlayerSquadManager.OnExitStrategyMode -= OnExitStrategyMode;

        TownCenter.OnTownCenterDestroyed -= NormalTime;
        EnemySquadManager.OnAllEnemiesDefeated -= NormalTime;
    }

    void OnEnterStrategyMode()
    {
        SlowTime();
    }

    void OnExitStrategyMode()
    {
        NormalTime();
    }

    void SlowTime()
    {
        StopAllCoroutines();
        StartCoroutine(TransitionTimeAnimation(0.05f));
    }

    void NormalTime()
    {
        StopAllCoroutines();
        StartCoroutine(TransitionTimeAnimation(1f));
    }

    IEnumerator TransitionTimeAnimation(float timeScaleDestination)
    {
        float timer = 0f;
        while (timer < 1f)
        {
            timer += Time.deltaTime * animationSpeed;
            Time.timeScale = Mathf.Lerp(Time.timeScale, timeScaleDestination, timer);
            yield return null;
        }
    }
}
