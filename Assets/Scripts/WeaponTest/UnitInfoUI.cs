using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitInfoUI : MonoBehaviour
{
    private Camera cam;
    private CameraController camContrll;
    private Transform selUnit;
    private UnitBase selUnitScript;
    private UnitHumanoid selUnitHum;
    private GameObject unitInfoObj;
    private Text unitInfoText;

    private string weaponStr = "";

    private void Start()
    {
        cam = Camera.main;
        camContrll = cam.GetComponent<CameraController>();
    }

    private void LateUpdate()
    {
        if (camContrll)
        {
            if (camContrll.SelectedUnit != null)
            {
                if (!selUnit)
                {
                    selUnit = camContrll.SelectedUnit;
                    selUnitScript = selUnit.GetComponent<UnitBase>();
                    selUnitHum = selUnit.GetComponent<UnitHumanoid>();

                    if (selUnitHum.type == UnitType.Base)
                    {
                        camContrll.ClearUnitSelection();
                        selUnit = null;

                        return;
                    }

                    if (selUnitScript)
                    {
                        unitInfoObj = selUnit.GetComponent<UnitBase>().UnitInfoUI;
                        unitInfoText = unitInfoObj.GetComponent<Text>();
                    }

                    int i = 1;

                    foreach (WeaponBase wep in selUnitScript.Weapons)
                    {
                        weaponStr = "Weapon " + i++ + ": " + wep.name + "\n - RPM: " + (60 / wep.delayBetweenFire) + "\n - Damage per hit: " + wep.damage + "\n";
                    }
                }

            }
            else
            {
                if (unitInfoText)
                {
                    unitInfoText.text = "";
                }

                selUnit = null;
                return;
            }

            if (selUnit)
            {
                string newInfo = "";

                newInfo += "Health: " + selUnitHum.Health + " / " + selUnitHum.MaxHealth + "\n";
                newInfo += "Speed: " + selUnitScript.CurSpeed + "\n";
                newInfo += "Altitude: " + (GameConfig.Instance.WaterLevel + selUnit.transform.position.y) + "\n";
                newInfo += "VSI: " + selUnitScript.VSI;
                newInfo += weaponStr;

                unitInfoText.text = newInfo; // figure out a way to size it or smth
            }
        }
    }
}
