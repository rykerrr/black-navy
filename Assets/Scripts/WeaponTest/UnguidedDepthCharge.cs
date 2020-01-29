using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class UnguidedDepthCharge : UnguidedProjectile
{
    private void Update()
    {
        isOutOfWater = IsOutOfWater();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        thisRb.velocity = Vector3.zero;
        StopAllCoroutines();
    }
}
#pragma warning restore 0649