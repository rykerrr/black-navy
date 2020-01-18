using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public abstract class ShipBase : UnitBase
{
    [Header("Base ship properties")]
    [HideInInspector] protected float veloc1 = 0f;
}
#pragma warning restore 0649