using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
#pragma warning disable 0649
public class WeaponGuidedTorpedoLauncher
{
    [Header("WeaponProperties")]
    [SerializeField] private Transform torpedoPrefab;
    [SerializeField] private Transform torpedoBay;
    [SerializeField] private Transform owner;
    [SerializeField] private float torpedoDelay;

    [HideInInspector] public LayerMask whatIsTarget;
    [HideInInspector] public LayerMask whatAreOurProjectiles;

    private float torpedoTimer;
 
    private void FireTorpedo(Transform target)
    {
        Transform torpClone = Poolable.Get<GuidedTorpedo>(() => Poolable.CreateObj<GuidedTorpedo>(torpedoPrefab.gameObject), torpedoBay.position, Quaternion.identity).transform;
        Torpedo torp = torpClone.GetComponent<Torpedo>();
        torp.whatIsTarget = whatIsTarget;
        int layerValue = whatAreOurProjectiles.layermask_to_layer();
        torpClone.gameObject.layer = layerValue;

        //if (CheckIfLookingAtTarget(out Transform subTarget) == true) // leave this to the submarine
        //{
        //    if (subTarget)
        //    {
        //        
        //    }
        //}
        torp.target = target;

        torpClone.up = owner.up;

        torpedoTimer = torpedoDelay + Time.time;
    }
}
#pragma warning restore 0649