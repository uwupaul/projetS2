using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerGroundCheck : MonoBehaviour
{
    PlayerController playerController;

    public Vector3 enterPos;
    public Vector3 exitPos;

    public int damageOnFall;
    public int heightFallDmg;
    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
        Debug.Log("Awake du ground check");
    }
    
    /*
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter");
        
        enterPos = transform.position;

        if (exitPos.y - enterPos.y > heightFallDmg)
            playerController.TakeDamage(damageOnFall * Convert.ToInt32(exitPos.y - enterPos.y), null);
        
        if (other.gameObject == playerController.gameObject)
            return;
        
        
        playerController.SetGroundedState(true);
    }
    void OnTriggerExit(Collider other)
    {
        Debug.Log("OnTriggerExit");
        
        exitPos = transform.position;

        if (other.gameObject == playerController.gameObject)
            return;

        playerController.SetGroundedState(false);
    }
    void OnTriggerStay(Collider other)
    {
        Debug.Log("OnTriggerStay");
        
        if (other.gameObject == playerController.gameObject)
            return;
        
        
        playerController.SetGroundedState(true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("OnCollisionEnter");
        
        if (collision.gameObject == playerController.gameObject)
            return;
        
        
        playerController.SetGroundedState(true);    
    }

    private void OnCollisionExit(Collision collision)
    {
        Debug.Log("OnCollisionExit");
        
        if (collision.gameObject == playerController.gameObject)
            return;

        playerController.SetGroundedState(false);
    }

    private void OnCollisionStay(Collision collision)
    {
        Debug.Log("OnCollisionStay");
        
        if (collision.gameObject == playerController.gameObject)
            return;
        
        playerController.SetGroundedState(true);  
    }
    */
}