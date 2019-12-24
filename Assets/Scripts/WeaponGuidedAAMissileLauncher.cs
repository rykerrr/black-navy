using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
#pragma warning disable 0649
public class WeaponGuidedAAMissileLauncher
{
    [Header("Weapon properties")]
    [SerializeField] private Transform missilePrefab;
    [SerializeField] private Transform missileBay;
    [SerializeField] private Transform owner;
    [SerializeField] private float missileDelay;

    [HideInInspector] public LayerMask whatAreOurProjectiles;
    [HideInInspector] public LayerMask whatIsTarget;

    private float missileTimer;

    public void LaunchMissile(Transform target)
    {
        if (Time.time > missileTimer)
        {
            Transform missileClone = Poolable.Get<GuidedAAMissile>(() => Poolable.CreateObj<GuidedAAMissile>(missilePrefab.gameObject), missileBay.position, owner.rotation).transform;
            int layerValue = whatAreOurProjectiles.layermask_to_layer();
            missileClone.gameObject.layer = layerValue;
            GuidedAAMissile missile = missileClone.GetComponent<GuidedAAMissile>();
            missile.whatIsTarget = whatIsTarget;
            missile.target = target;
            missileTimer = missileDelay + Time.time;
        }
    }
}
#pragma warning restore 0649