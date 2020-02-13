using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit Loadout", menuName = "Unit Loadout")]
public class UnitWeaponLoadout : ScriptableObject
{
    public UnitType unitType;
    public List<WeaponBase> weapons;
    public List<WeaponBase> unlockedWeapons;
    public List<WeaponBase> availableWeapons;

    private void OnEnable()
    {
        if(unlockedWeapons != null)
        {
            unlockedWeapons.Clear();
        }

        InitializeWeapons();
        //Debug.Log("init'd");
    }

    private void OnDisable()
    {
        //Debug.Log("uninit'd");
        unlockedWeapons.Clear();
    }

    private void InitializeWeapons()
    {
        if (availableWeapons != null)
        {
            if (availableWeapons.Count > 0)
            {
                foreach (WeaponBase possible in availableWeapons)
                {
                    if (possible)
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
}
