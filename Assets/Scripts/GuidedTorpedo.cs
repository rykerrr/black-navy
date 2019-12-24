using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class GuidedTorpedo : GuidedProjectile
{
    // [Header("Guided torpedo properties")]

    private void Update()
    {
        if (!target)
        {
            FindTarget();
        }

        if (Time.timeScale >= 0.1f && isOutOfWater)
        {
            if (target)
            {
                Vector3 dist = (target.position - transform.position).normalized;
                transform.up = Vector3.MoveTowards(transform.up, dist, rotationSmoothing);
            }
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (isOutOfWater)
        {
            thisRb.velocity = transform.up * speed * Time.fixedDeltaTime;
        }
    }
}
#pragma warning restore 0649