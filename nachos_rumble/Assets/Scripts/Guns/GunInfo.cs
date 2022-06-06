using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FPS/New Gun")]

public class GunInfo : ItemInfo
{
    public float damage;
    public float shotSpread;
    public float shotsPerSeconds;
    public float scopeSensMultiplier;
    public float noscopePrecisionMultiplier;
    public float movementPrecisionMultiplier;
    public float adsFOV;
    public float hipFOV;
    public float bulletInGun;
}
