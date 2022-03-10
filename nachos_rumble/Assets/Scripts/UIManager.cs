using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class UIManager : MonoBehaviourPunCallbacks
{
    [SerializeField] SettingsMenu settingsMenu;
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