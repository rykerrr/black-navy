using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class BallisticSubmarine : ShipBase
{
    [Header("Ballistic submarine properties")]
    [SerializeField] private WeaponGuidedSSMissileLauncher ssMissileLauncher;

    private Vector2 veloc2;

    private void Start()
    {
        OnEnable();
    }

    private void Update()
    {
        if (target)
        {
            if (targHumanoid.type != UnitType.Submarine)
            {
                ssMissileLauncher.LaunchSSMissile();
            }
        }
        else
        {
            if (FindTarget())
            {
                LogUtils.DebugLog(FindTarget());
                return; // loop back
            }
        }
    }

    private void FixedUpdate()
    {
        if (target)
        {
            curSpd = Mathf.SmoothDamp(curSpd, 0f, ref veloc1, 3f);
        }
        else
        {
            curSpd = Mathf.SmoothDamp(curSpd, speed, ref veloc1, 4f);
        }

        thisRb.velocity = transform.up * curSpd * Time.fixedDeltaTime;
    }

    private void OnEnable()
    {
        ssMissileLauncher.whatAreOurProjectiles = whatAreOurProjectiles;
        ssMissileLauncher.whatIsTarget = whatIsTarget;
    }

    private void OnDisable()
    {
        target = null;
        curSpd = 0f;
    }
}
#pragma warning restore 0649
