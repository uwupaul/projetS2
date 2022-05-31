using System;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    PhotonView PV;
    GameObject controller;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        PV.Owner.CustomProperties["Kills"] = 0;
        PV.Owner.CustomProperties["Deaths"] = 0;
    }

    void Start()
    {
        if (PV.IsMine)
        {
            CreateController();
        }
    }

    void CreateController()
    {
        Transform spawnpoint = SpawnManager.Instance.GetSpawnPoint();
        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs","PlayerController"),spawnpoint.position,spawnpoint.rotation, 0, new object[] { PV.ViewID });
    }

    public void Die()
    {
        PhotonNetwork.Destroy(controller);
        CreateController();
        ApplyDeath();
    }

    private void ApplyDeath()
    {
        Hashtable H = new Hashtable();
        int deathOfParent = Convert.ToInt32(PV.Owner.CustomProperties["Death"]);
        
        H.Add("Death", deathOfParent + 1);
        PV.Owner.SetCustomProperties(H);
    }
}
