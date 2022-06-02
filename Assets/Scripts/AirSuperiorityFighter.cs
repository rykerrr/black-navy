using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class AirSuperiorityFighter : AircraftBase
{
    [Header("Air superiority fighter properties")]
    [SerializeField] private WeaponAutoCannon autoCannon;
    [SerializeField] private WeaponGuidedAAMissileLauncher missileLauncher;
    [SerializeField] private float evasionLength;
    [SerializeField] private float escapeRange;

    private float veloc1;

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
            LogUtils.DebugLog("possible error @vsi");
            Debug.Break();
        }

        if (returningToBaseAlt)
        {
            ReturnToBaseAlt();
            return;
        }

        if (evading)
        {
            Evade(escapeRange);
            return;
        }

        if (transform.position.y >= yBaseAltitude * 2f)
        {
            retPosition = new Vector3(transform.up.x * 50f * -1f, yBaseAltitude * -2f, transform.position.z);
            returningToBaseAlt = true;
            return;
        }

        if (!takenOff)
        {
            curSpd = Mathf.SmoothDamp(curSpd, speed, ref veloc1, timeToTakeOffFully / 3f);
            LogUtils.DebugLog("hello");
        }
        else
        {
            bool isNearWater = CheckIfNearWater();
            bool isAboveCeil = CheckifAboveCeil();

            if (autoCannon.currentShells <= 0)
            {
                evadePosition = new Vector3(transform.up.x * 10f * vsi, evadeAlt, transform.position.z);

                evading = true;
                evadeTimer = (Time.time + evasionLength) * 1.2f;
            }

            if (target)
            {
                Vector3 dist = (target.position - transform.position);
                bool targIsVisual = CheckIfLookingAtTarget(90f);

                if (dist.magnitude >= targetCheckRadius)
                {
                    target = null;
                    return;
                }

                if (dist.magnitude <= escapeRange && !evading)
                {
                    evadePosition = new Vector3(transform.up.x * 100f, transform.position.y * 1.2f, transform.position.z);
                    evading = true;
                    evadeTimer = Time.time + evasionLength;
                    return;
                }

                transform.up = Vector3.MoveTowards(transform.up, dist.normalized, rotationSmoothing * Time.deltaTime * 1.4f);

                if (targIsVisual)
                {
                    curSpd = Mathf.SmoothDamp(curSpd, speed * 0.8f, ref veloc1, 2f);
                    autoCannon.FireCannon(0.01f);
                    missileLauncher.LaunchMissile(target);
                }
                else
                {
                    curSpd = Mathf.SmoothDamp(curSpd, speed * 1.2f, ref veloc1, 2f);
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
                    transform.up = Vector3.MoveTowards(transform.up, new Vector3(transform.up.x, 0f, transform.position.z), rotationSmoothing * Time.deltaTime);
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
        curSpd = Mathf.SmoothDamp(curSpd, speed * 1.4f, ref veloc1, 2f);
        base.Evade(escapeRange);

        if (evading == false)
        {
            autoCannon.currentShells = autoCannon.MaxShells;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        autoCannon.whatAreOurProjectiles = whatAreOurProjectiles;
        missileLauncher.whatAreOurProjectiles = whatAreOurProjectiles;
        autoCannon.currentShells = autoCannon.MaxShells;
    }

    private void OnDisable()
    {
    }
}
#pragma warning restore 0649