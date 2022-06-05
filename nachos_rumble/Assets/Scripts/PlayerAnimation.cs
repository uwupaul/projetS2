using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator Animator;
    private CharacterController CharacterController;
    private PhotonView PV;
    
    private int zVelHash;
    private int xVelHash;
    
    private void Start()
    {
        PV = GetComponent<PhotonView>();
        
        if (!PV.IsMine)
            return;
        
        Animator = GetComponent<Animator>();
        CharacterController = GetComponent<CharacterController>();

        zVelHash = Animator.StringToHash("Z_Velocity");
        xVelHash= Animator.StringToHash("X_Velocity");
    }

    private void Update()
    {
        if (!PV.IsMine)
            return;
        
        var v = transform.InverseTransformDirection(CharacterController.velocity);
        //Debug.Log($"CC Z velocity : {v.z}, X : {v.x}");
            
        Animator.SetFloat(zVelHash, v.z);
        Animator.SetFloat(xVelHash, v.x);
    }
}