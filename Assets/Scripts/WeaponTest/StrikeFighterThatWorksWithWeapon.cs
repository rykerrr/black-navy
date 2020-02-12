﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class StrikeFighterThatWorksWithWeapon : AircraftBase
{
    [Header("Strike fighter properties")]
    [SerializeField] private float aircraftEscapeRange;
    [SerializeField] private float shipEscapeRange;
    [SerializeField] private float evasionLength;
    [SerializeField] private float distanceBeforeFire;

    private float veloc1;
    [SerializeField] private bool targIsVisual = false;

    protected override void Start()
    {
        base.Start();
        OnEnable();
    }

    private void Update()
    {
        vsi = (int)System.Math.Round(thisRb.velocity.y / Mathf.Abs(thisRb.velocity.y), 0, System.MidpointRounding.AwayFromZero);

        if(transform.position.y < waterLevel)
        {
            Debug.Log("HOOT HOOT! | " + name + " | " + vsi);
        }

        if (vsi == 0)
        {
            Debug.Log("possible error @vsi");
        }

        if (!takenOff)
        {
            curSpd = Mathf.SmoothDamp(curSpd, speed, ref veloc1, timeToTakeOffFully / 3f);
        }
        else
        {
            bool isNearWater = CheckIfNearWater();
            bool isAboveCeil = CheckifAboveCeil();

            if (returningToBaseAlt)
            {
                ReturnToBaseAlt();
                return;
            }

            if (evading)
            {
                Evade(aircraftEscapeRange);
                return;
            }

            if (target)
            {
                Vector3 dist = (target.position - transform.position);
                targIsVisual = CheckIfLookingAtTarget(distanceBeforeFire);

                if (dist.magnitude >= targetCheckRadius)
                {
                    target = null;
                    return;
                }

                if (targHumanoid.type == UnitType.Aircraft)
                {
                    if (dist.magnitude <= aircraftEscapeRange && !evading)
                    {
                        float yAlt = vsi == 1 ? evadeAlt * vsi * Random.Range(2f, 3f) : evadeAlt * vsi * Random.Range(0.2f, 0.7f);
                        yAlt *= transform.up.x >= 0 ? 1 : -1;
                        evadePosition = new Vector3(transform.up.x * 300, yAlt * 2f, transform.position.z);
                        evading = true;
                        evadeTimer = Time.time + evasionLength;
                        return;
                    }

                    //dist += (Vector3)(targRb.velocity / 4f);
                }
                else
                {
                    if (dist.magnitude <= shipEscapeRange && !evading)
                    {
                        evadePosition = new Vector3(transform.position.x * 1.1f, BaseAltitude * 1.4f, transform.position.z);
                        evading = true;
                        evadeTimer = Time.time + evasionLength;
                        return;
                    }
                }

                transform.up = Vector3.MoveTowards(transform.up, dist.normalized, 1.4f * rotationSmoothing * Time.deltaTime);

                if (targIsVisual)
                {
                    curSpd = Mathf.SmoothDamp(curSpd, speed * 0.8f, ref veloc1, 2f);
                }

                if (!FireWeapons())
                {
                    evadePosition = new Vector3(transform.position.x * 1.1f, evadeAlt * 1.4f, transform.position.z);

                    evading = true;
                    evadeTimer = (Time.time + evasionLength) * 1.2f;
                    return;
                }
            }
            else
            {
                if (FindTarget())
                {
                    return; // loop back
                }
                else
                {
                    transform.up = Vector3.MoveTowards(transform.up, new Vector3(transform.up.x, 0f, transform.position.z), rotationSmoothing * Time.time);
                }
            }

        }
    }

    private void FixedUpdate()
    {
        thisRb.velocity = transform.up * curSpd * Time.fixedDeltaTime;
    }

    protected override void Evade(float escapeRange)
    {
        base.Evade(escapeRange);

        if (evading == false)
        {
            ReloadWeapons();
        }
    }

    protected override void ReturnToBaseAlt()
    {
        base.ReturnToBaseAlt();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }
}
#pragma warning restore 0649