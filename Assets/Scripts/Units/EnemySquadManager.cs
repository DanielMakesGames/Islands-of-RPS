using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySquadManager : SquadManager
{
    [SerializeField] EnemyWave[] EnemyWaves = null;

    int currentEnemyWaveIndex = 0;

    private void Start()
    {
        if (currentEnemyWaveIndex < EnemyWaves.Length)
        {
            InstantiateEnemyWave(currentEnemyWaveIndex);
        }
    }

    void InstantiateEnemyWave(int enemyWaveIndex)
    {
        for (int i = 0; i < EnemyWaves[enemyWaveIndex].EnemyBoats.Length; ++i)
        {
            StartCoroutine(InstantiateEnemyBoat(
                EnemyWaves[enemyWaveIndex].EnemyBoats[i],
                EnemyWaves[enemyWaveIndex].SpawnLocations[i],
                EnemyWaves[enemyWaveIndex].Directions[i],
                EnemyWaves[enemyWaveIndex].SpawnTimes[i]));
        }
    }

    IEnumerator InstantiateEnemyBoat(GameObject enemyBoat, Vector3 spawnLocation,
        Vector3 direction, float spawnTime)
    {
        yield return new WaitForSeconds(spawnTime);

        GameObject clone = Instantiate(enemyBoat);
        clone.transform.position = spawnLocation;
        clone.transform.forward = direction;

        Squad[] squads = clone.GetComponentsInChildren<Squad>();
        for (int i = 0; i < squads.Length; ++i)
        {
            mySquads.Add(squads[i]);
            squads[i].SetSquadManager(this);
        }
    }

    // when Squad lands on island, use nav mesh link to hop on the island
    // Squad has independent AI to to attack player
}
