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

    private void Update()
    {
        if (!PV.IsMine)
            return;

        var vel = CharacterController.velocity;
        Debug.Log($"{vel.x}, y : {vel.z}");
        var v = transform.InverseTransformDirection(vel);
        Debug.Log($"CC Z velocity : {v.z}, X : {v.x}");

        //Debug.Log($"X : {Animator.GetFloat("X_Velocity")}, Z : {Animator.GetFloat("Z_Velocity")}");
        v = Vector3.Lerp(v, new Vector3(Animator.GetFloat("X_Velocity"), 0, Animator.GetFloat("Z_Velocity")), Time.deltaTime);
        
        Animator.SetFloat(zVelHash, v.z);
        Animator.SetFloat(xVelHash, v.x);
        Animator.SetBool("Grounded", PlayerController.grounded);

        PlayerController.Speed = v.magnitude;
    }
}