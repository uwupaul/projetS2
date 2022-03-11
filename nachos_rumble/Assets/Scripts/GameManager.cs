using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] SettingsMenu settingsMenu;
    GameManager gameManager;
    public bool EscapeMod;

    private void Awake()
    {
        gameManager = this;
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