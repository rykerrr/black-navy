using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class WeaponTurretPlacer : MonoBehaviour
{
    [SerializeField] bool placer = false;
    [SerializeField] private WeaponBase selectedTurret = null;

    private Camera targCam;
    private List<WeaponPlacementNode> nodes = new List<WeaponPlacementNode>();

    private void Start()
    {
        targCam = Camera.main;
        nodes = Resources.FindObjectsOfTypeAll<WeaponPlacementNode>().ToList();
        //LogUtils.DebugLog(FindObjectsOfTypeAll(typeof(WeaponPlacementNode)));
    }

    private void Update()
    {
        if (placer)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                TryFindNode();
            }
            else if (Input.anyKeyDown && (!Input.GetKeyDown(KeyCode.A) || !Input.GetKeyDown(KeyCode.W) || !Input.GetKeyDown(KeyCode.D) || !Input.GetKeyDown(KeyCode.S)))
            {
                TurnOffPlacer();
            }
        }
    }

    private void TryFindNode()
    {
        Vector2 selectPos = targCam.ScreenToWorldPoint(Input.mousePosition);
        LayerMask nodeMask = new LayerMask();
        nodeMask = (1 << 13);

        RaycastHit2D hit = Physics2D.Raycast(selectPos, Vector2.zero, 3f, nodeMask);

        if (hit)
        {
            if (hit.collider)
            {
                LogUtils.DebugLog("Found node!");
                WeaponBase newTurr = Instantiate(selectedTurret).GetComponent<WeaponBase>();
                hit.collider.GetComponent<WeaponPlacementNode>().PlaceTurret(newTurr);
            }
        }

        TurnOffPlacer();
    }

    private void TurnOffPlacer()
    {
        foreach (WeaponPlacementNode nod in nodes)
        {
            nod.gameObject.SetActive(false);
        }

        selectedTurret = null;
        placer = false;
    }

    public void TurnOnPlacer(Transform newTurret)
    {
        if (!newTurret.GetComponent<WeaponBase>().unlocked)
        {
            LogUtils.DebugLog("Weapon isn't unlocked, doofus");
            return;
        }

        selectedTurret = newTurret.GetComponent<WeaponBase>();

        int nodeAmn = 0;

        foreach (WeaponPlacementNode nod in nodes)
        {
            if (nod.TypeOfMount == selectedTurret.typeOfMountRequired)
            {
                if (!nod.HasWeapon)
                {
                    nod.gameObject.SetActive(true);
                    LogUtils.DebugLog("found 1");
                    nodeAmn++;
                }
            }
            else
            {
                LogUtils.DebugLog(nod.TypeOfMount + " | " + selectedTurret.typeOfMountRequired);
            }
        }

        if (nodeAmn <= 0)
        {
            TurnOffPlacer();
            LogUtils.DebugLog("Not enough nodes that can mount said weapon, " + nodeAmn + " | " + nodes.Count);
            return;
        }

        placer = true;
    }
}
#pragma warning restore 0649