using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

#pragma warning disable 0649
public class UIShopHandler : MonoBehaviour
{
    [SerializeField] private List<WeaponBase> weapons;
    private LoadoutSwitcharoo loadoutScript;

    private void Start()
    {
        loadoutScript = LoadoutSwitcharoo.Instance;
    }

    public void BuyWeapon()
    {
        GameObject button = EventSystem.current.currentSelectedGameObject;
        WeaponBase selectedWep = weapons.Find(wep => wep == button.GetComponent<WeaponshopTooltip>().Wep);

        selectedWep.unlocked = !selectedWep.unlocked;
        loadoutScript.RefreshLoadouts();
        Debug.Log(selectedWep.unlocked);
    }
}
#pragma warning restore 0649