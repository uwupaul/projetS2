using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class UIManager : MonoBehaviourPunCallbacks
{
    [SerializeField] public SettingsMenu settingsMenu;
    public bool EscapeMod;
    
    public static UIManager Instance;
    
    private void Awake()
    {
        if (Instance && Instance != this)// verifie si une autre roomManager exists
        {
            Destroy(gameObject); // il ne peut y en avoir que un 
            return;
        }
        Instance = this;
        EscapeMod = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape pressed in UIManager");
            settingsMenu.Toggle();
        }
    }
}