using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
[CreateAssetMenu(fileName = "New Unit Loadout", menuName = "Unit Loadout")]
public class UnitWeaponLoadout : ScriptableObject
{
    public UnitType unitType;
    public List<WeaponBase> weapons;
    public List<WeaponBase> unlockedWeapons;
    public List<WeaponBase> availableWeapons;

    private void OnEnable()
    {
        InitializeWeapons();
        //LogUtils.DebugLog("init'd");
    }

    private void OnDisable()
    {
        //LogUtils.DebugLog("uninit'd");
        // unlockedWeapons.Clear();
    }

    public void InitializeWeapons()
    {
        if (unlockedWeapons != null)
        {
            unlockedWeapons.Clear();
        }

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
#pragma warning restore 0649