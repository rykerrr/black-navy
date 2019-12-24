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
    private float missileTimer;

    public void LaunchMissile()
    {
        if(Time.time > missileTimer)
        {
            Transform missileClone = Poolable.Get<UnguidedMissile>(() => Poolable.CreateObj<UnguidedMissile>(missilePrefab.gameObject), missileBay.position, owner.rotation).transform;
            int layerValue = whatAreOurProjectiles.layermask_to_layer();
            missileClone.gameObject.layer = layerValue;
            UnguidedMissile rocket = missileClone.GetComponent<UnguidedMissile>();
            rocket.ActivateBoost();
            missileTimer = missileDelay + Time.time;
        }

    }
}
#pragma warning restore 0649