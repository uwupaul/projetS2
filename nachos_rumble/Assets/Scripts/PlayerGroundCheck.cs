using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerGroundCheck : MonoBehaviour
{
    PlayerController playerController;

    public Vector3 enterPos;
    public Vector3 exitPos;

    public int damageOnFall = 3;
    public int heightFallDmg = 5;
    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
    }

    void OnTriggerEnter(Collider other)
    {
        enterPos = transform.position;

        if (exitPos.y - enterPos.y > heightFallDmg)
        { 
            playerController.TakeDamage(damageOnFall * Convert.ToInt32(exitPos.y - enterPos.y), null);
        }
        
        if (other.gameObject == playerController.gameObject)
        {return;}
        playerController.SetGroundedState(true);
    }
    
    void OnTriggerExit(Collider other)
    {
        exitPos = transform.position;

        if (other.gameObject == playerController.gameObject)
        {return;}
        playerController.SetGroundedState(false);
    }
    void OnTriggerStay(Collider other)
    {
        if (other.gameObject == playerController.gameObject)
        {return;}
        playerController.SetGroundedState(true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == playerController.gameObject)
        {return;}
        playerController.SetGroundedState(true);    
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject == playerController.gameObject)
        {return;}
        playerController.SetGroundedState(false);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject == playerController.gameObject)
        {return;}
        playerController.SetGroundedState(true);  
    }
}