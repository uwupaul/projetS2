using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public ItemInfo itemInfo;
    public GameObject itemGameObject;

    public bool isEquiped;

    public abstract void Equip();
    public abstract void Unequip();

    protected void ChangeLayersRecursively(Transform trans, string name)
    {
        // Pour changer un objet de layer. Sert à tout les items pour ne pas clip dans les murs.
        
        // Faire que les armes restent dans la caméra principale mais soit render en ShadowsOnly
        
        foreach (Transform child in trans)
        {
            child.gameObject.layer = LayerMask.NameToLayer(name);
            ChangeLayersRecursively(child, name);
        }
    }
}