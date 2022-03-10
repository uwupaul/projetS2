using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager menuManager;
    
    [SerializeField] Menu[] menus;

    private void Awake()
    {
        menuManager = this;
    }

    public void OpenMenu(string menuName)
    {
        foreach (Menu menu in menus)
        {
            if (menu.menuName == menuName)
                menu.Open();
            
            else if (menu.open)
                CloseMenu(menu);
        }
    }

    public void OpenMenu(Menu menu)
    {
        foreach (Menu otherMenu in menus)
        {
            if (otherMenu.open)
                CloseMenu(otherMenu);
        }
        menu.Open();
    }

    public void CloseMenu(Menu menu)
    {
        menu.Close();
    }

    public void CloseMenu(string name)
    {
        foreach (Menu menu in menus)
        {
            if (menu.menuName == name)
            {
                menu.Close();
                return;
            }
        }
    }
}
