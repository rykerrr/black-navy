﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class WeaponPlacementNode : MonoBehaviour
{
    [SerializeField] private Base ownerBase;
    [SerializeField] private WeaponBase weapon;
    [SerializeField] private SpriteRenderer graphics;
    [SerializeField] private MountType type;

    public SpriteRenderer Graphics => graphics;
    public MountType TypeOfMount => type;
    public bool HasWeapon { get; private set; }

    private void Start()
    {
        if ((graphics = transform.GetComponent<SpriteRenderer>()) == null)
        {
            if ((graphics = transform.GetComponentInChildren<SpriteRenderer>()) == null)
            {
                Debug.Log("No sprite renderer found noob");
            }
        }

        gameObject.SetActive(false);
    }

    public bool PlaceTurret(WeaponBase turret)
    {
        Debug.Log("placing turret");
        if (weapon != null)
        {
            Debug.Log("Has a weapon already");
            return false;
        }

        turret.gameObject.layer = ownerBase.gameObject.layer;
        turret.whatAreOurProjectiles = ownerBase.whatAreOurProjectiles;
        turret.whatIsTarget = ownerBase.whatIsTarget;
        turret.owner = ownerBase.transform;
        turret.transform.parent = ownerBase.transform;
        HasWeapon = true;

        turret.transform.position = transform.position;
        Debug.Log("placed");
        return true;
    }

    public void RemoveTurret()
    {
        if (weapon != null)
        {
            weapon.ReturnToPool();
            HasWeapon = false;
            weapon = null;
        }
        else
        {
            Debug.Log("Doesn't have a weapon");
        }
    }
}

public enum MountType { Underwater, Roof }
#pragma warning restore 0649