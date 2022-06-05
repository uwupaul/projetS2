using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator Animator;
    private CharacterController CharacterController;
    
    private int zVelHash;
    private int xVelHash;
    
    private void Start()
    {
        Animator = GetComponent<Animator>();
        CharacterController = GetComponent<CharacterController>();
        
        zVelHash = Animator.StringToHash("Z_Velocity");
        xVelHash= Animator.StringToHash("X_Velocity");
    }

    private void Update()
    {
        var v = transform.InverseTransformDirection(CharacterController.velocity);
            
        Animator.SetFloat(zVelHash, v.z);
        Animator.SetFloat(xVelHash, v.x);
    }
}