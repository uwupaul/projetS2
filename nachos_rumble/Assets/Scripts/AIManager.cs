using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using UnityEngine.AI;

public class AIManager : MonoBehaviour
{
    PhotonView PV;
    GameObject controller;

    private void Awake()
    {
        PV = GetComponent<PhotonView>(); // pour identifier l'IA
    }
    
    void Start()
    {
        if (PV.IsMine)
            CreateController(); 
        
    }

    void CreateController()
    {
        Transform spawnpoint = SpawnManager.Instance.GetSpawnPoint();
        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs","AIController"),spawnpoint.position,spawnpoint.rotation, 0, new object[] { PV.ViewID });
        
        //On va instantier le prefab 'AIController' qui se trouve dans le dossier 'PhotonPrefabs' à la position
        //et à la même rotation qu'un SpawnPoint choisi aléatoirement par 'GetSpawnPoint'
    }

    public void Die()
    {
        PhotonNetwork.Destroy(controller);
        CreateController();
    }
}
