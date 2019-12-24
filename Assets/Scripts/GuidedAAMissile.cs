using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class GuidedAAMissile : GuidedProjectile // air to air
{
    [Header("AA Missile Properties")]
    [SerializeField] private float timeBeforeFire;
    [SerializeField] private float fuel;

    private float fireTimer;

    private void Start()
    {
        fireTimer = Time.time + timeBeforeFire;
    }

    private void Update()
    {
        if(fuel >= 0)
        {
            fuel--;
        }

        if (Time.time > timeBeforeFire)
        {
            if (isOutOfWater)
            {
                if (Time.timeScale >= 0.1f)
                {
                    if (target)
                    {
                        Vector2 dir = target.position - transform.position;
                        Debug.DrawRay(transform.position, dir, Color.magenta); // change this to MoveTowards
                        transform.up = Vector3.MoveTowards(transform.up, dir.normalized, rotationSmoothing); // there we go
                        //float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f; // can use this to check angle to break the lock
                        //Quaternion angleAxis = Quaternion.AngleAxis(angle, Vector3.forward);
                        //transform.rotation = Quaternion.Slerp(transform.rotation, angleAxis, 1f);
                    }
                }
            }
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if(isOutOfWater)
        {
            if(fuel >= 0)
            {
                thisRb.velocity = transform.up * speed * Time.fixedDeltaTime;
            }
            else
            {
                return;
            }
        }
    }
}
#pragma warning restore 0649