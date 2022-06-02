using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class Frigate : ShipBase
{
    [Header("Frigate properties")]
    [SerializeField] private WeaponGuidedSSMissileLauncher ssMissileLauncher;
    [SerializeField] private WeaponAutoCannon autoCannon;

    protected void Start()
    {
        OnEnable();
    }

    private void Update()
    {
        if(target)
        {
            ssMissileLauncher.LaunchSSMissile();
            autoCannon.FireCannon(0.1f);
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
        if(target)
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
        autoCannon.whatAreOurProjectiles = whatAreOurProjectiles;
        ssMissileLauncher.whatAreOurProjectiles = whatAreOurProjectiles;
        ssMissileLauncher.whatIsTarget = whatIsTarget;
        autoCannon.currentShells = autoCannon.MaxShells;
    }

    private void OnDisable()
    {
        target = null;
        curSpd = 0f;
    }
}
#pragma warning restore 0649