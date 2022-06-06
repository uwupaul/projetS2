using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gun : Item
{
    public GameObject bulletImpactPrefab;
    public bool isScoped;
    public abstract override void Equip();
    public abstract override void Unequip();

    public float ComputePrecisionMultiplier(float speed)
    {
        // retourne le multiplicateur de précision : exemple -> si la fonction retourne 0.5, l'arme est moitié moins précise
        float res =  1 - speed * ((1 - ((GunInfo) itemInfo).movementPrecisionMultiplier) / 7);
        res = isScoped ? res : res * ((GunInfo) itemInfo).noscopePrecisionMultiplier;
        return res;
    }
}
