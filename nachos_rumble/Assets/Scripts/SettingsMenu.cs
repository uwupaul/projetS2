using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] GameManager _gameManager;

    bool _EscapeMod
    {
        get => _gameManager.EscapeMod;
        set => _gameManager.EscapeMod = value;
    }

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
        _EscapeMod = !_EscapeMod;
        
        if (_EscapeMod)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
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
