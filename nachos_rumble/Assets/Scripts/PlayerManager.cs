using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PlayerManager : MonoBehaviour
{
    // Start is called before the first frame update
    PhotonView PV;
    GameObject controller;

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

    // Update is called once per frame
    void CreateController()
    {
        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs","PlayerController"),Vector3.zero,Quaternion.identity, 0, new object[] { PV.ViewID });
    }

    public void Die()
    {
        PhotonNetwork.Destroy(controller);
        CreateController();
    }
}
