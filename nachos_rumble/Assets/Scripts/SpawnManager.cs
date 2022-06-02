    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;
    private SpawnPoint[] spawnpoints;
    public float securityDistance = 3f;

    private void Awake()
    {
        Instance = this;
        spawnpoints = GetComponentsInChildren<SpawnPoint>();
    }

    public Transform GetSpawnPoint()
    {
        Transform t;
        do {
            t = spawnpoints[Random.Range(0, spawnpoints.Length)].transform;
        } while (!IsSpawnValid(t));

        return t;
    }

    bool IsSpawnValid(Transform t)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players)
        {
            float playerDistance = Vector3.Distance(t.position, p.transform.position);
            // distance entre le transform 't' du spawn et la position du player
            if (playerDistance < securityDistance)
                return false;
        }
        
        return true;
    }
}
