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

    public void FireTorpedo(Transform target)
    {
        if (Time.time >= torpedoTimer)
        {
            Transform torpClone = Poolable.Get<GuidedTorpedo>(() => Poolable.CreateObj<GuidedTorpedo>(torpedoPrefab.gameObject), torpedoBay.position, Quaternion.identity).transform;
            GuidedTorpedo torp = torpClone.GetComponent<GuidedTorpedo>();
            torp.target = target;
            torp.whatIsTarget = whatIsTarget;
            int layerValue = whatAreOurProjectiles.layermask_to_layer();
            torpClone.gameObject.layer = layerValue;

            torpClone.up = owner.up;

            torpedoTimer = torpedoDelay + Time.time;
        }
    }
}
#pragma warning restore 0649