using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649
public class LoadoutSwitcharoo : Singleton<LoadoutSwitcharoo>
{
    [SerializeField] private Dropdown[] weaponDropdownMenus; // [unit][dropdownmenu] -> aircraft/submarine/ship [number], there needs to be a second number aka a 2d matrix
    // so that it doesn't activate 3 buttons if there are 2 weapons
    //[SerializeField] private WeaponBase[] AircraftWeapons; // [aircraft, submarine, ship][wep]
    //[SerializeField] private WeaponBase[] SubmarineWeapons;
    //[SerializeField] private WeaponBase[] ShipWeapons;
    [SerializeField] private UnitWeaponLoadout[] team1UnitLoadouts; // 1 for each unit
    [SerializeField] private UnitWeaponLoadout[] team2UnitLoadouts; // 1 for each unit

    private int curUnit;
    private int wepNum;
    private int team = 0;

    public int GetUnit => curUnit;
    public UnitWeaponLoadout[] GetTeam1UnitLoadouts => team1UnitLoadouts;
    public UnitWeaponLoadout[] GetTeam2UnitLoadouts => team2UnitLoadouts;

    private void Awake()
    {
        LoadBaseWeapons();
    }

    private void LoadBaseWeapons()
    {
        for (int i = 0; i < team1UnitLoadouts.Length; i++)
        {
            for (int j = 0; j < team1UnitLoadouts[i].weapons.Length; j++)
            {
                team1UnitLoadouts[i].weapons[j] = team1UnitLoadouts[i].availableWeapons[0];
            }
        }

        for (int i = 0; i < team2UnitLoadouts.Length; i++)
        {
            for (int j = 0; j < team2UnitLoadouts[i].weapons.Length; j++)
            {
                team2UnitLoadouts[i].weapons[j] = team2UnitLoadouts[i].availableWeapons[0];
            }
        }
    }

    public void SwitchUnit(int unit)
    {
        curUnit = unit;

        foreach (Dropdown drop in weaponDropdownMenus) // disables dropdown menus
        {
            drop.gameObject.SetActive(false);
            drop.ClearOptions();
        }

        List<Dropdown.OptionData> newOptions = new List<Dropdown.OptionData>();

        foreach (WeaponBase weapon in team1UnitLoadouts[unit].availableWeapons)
        {
            Dropdown.OptionData newOpt = new Dropdown.OptionData(weapon.name);
            newOptions.Add(newOpt);
        }

        for (int i = 0; i < team1UnitLoadouts[unit].weapons.Length; i++)
        {
            weaponDropdownMenus[i].gameObject.SetActive(true);

            weaponDropdownMenus[i].AddOptions(newOptions);
        }

        for (int i = 0; i < team1UnitLoadouts[unit].weapons.Length; i++)
        {
            weaponDropdownMenus[i].gameObject.SetActive(true);

            if (team == 0)
            {
                for (int j = 0; j < weaponDropdownMenus[i].options.Count; j++)
                {
                    if (team1UnitLoadouts[curUnit].weapons[i].name == weaponDropdownMenus[i].options[j].text)
                    {
                        weaponDropdownMenus[i].SetValueWithoutNotify(j);
                    }
                }
            }
            else if (team == 1)
            {
                for (int j = 0; j < weaponDropdownMenus[i].options.Count; j++)
                {
                    if (team2UnitLoadouts[curUnit].weapons[i].name == weaponDropdownMenus[i].options[j].text)
                    {
                        weaponDropdownMenus[i].SetValueWithoutNotify(j);
                    }
                }
            }
        }
    } // gets called when u tap on unit

    public void PickWeaponNum(int wepNum) // gets called as first function for weapon switch dropdown menu
    {
        this.wepNum = wepNum;
    }

    public void SwitchWeapon(int wep) // gets called as second function for weapon switch dropdown menu
    {
        //unitLoadouts[curUnit].weapons[wepNum] = PickableWeapons[(int) unitLoadouts[curUnit].unitType][wep];
        //unitLoadouts[curUnit].weapons[wepNum] = PickableWeapons[(int) unitLoadouts[curUnit].unitType]
        if (team == 0)
        {
            team1UnitLoadouts[curUnit].weapons[wepNum] = team1UnitLoadouts[curUnit].availableWeapons[wep];
        }
        else if (team == 1)
        {
            team2UnitLoadouts[curUnit].weapons[wepNum] = team2UnitLoadouts[curUnit].availableWeapons[wep];
        }

    }

    public void SwitchTeamLoadout(int team)
    {
        foreach (Dropdown drop in weaponDropdownMenus)
        {
            drop.gameObject.SetActive(false);
        }

        this.team = team;
    }
}

[System.Serializable]
public struct DropdownMenuGroups // left for tomorrow
{
    [SerializeField] private string name; // unit name

    [SerializeField] Dropdown.OptionData yeet;
}
#pragma warning disable 0649