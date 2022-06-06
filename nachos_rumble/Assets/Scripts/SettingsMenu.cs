using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class SettingsMenu : MonoBehaviourPunCallbacks
{
    public AudioMixer audioMixer;
    [SerializeField] Slider masterVolumeSlider;
    [SerializeField] Slider mouseSensitivitySlider;
    [SerializeField] TMP_Dropdown resolutionDropdown;
    [SerializeField] Toggle FullScreenToggle;
    public float mouseSensitivity;

    private Tuple<int,int>[] resolutions =
    {
        new Tuple<int, int>(2560, 1440),
        new Tuple<int, int>(1920, 1080),
        new Tuple<int, int>(1600, 900),
        new Tuple<int, int>(1024, 576)
    };

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
        mouseSensitivity = 1.5f;
        mouseSensitivitySlider.value = mouseSensitivity;
        resolutionDropdown.value = FindCurrentResolution();
        FullScreenToggle.isOn = Screen.fullScreen;
    }

    int FindCurrentResolution()
    {
        for (int i = 0; i < resolutions.Length; i++) 
        {
            if (resolutions[i].Item1 == Screen.width &&
                resolutions[i].Item2 == Screen.height)
            {
                return i;
            }
        }

        return 0;
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
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }
    
    
    public void SetMouseSensitivity(float sens)
    {
        mouseSensitivity = sens;
    }

    public void SetScreenResolution(int index)
    {
        Screen.SetResolution(resolutions[index].Item1, resolutions[index].Item2, Screen.fullScreen);
    }

    public void ToggleFullScreen(bool toggle)
    {
        Screen.fullScreen = toggle;
    }
    
    public void QuitGame()
    {
        PlayerData.Instance.SaveProfile();
        Application.Quit();
    }
    
    public void QuitRoom()
    {
        PlayerData.Instance.SaveProfile();
        PhotonNetwork.LeaveRoom();
    }
}
