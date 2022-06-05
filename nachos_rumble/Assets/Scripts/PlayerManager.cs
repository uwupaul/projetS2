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
    
    public AudioSource SpawnSound;
    public AudioSource DeathSound;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
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
        
        AudioManager.Instance.SendSound(3, SpawnSound.minDistance, SpawnSound.maxDistance, spawnpoint.position);
    }

    public void Die()
    {
        Vector3 position = controller.GetComponent<CharacterController>().transform.position;
        
        AudioManager.Instance.SendSound(4, DeathSound.minDistance, DeathSound.maxDistance, position);
        PhotonNetwork.Destroy(controller);
        CreateController();
        ApplyDeath();
    }

    private void ApplyDeath()
    {
        Hashtable H = new Hashtable();
        int deathOfParent = Convert.ToInt32(PV.Owner.CustomProperties["D"]);
        
        H.Add("D", deathOfParent + 1);
        PV.Owner.SetCustomProperties(H);
        
        PlayerData.Instance.globalDeaths++;
    }
}
