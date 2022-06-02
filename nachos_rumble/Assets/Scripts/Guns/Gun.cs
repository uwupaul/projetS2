using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gun : Item
{
    public GameObject bulletImpactPrefab;
    public bool isScoped;
    public abstract override void Equip();
    public abstract override void Unequip();
    
    // faire que les guns ont un attribut LookMultiplyer, MoveMultiplyer qui est référencé dans le PlayerController,
    // et qui influe donc sur la vitesse et la sensibilité du PlayerController
}
