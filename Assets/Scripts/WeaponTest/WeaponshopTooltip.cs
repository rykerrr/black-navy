using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

#pragma warning disable 0649
public class WeaponshopTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Multiline] public string curText;

    [SerializeField] private Image imgObj;
    [SerializeField] private Text textObj;
    [SerializeField] private float appearanceTimer;
    [SerializeField] private float waitBeforeReappTimer;
    [SerializeField] private WeaponBase wep;
    [SerializeField] private List<UnitWeaponLoadout> loadoutsThatUseWeapon = new List<UnitWeaponLoadout>();
    //private float veloc1;
    //private float veloc2;

    public WeaponBase Wep => wep;

    private LoadoutSwitcharoo loadoutLoader;
    private WeaponAdditionalInfo wepInfo;
    private float timeToWait;
    private bool pointerDown;
    private int i = 0;

    private void Start()
    {
        loadoutLoader = LoadoutSwitcharoo.Instance;
        wepInfo = wep.GetComponent<WeaponAdditionalInfo>();
        CheckWhichUnitUsesWeapon();

        curText = "Name: " + wep.name + "\nUnlocked: " + wep.unlocked + "\nMax ammo before rearm: " + (wep.maxAmmo > 50000 ? Mathf.Infinity : wep.maxAmmo == 0 ? Mathf.Infinity : wep.maxAmmo)
+ "\nRearmament time: " + wep.reloadTime + "\nDamage per projectile: " + wep.damage + "\nWeapon type: " + FormatString("" + wep.typeOfWeapon)
+ "\nExtra desc: " + wepInfo.desc + "Cost: " + wep.ShopCost + "\nUnits that can use this: ";

        foreach (UnitWeaponLoadout loadout in loadoutsThatUseWeapon)
        {
            if (loadout == loadoutsThatUseWeapon[loadoutsThatUseWeapon.Count - 1])
            {
                curText += loadout.name;
            }
            else
            {
                curText += loadout.name + ", ";
            }
        }

        //if (GameConfig.Instance.IsSandbox)
        //{
        //}
        //else
        //{

        //}

        //Debug.Log("" + transform.name[5]);
        //Debug.Log(int.Parse("" + transform.name[5]));
        //Debug.Log(loadoutLoader.GetTeam1UnitLoadouts[loadoutLoader.GetUnit]);
        //Debug.Log(loadoutLoader.GetTeam1UnitLoadouts[loadoutLoader.GetUnit].unlockedWeapons[int.Parse("" + transform.name[5])]);
        imgObj.color = new Color(imgObj.color.r, imgObj.color.g, imgObj.color.b, 0f);
        textObj.color = new Color(textObj.color.r, textObj.color.g, textObj.color.b, 0f);
    }

    private void Update()
    {
        if (pointerDown)
        {
            curText = "Name: " + wep.name + "\nUnlocked: " + wep.unlocked + "\nMax ammo before rearm: " + (wep.maxAmmo > 50000 ? Mathf.Infinity : wep.maxAmmo == 0 ? Mathf.Infinity : wep.maxAmmo)
+ "\nRearmament time: " + wep.reloadTime + "\nDamage per projectile: " + wep.damage + "\nWeapon type: " + FormatString("" + wep.typeOfWeapon)
+ "\nExtra desc: " + wepInfo.desc + "Cost: " + wep.ShopCost + "\nUnits that can use this: ";

            //Debug.Log("pointer's down!");
            if (imgObj.color.a >= 0.00001f)
            {
                timeToWait = appearanceTimer;
                imgObj.StartCoroutine(StartDamp(1, 0f, imgObj, textObj, curText, timeToWait, null, true));
            }
            else
            {
                //imgObj.StopAllCoroutines();
                timeToWait = appearanceTimer;
                imgObj.StartCoroutine(StartDamp(1, waitBeforeReappTimer, imgObj, textObj, curText, timeToWait, null, true));
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        imgObj.StopAllCoroutines();

        //if (imgObj.color.a >= 0.00001f)
        //{
        //    textObj.text += "\n";
        //}

        pointerDown = true;
        //Debug.Log("enter");
        //if (imgObj.color.a >= 0.00001f)
        //{
        //    StartCoroutine(StartDamp(1, 0f, imgObj, textObj, curText, timeToWait, null, false));
        //    timeToWait = appearanceTimer;
        //}
        //else
        //{
        //    StartCoroutine(StartDamp(1, waitBeforeReappTimer, imgObj, textObj, curText, timeToWait, null, true));
        //    timeToWait = appearanceTimer;
        //}
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        imgObj.StopAllCoroutines();
        //Debug.Log("exit");
        pointerDown = false;
        textObj.text = "";
        i = 0;

        if (imgObj.color.a <= 0.999f)
        {
            imgObj.StartCoroutine(StartDamp(0f, 0f, imgObj, textObj, curText, timeToWait, () => { textObj.text = ""; i = 0; Debug.Log("memes"); }));
            timeToWait = appearanceTimer;
        }
        else
        {
            imgObj.StartCoroutine(StartDamp(0f, waitBeforeReappTimer / 4f, imgObj, textObj, curText, timeToWait, () => { textObj.text = ""; i = 0; Debug.Log("memes"); }));
            timeToWait = appearanceTimer;
        }
    }

    private void CheckWhichUnitUsesWeapon()
    {
        foreach (UnitWeaponLoadout loadout in loadoutLoader.GetTeam1UnitLoadouts)
        {
            foreach (WeaponBase weapon in loadout.availableWeapons)
            {
                if (weapon == wep)
                {
                    loadoutsThatUseWeapon.Add(loadout);
                    break;
                }
            }
        }
    }

    private string FormatString(string str)
    {
        string newStr = "" + str[0];

        for (int i = 1; i < str.Length; i++)
        {
            if ((int)str[i] > 64 && (int)str[i] < 91)
            {
                newStr += " ";
            }

            newStr += str[i];
        }

        return newStr;
    }

    private IEnumerator StartDamp(float alpha, float waitTime, Image imgObj, Text textObj, string curText, float timeToWait, Action callback = null, bool startText = false)
    {
        yield return new WaitForSeconds(waitTime);

        imgObj.StartCoroutine(DampToAlpha(alpha, imgObj, textObj, timeToWait));

        yield return new WaitForSeconds(0.3f);

        if (startText)
        {
            Coroutine textCor = imgObj.StartCoroutine(TextSlowlyAppears(imgObj, textObj, curText));
        }

        yield break;
    }

    private IEnumerator DampToAlpha(float alpha, Image imgObj, Text textObj, float timeToWait, Action callback = null)
    {
        while (true)
        {
            if (imgObj) imgObj.color = new Color(imgObj.color.r, imgObj.color.g, imgObj.color.b, Mathf.MoveTowards(imgObj.color.a, alpha, timeToWait));
            if (textObj) textObj.color = new Color(textObj.color.r, textObj.color.g, textObj.color.b, Mathf.MoveTowards(textObj.color.a, alpha, timeToWait));

            if (imgObj.color.a <= alpha + Mathf.Epsilon && imgObj.color.a >= alpha - Mathf.Epsilon)
            {
                callback?.Invoke();
                yield break;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator TextSlowlyAppears(Image imgObj, Text textObj, string curText)
    {
        while (true)
        {
            if (i > curText.Length - 1)
            {
                //Debug.Log("breaking out");
                yield break;
            }

            textObj.text += curText[i++];

            yield return new WaitForEndOfFrame();
        }
    }
}
#pragma warning restore 0649