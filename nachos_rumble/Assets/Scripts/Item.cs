using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public ItemInfo itemInfo;
    public GameObject itemGameObject;

    public abstract void Use();

    #region scope

    public Animator animator;

    public GameObject scopeOverlay;
    public GameObject weaponCamera;
    public Camera mainCam;
    
    public bool isScoped = false;
    public float scopedDistance = 15f;
    
    private GameObject textHealth;
    private GameObject ui_username;
    private GameObject ui_kills;
    private GameObject ui_death;
    private GameObject HealthBar;
    private GameObject crossHair;

    private float normalFOV = 70f;

    private void Awake()
    {
        mainCam.fieldOfView = 70;
    }

    private void Start()
    {
        ui_username = GameObject.Find("Canvas/BottomLeft/UsernameText");
        ui_kills = GameObject.Find("Canvas/TopLeft/KillsText");
        ui_death = GameObject.Find("Canvas/TopLeft/DeathText");
        textHealth = GameObject.Find("Canvas/BottomLeft/TextHealth");
        HealthBar = GameObject.Find("Canvas/BottomLeft/HealthBar");
        crossHair = GameObject.Find("Canvas/Crosshair");
        
        HealthBar.SetActive(true);
        ui_username.SetActive(true);
        ui_kills.SetActive(true);
        ui_death.SetActive(true);
        textHealth.SetActive(true);
        crossHair.SetActive(true);
    }

    public void Scope()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isScoped = !isScoped;
        }

        if (isScoped)
        {
            StartCoroutine(Scoped());
        }
        else
        {
            UnScoped();
        }
    }
    
    public void SimpleScope()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isScoped = !isScoped;
        }

        if (isScoped)
        {
            StartCoroutine(SimpleScoped());
        }
        else
        {
            SimpleUnScoped();
        }
    }
    
    public void SimpleUnScoped()
    {
        animator.SetBool("Scoped", isScoped);
        crossHair.SetActive(true);
        mainCam.fieldOfView = 70f;
    }

    IEnumerator SimpleScoped()
    {
        animator.SetBool("Scoped", isScoped);
        
        yield return new WaitForSeconds(.18f);

        if (crossHair != null)
        {
            crossHair.SetActive(false);
        }
        
        normalFOV = mainCam.fieldOfView;
        mainCam.fieldOfView = 55f;
    }

    public void UnScoped()
    {
        animator.SetBool("Scoped", isScoped);
        
        scopeOverlay.SetActive(false);
        weaponCamera.SetActive(true);
        
        HealthBar.SetActive(true);
        ui_username.SetActive(true);
        ui_kills.SetActive(true);
        ui_death.SetActive(true);
        textHealth.SetActive(true);
        crossHair.SetActive(true);
        

        mainCam.fieldOfView = 70f;
    }

    IEnumerator Scoped()
    {
        animator.SetBool("Scoped", isScoped);
        
        yield return new WaitForSeconds(.18f);

        scopeOverlay.SetActive(true);
        weaponCamera.SetActive(false);
        
        HealthBar.SetActive(false);
        ui_username.SetActive(false);
        ui_kills.SetActive(false);
        ui_death.SetActive(false);
        textHealth.SetActive(false);

        if (crossHair != null)
        {
            crossHair.SetActive(false);
        }

        normalFOV = mainCam.fieldOfView;
        mainCam.fieldOfView = scopedDistance;
    }
    #endregion
}
