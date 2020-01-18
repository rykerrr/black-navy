using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
#pragma warning disable 0649
public class WeaponUnguidedMissileLauncher
{
    [Header("Weapon properties")]
    [SerializeField] private Transform missilePrefab;
    [SerializeField] private Transform owner;
    [SerializeField] private Transform missileBay;
    [SerializeField] private float missileDelay;

    [HideInInspector] public LayerMask whatAreOurProjectiles;
    [SerializeField] private float missileTimer;

    public bool LaunchMissile()
    {
        if(Time.time > missileTimer)
        {
            Transform missileClone = Poolable.Get<UnguidedMissile>(() => Poolable.CreateObj<UnguidedMissile>(missilePrefab.gameObject), missileBay.position, owner.rotation).transform;
            int layerValue = whatAreOurProjectiles.layermask_to_layer();
            missileClone.gameObject.layer = layerValue;
            missileClone.gameObject.SetActive(true);
            UnguidedMissile missile = missileClone.GetComponent<UnguidedMissile>();
            missile.ActivateBoost();
            missileTimer = missileDelay + Time.time;

            return true;
        }

        return false;
    }
}
#pragma warning restore 0649