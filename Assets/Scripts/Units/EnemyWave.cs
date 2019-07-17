using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Wave", menuName = "Enemy Wave")]
public class EnemyWave : ScriptableObject
{
    public GameObject[] EnemyBoats;
    public Vector3[] SpawnLocations;
    public Vector3[] Directions;
    public float[] SpawnTimes;
}
