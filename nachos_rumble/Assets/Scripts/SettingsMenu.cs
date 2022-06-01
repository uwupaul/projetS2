using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class SettingsMenu : MonoBehaviourPunCallbacks
{
    public AudioMixer audioMixer;
    [SerializeField] Slider masterVolumeSlider;
    [SerializeField] Slider mouseSensitivitySlider;
    public float mouseSensitivity;

    bool _EscapeMod
    {
        get => UIManager.Instance.EscapeMod;
        set => UIManager.Instance.EscapeMod = value;
    }

    private void Awake()
    {
        float masterVolume;
        audioMixer.GetFloat("MasterVolume",out masterVolume);
        
        masterVolumeSlider.value = masterVolume;
        mouseSensitivity = 2;
        mouseSensitivitySlider.value = mouseSensitivity;
    }

    public void Toggle()
    {
        gameObject.SetActive(!gameObject.activeSelf);
        _EscapeMod = !_EscapeMod;
        
        if (_EscapeMod)
            EnableMouse();
        else
            DisableMouse();
    }

    public static void EnableMouse()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public static void DisableMouse()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", volume);
        Debug.Log($"SettingsMenu : SetMasterVolume to {volume}");
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        Debug.Log($"SettingsMenu : SetQuality to {qualityIndex}");
    }
    
    
    public void SetMouseSensitivity(float sens)
    {
        mouseSensitivity = sens;
        Debug.Log($"SettingsMenu : SetMouseSensitivity to {sens}");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
    public void QuitRoom()
    {
        //Destroy(RoomManager.Instance.gameObject);
        PhotonNetwork.LeaveRoom();
    }
}
