using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class UIManager : MonoBehaviourPunCallbacks
{
    [SerializeField] SettingsMenu settingsMenu;
    public bool EscapeMod;
    // UTILISER UNE INSTANCE PUBLIC ET STATIC POUR FAIRE CE QU'ON VEUT????
    
    private void Awake()
    {
        EscapeMod = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            settingsMenu.Toggle();
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel(0);
    }
}