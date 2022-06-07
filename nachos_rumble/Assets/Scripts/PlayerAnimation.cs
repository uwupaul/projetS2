using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator Animator;
    private CharacterController CharacterController;
    private PlayerController PlayerController;
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
        PlayerController = GetComponent<PlayerController>();

        zVelHash = Animator.StringToHash("Z_Velocity");
        xVelHash= Animator.StringToHash("X_Velocity");
    }

    public void SetAnimation(Vector3 v)
    {
        Animator.SetFloat(zVelHash, v.z);
        Animator.SetFloat(xVelHash, v.x);
        Animator.SetBool("Grounded", PlayerController.grounded);

        PlayerController.Speed = v.magnitude;
    }
    
    /*
    private void Update()
    {
        if (!PV.IsMine)
            return;

        var vel = CharacterController.velocity;
        var v = transform.InverseTransformDirection(vel);
        
        Animator.SetFloat(zVelHash, v.z);
        Animator.SetFloat(xVelHash, v.x);
        Animator.SetBool("Grounded", PlayerController.grounded);

        PlayerController.Speed = v.magnitude;
    }
    */
}