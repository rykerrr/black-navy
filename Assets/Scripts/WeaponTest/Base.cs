using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class Base : Poolable
{
    [SerializeField] private Transform[] roofMounts;
    [SerializeField] private Transform[] underwaterMounts;
    [SerializeField] private Transform airfieldSpawn;

    [SerializeField] public UnitLayerMask whatUnitsToTarget;
    [SerializeField] public LayerMask whatAreOurProjectiles;
    [SerializeField] public LayerMask whatIsTarget;
}
#pragma warning restore 0649