using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class GuidedTorpedo : GuidedProjectile
{
    [Header("Guided torpedo properties")]
    [SerializeField] private float timeToChangeGravityOutOfWater;

    [SerializeField] private float newSpeed;

    private void Update()
    {
        if (!target)
        {
            FindTarget();
        }

        if (Time.timeScale >= 0.1f)
        {
            isOutOfWater = IsOutOfWater(); // dont touch

            if (isOutOfWater)
            {
                newSpeed = Mathf.MoveTowards(newSpeed, 0f, rotationSmoothing * 5f * Time.deltaTime);

                transform.up = Vector2.MoveTowards(transform.up, thisRb.velocity.normalized, 0.1f * Time.deltaTime);
            }
            else
            {
                newSpeed = Mathf.MoveTowards(newSpeed, speed, rotationSmoothing * Time.deltaTime);

                if (target)
                {
                    Vector3 dist = (target.position - transform.position).normalized;
                    transform.up = Vector3.MoveTowards(transform.up, dist, rotationSmoothing * 2f * Time.deltaTime);
                }
            }


        }

    }

    protected override void FixedUpdate()
    {
        if (isOutOfWater)
        {
            //thisRb.velocity = Vector2.MoveTowards(thisRb.velocity, Vector2.zero, 1f * Time.deltaTime);
        }
        else
        { // its only being set here so that gravity negates the velocity change when out of water and i dont get headaches about that yes
            thisRb.velocity = Vector2.MoveTowards(thisRb.velocity, transform.up * newSpeed * Time.fixedDeltaTime, rotationSmoothing);
        }
    }

    protected override bool IsOutOfWater()
    {
        if (transform.position.y >= waterLevel)
        {// is out of water
            thisRb.gravityScale = Mathf.MoveTowards(thisRb.gravityScale, 1f, timeToChangeGravityOutOfWater * Time.deltaTime);
            thisRb.velocity = new Vector2(Mathf.MoveTowards(thisRb.velocity.x, 0f, timeToChangeGravityOutOfWater * 2f * Time.deltaTime), thisRb.velocity.y);
            return true;
        }
        else
        { // is in water
            thisRb.gravityScale = Mathf.MoveTowards(thisRb.gravityScale, inWaterGrav, timeToChangeGravityOutOfWater / 1.2f * Time.deltaTime);
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