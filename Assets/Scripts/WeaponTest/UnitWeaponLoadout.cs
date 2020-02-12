using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
[CreateAssetMenu(fileName = "New loadout", menuName = "Loadout")]
public class UnitWeaponLoadout : ScriptableObject
{
    public UnitType unitType;
    public List<WeaponBase> weapons;
    public List<WeaponBase> unlockedWeapons;
    public List<WeaponBase> availableWeapons;

    private void OnEnable()
    {
        unlockedWeapons.Clear();
        InitializeWeapons();
        Debug.Log("init'd");
    }

    private void OnDisable()
    {
        Debug.Log("uninit'd");
        unlockedWeapons.Clear();
    }

    private void InitializeWeapons()
    {
        if(availableWeapons != null)
        {
            if (availableWeapons.Count > 0)
            {
                foreach (WeaponBase possible in availableWeapons)
                {
                    if (possible.unlocked)
                    {
                        unlockedWeapons.Add(possible);
                    }
                }
            }
        }
    }
}
#pragma warning disable 0649