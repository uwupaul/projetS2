using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] SettingsMenu settingsMenu;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            settingsMenu.Toggle();
    }
}
