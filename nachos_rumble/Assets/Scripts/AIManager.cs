using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class AIManager : MonoBehaviour
{
    private PhotonView PV;
    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (!PhotonNetwork.LocalPlayer.IsMasterClient)
            return;
        
        int n = (int)PhotonNetwork.CurrentRoom.CustomProperties["AI"];
        
        for (int i = 0; i < n; i++)
            SpawnAI();
    }

    public void SpawnAI()
    {
        Transform spawnpoint = SpawnManager.Instance.GetSpawnPoint();
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs","AIController"),spawnpoint.position,spawnpoint.rotation, 0, new object[] { PV.ViewID });
    }

    public void Die(AIController ai)
    {
        PhotonNetwork.Destroy(ai.gameObject);
        SpawnAI();
    }
}
