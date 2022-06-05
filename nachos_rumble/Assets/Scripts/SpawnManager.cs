    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;
    public SpawnPoint[] spawnpoints;
    public float securityDistance;

    private void Awake()
    {
        Instance = this;
        spawnpoints = GetComponentsInChildren<SpawnPoint>();
    }

    public Transform GetSpawnPoint()
    {
        Transform t;
        int index;
        do
        {
            index = Random.Range(0, spawnpoints.Length);
            t = spawnpoints[index].transform;
        } while (!IsSpawnValid(t));
        
        Debug.Log($"Spawned at spawn index {index}");
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
