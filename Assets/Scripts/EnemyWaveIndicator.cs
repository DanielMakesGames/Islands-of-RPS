﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWaveIndicator : MonoBehaviour
{
    [SerializeField] GameObject RockSilhouette = null;
    [SerializeField] GameObject PaperSilhouette = null;
    [SerializeField] GameObject ScissorSilhouette = null;

    List<GameObject> icons;

    const float xSpacing = 160f;
    const float ySpacing = -160f;
    const int columns = 6;

    enum EnemyIcon
    {
        Rock,
        Paper,
        Scissor
    }

    private void Awake()
    {
        icons = new List<GameObject>();
    }

    private void OnEnable()
    {
        GameplayManager.OnIslandLoaded += DisplayEnemyWaves;
        GameplayManager.OnStartGameplay += DisableDisplay;
    }

    private void OnDisable()
    {
        GameplayManager.OnIslandLoaded -= DisplayEnemyWaves;
        GameplayManager.OnStartGameplay -= DisableDisplay;
    }

    void DisplayEnemyWaves()
    {
        EnemySquadManager enemySquadManager = FindObjectOfType<EnemySquadManager>();

        for (int i = 0; i < enemySquadManager.CurrentEnemyWave.EnemyBoats.Length; ++i)
        {
            if (enemySquadManager.CurrentEnemyWave.EnemyBoats[i].name.Contains("Rock"))
            {
                StartCoroutine(InstantiateEnemyIcon(EnemyIcon.Rock, i,
                    enemySquadManager.CurrentEnemyWave.EnemyBoats.Length, i * 0.25f));
            }
            else if (enemySquadManager.CurrentEnemyWave.EnemyBoats[i].name.Contains("Paper"))
            {
                StartCoroutine(InstantiateEnemyIcon(EnemyIcon.Paper, i,
                    enemySquadManager.CurrentEnemyWave.EnemyBoats.Length, i * 0.25f));
            }
            else if (enemySquadManager.CurrentEnemyWave.EnemyBoats[i].name.Contains("Scissor"))
            {
                StartCoroutine(InstantiateEnemyIcon(EnemyIcon.Scissor, i,
                    enemySquadManager.CurrentEnemyWave.EnemyBoats.Length, i * 0.25f));
            }
            else
            {

            }
        }
    }

    IEnumerator InstantiateEnemyIcon(EnemyIcon enemyBoat, int index, int totalSquads, float spawnTime)
    {
        yield return new WaitForSeconds(spawnTime);

        GameObject clone = null;
        switch (enemyBoat)
        {
            case EnemyIcon.Rock:
                clone = Instantiate(RockSilhouette);
                break;
            case EnemyIcon.Paper:
                clone = Instantiate(PaperSilhouette);
                break;
            case EnemyIcon.Scissor:
                clone = Instantiate(ScissorSilhouette);
                break;
            default:
                clone = Instantiate(RockSilhouette);
                break;
        }


        int currentRow = Mathf.FloorToInt((float)index / (float)columns);
        int totalRows = Mathf.CeilToInt((float)totalSquads / (float)columns);
        float startPositionX = 0f;
        if (currentRow < totalRows - 1)
        {
            totalSquads = columns;
        }
        else
        {
            totalSquads = totalSquads % columns;
            if (totalSquads == 0)
            {
                totalSquads = columns;
            }
        }

        index = index % columns;
        startPositionX = (totalSquads - 1) * xSpacing / -2f;

        clone.transform.SetParent(transform, false);
        clone.transform.localPosition = new Vector3(
            startPositionX + index * xSpacing,
            ySpacing * (currentRow + 1f), 0f);
        clone.transform.localRotation = Quaternion.identity;
        clone.transform.localScale = Vector3.one * 30f;

        icons.Add(clone);
    }

    void DisableDisplay()
    {
        for (int i = 0; i < icons.Count; ++i)
        {
            Destroy(icons[i]);
        }
        icons.Clear();
        gameObject.SetActive(false);
    }
}
