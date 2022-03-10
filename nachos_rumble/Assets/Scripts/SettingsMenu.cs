using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using Photon.Pun;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    PlayerController PC;
    [SerializeField] Slider masterVolumeSlider;
    [SerializeField] Slider mouseSensitivitySlider;

    private void Awake()
    {
        PC = FindObjectOfType<PlayerController>();

        float masterVolume;
        audioMixer.GetFloat("MasterVolume",out masterVolume);

        masterVolumeSlider.value = masterVolume;
        mouseSensitivitySlider.value = PC.mouseSensitivity;
    }

    public void Toggle()
    {
        gameObject.SetActive(!gameObject.activeSelf);
        PC.EscapeMod = !PC.EscapeMod;
        
        if (!PC.EscapeMod)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", volume);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }
    
    
    public void SetMouseSensitivity(float sens)
    {
        PC.mouseSensitivity = sens;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
    public void QuitRoom()
    {
        Destroy(RoomManager.Instance.gameObject);
        PhotonNetwork.LeaveRoom();
    }
}
