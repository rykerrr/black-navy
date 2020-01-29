using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class StrikeFighter : AircraftBase
{
    [Header("Strike fighter properties")]
    [SerializeField] private WeaponUnguidedMissileLauncher unguidedMissileLauncher;
    [SerializeField] private WeaponAutoCannon autoCannon;
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
        int vsi = thisRb.velocity.y / Mathf.Abs(thisRb.velocity.y) >= 0 ? vsi = 1 : vsi = -1;

        if (vsi == 0)
        {
            Debug.Log("possible error @vsi");
            Debug.Break();
        }

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

        if (!takenOff)
        {
            curSpd = Mathf.SmoothDamp(curSpd, speed, ref veloc1, timeToTakeOffFully / 3f);
        }
        else
        {
            bool isNearWater = CheckIfNearWater();
            bool isAboveCeil = CheckifAboveCeil();

            if (autoCannon.currentShells <= 0)
            {
                Debug.Log("Out of ammo");
                evadePosition = new Vector3(transform.up.x * 10f * vsi, evadeAlt, transform.position.z);

                evading = true;
                evadeTimer = (Time.time + evasionLength) * 1.3f;
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
                        evadePosition = new Vector3(transform.position.x * 10, BaseAltitude * 1.4f, transform.position.z);
                        evading = true;
                        evadeTimer = Time.time + evasionLength;
                        return;
                    }
                }

                transform.up = Vector3.MoveTowards(transform.up, dist.normalized, 1.4f * rotationSmoothing * Time.deltaTime);

                if (targIsVisual)
                {
                    autoCannon.FireCannon(0.1f);
                    unguidedMissileLauncher.LaunchMissile();
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
                    transform.up = Vector3.MoveTowards(transform.up, new Vector3(transform.up.x, 0f, transform.position.z), rotationSmoothing);
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
            autoCannon.currentShells = autoCannon.MaxShells;
            Debug.Log("reloaded boy");
        }
    }

    protected override void ReturnToBaseAlt()
    {
        base.ReturnToBaseAlt();

        if (returningToBaseAlt == false)
        {
            autoCannon.currentShells = autoCannon.MaxShells;
            Debug.Log("reloaded boy");
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        autoCannon.currentShells = autoCannon.MaxShells;
        autoCannon.whatAreOurProjectiles = whatAreOurProjectiles;
        unguidedMissileLauncher.whatAreOurProjectiles = whatAreOurProjectiles;
    }
}
#pragma warning restore 0649