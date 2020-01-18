using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class GuidedAAMissile : GuidedProjectile // air to air
{
    [Header("AA Missile Properties")]
    [SerializeField] private float timeBeforeFire;
    [SerializeField] private int fuel;

    private float fireTimer;
    private int curFuel;

    private void Update()
    {
        if (curFuel >= 0)
        {
            curFuel--;
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
                        transform.up = Vector3.MoveTowards(transform.up, dir.normalized, rotationSmoothing * Time.deltaTime); // there we go
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
        float curSpd = speed * Time.fixedDeltaTime;

        if (isOutOfWater)
        {
            if (curFuel >= 0)
            {
                thisRb.velocity = transform.up * curSpd;
            }
            else
            {
                return;
            }
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        curFuel = fuel;
        fireTimer = Time.time + timeBeforeFire;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        thisRb.velocity = Vector3.zero;
        target = null;
    }
}
#pragma warning restore 0649