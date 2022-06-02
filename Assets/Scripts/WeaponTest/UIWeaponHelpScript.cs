using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649
public class UIWeaponHelpScript : MonoBehaviour
{
    [SerializeField] private List<GameObject> strongAgainstImageObjs;
    [SerializeField] private WeaponAdditionalInfo selectedInfo;
    [SerializeField] private Image weaponImg;
    [SerializeField] private Text strongAgainstName;
    [SerializeField] private Text strongAgainstType;
    [SerializeField] private Text unitNameText;
    [SerializeField] private Text unitTypeText;
    [SerializeField] private Text wepName;
    [SerializeField] private Text wepDesc;

    private List<Image> strongAgainstImgs = new List<Image>();
    private List<StrongAgainstTooltip> strongAgainstTooltips = new List<StrongAgainstTooltip>();
    private Image selectedTtipUnit;

    private void Start()
    {
        foreach (GameObject obj in strongAgainstImageObjs)
        {
            List<Image> imgs = obj.GetComponentsInChildren<Image>().ToList();

            if(imgs.Count >= 1)
            {
                strongAgainstImgs.Add(imgs[1]);
            }

            strongAgainstTooltips.Add(obj.GetComponent<StrongAgainstTooltip>());
            obj.SetActive(false);
        }

        SelectWeapon(selectedInfo);
    }

    public void SelectWeapon(WeaponAdditionalInfo wepInfo)
    {
        for (int i = 0; i < wepInfo.strongAgainst.Count; i++)
        {
            strongAgainstImageObjs[i].SetActive(true);
            strongAgainstImgs[i].sprite = wepInfo.strongAgainst[i].Graphics.sprite;
            strongAgainstTooltips[i].unitName = wepInfo.strongAgainst[i].name;
            strongAgainstTooltips[i].unitType = "" + wepInfo.strongAgainst[i].Humanoid.type;
        }

        selectedInfo = wepInfo;
        wepName.text = wepInfo.gameObject.name;
        wepDesc.text = wepInfo.desc;
        weaponImg.sprite = wepInfo.UIImage;
        ChangeUnitTooltip(strongAgainstImageObjs[0].GetComponent<StrongAgainstTooltip>());
    }

    public void ChangeUnitTooltip(StrongAgainstTooltip ttip)
    {
        if(selectedTtipUnit)
        {
            selectedTtipUnit.color = new Color32(110, 110, 110, 255);
        }

        selectedTtipUnit = ttip.gameObject.GetComponent<Image>();
        selectedTtipUnit.color = new Color32(255, 255, 255, 255);

        unitNameText.text = ttip.unitName;
        unitTypeText.text = ttip.unitType;
    }
}
#pragma warning restore 0649