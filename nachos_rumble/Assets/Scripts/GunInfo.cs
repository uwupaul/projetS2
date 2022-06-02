using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FPS/New Gun")]

public class GunInfo : ItemInfo
{
    public float damage;
    public float shotSpread;
    public float shotsPerSeconds;
    public bool isAutomatic;
    public bool canScope;
    public int gunIndex;
}
