using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
#pragma warning disable 0649
public struct UnitWeaponLoadout
{
    public string name; // unit name
    public UnitType unitType;
    public WeaponBase[] weapons;
    public WeaponBase[] availableWeapons;
}
#pragma warning disable 0649