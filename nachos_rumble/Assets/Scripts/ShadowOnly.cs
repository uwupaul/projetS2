using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Rendering;

public class ShadowOnly : MonoBehaviour
{
    public PhotonView PV;
    void Start()
    {
        if (!PV.IsMine)
            return;

        var Renderer = GetComponent<Renderer>();
        Renderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
    }
}

