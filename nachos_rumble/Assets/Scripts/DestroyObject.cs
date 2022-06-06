using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class DestroyObject : MonoBehaviour
{
    PhotonView PV;
    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (!PV.IsMine)
            Destroy(gameObject);
    }
}