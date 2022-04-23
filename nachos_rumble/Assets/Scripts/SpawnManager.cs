    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    private SpawnPoint[] spawnpoints;

    private void Awake()
    {
        Instance = this;
        spawnpoints = GetComponentsInChildren<SpawnPoint>();
    }

    public Transform GetSpawnPoint()
    {
        return spawnpoints[Random.Range(0, spawnpoints.Length)].transform;
    }
}
