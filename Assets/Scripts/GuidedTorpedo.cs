using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class GuidedTorpedo : GuidedProjectile
{
    [Header("Guided torpedo properties")]
    [SerializeField] private float timeToChangeGravityOutOfWater;

    private Vector3 veloc3;
    private float veloc4;

    private void Update()
    {
        isOutOfWater = IsOutOfWater();

        if (!target)
        {
            Debug.Log(target + ", finding target?");
            FindTarget();
        }

        if (Time.timeScale >= 0.1f && !isOutOfWater)
        {
            if (target)
            {
                Vector3 dist = (target.position - transform.position).normalized;
                transform.up = Vector3.MoveTowards(transform.up, dist, rotationSmoothing * Time.deltaTime);
            }
        }
    }

    protected override void FixedUpdate()
    {
        if (!isOutOfWater)
        {
            thisRb.velocity = Vector3.SmoothDamp(thisRb.velocity, transform.up * speed * Time.fixedDeltaTime, ref veloc3, rotationSmoothing / Time.fixedDeltaTime);
        }
    }

    protected override bool IsOutOfWater()
    {
        if (transform.position.y >= waterLevel)
        {
            thisRb.gravityScale = Mathf.SmoothDamp(thisRb.gravityScale, 3f, ref veloc4, timeToChangeGravityOutOfWater / 3f);
            return true;
        }
        else
        {
            thisRb.gravityScale = Mathf.SmoothDamp(thisRb.gravityScale, 0.001f, ref veloc4, timeToChangeGravityOutOfWater / 1.4f);
            return false;
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        thisRb.velocity = Vector3.zero;
        target = null;
        StopAllCoroutines();
    }
}
#pragma warning restore 0649