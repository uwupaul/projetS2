using System;
using Photon.Pun;
using UnityEngine;

public class DestroyObject : MonoBehaviour
{
    PhotonView PV;
    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (PV.IsMine)
            Destroy(gameObject);
    }
}
