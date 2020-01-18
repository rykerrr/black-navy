using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnguidedProjectile : ProjectileBase
{
    protected override void OnEnable()
    {
        base.OnEnable();
        //Debug.Log("unguided base created");
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        //Debug.Log("unguided base destroyed");
    }
}
