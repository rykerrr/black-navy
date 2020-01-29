using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class GuidedTorpedo : GuidedProjectile
{
    [Header("Guided torpedo properties")]
    [SerializeField] private float timeToChangeGravityOutOfWater;

    private float veloc3;
    private float veloc4;
    private float newSpeed;

    private void Update()
    {
        isOutOfWater = IsOutOfWater(); // dont touch

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
        if (!isOutOfWater) // its a variable, check Update()
        {
            Debug.Log("is this even being called the fuck");
            newSpeed = Mathf.SmoothDamp(newSpeed, speed, ref veloc3, rotationSmoothing * Time.fixedDeltaTime);
            thisRb.velocity = transform.up * newSpeed * Time.fixedDeltaTime;
        }
    }

    protected override bool IsOutOfWater()
    {
        if (transform.position.y >= waterLevel)
        {
            thisRb.gravityScale = Mathf.SmoothDamp(thisRb.gravityScale, 1f, ref veloc4, timeToChangeGravityOutOfWater / 3f);
            Debug.Log("yeet");
            return true;
        }
        else
        {
            thisRb.gravityScale = Mathf.SmoothDamp(thisRb.gravityScale, inWaterGrav, ref veloc4, timeToChangeGravityOutOfWater / 1.2f);
            Debug.Log("nigger");
            return false;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        newSpeed = speed;
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