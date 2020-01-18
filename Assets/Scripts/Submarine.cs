using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class Submarine : ShipBase
{
    [Header("Submarine properties")]
    [SerializeField] private WeaponGuidedSSMissileLauncher ssMissileLauncher;
    [SerializeField] private WeaponGuidedTorpedoLauncher torpedoLauncher;
    [SerializeField] private float subFinderRadius;
    [SerializeField] private float subFinderRange;
    [SerializeField] private float subFinderDelay;

    private Transform isSubInSight;
    private float subFinderTimer;

    private Vector2 veloc2;

    private void Start()
    {
        OnEnable();
    }

    private void Update()
    {
        if (target)
        {
            FindEnemySub();

            if(targHumanoid.type != UnitType.Submarine)
            {
                ssMissileLauncher.LaunchSSMissile();
            }

            if (isSubInSight != null)
            {
                torpedoLauncher.FireTorpedo(isSubInSight);
            }
            else
            {
                torpedoLauncher.FireTorpedo(target);
            }
        }
        else
        {
            torpedoLauncher.FireTorpedo(null);

            if (FindTarget())
            {
                Debug.Log(FindTarget());
                return; // loop back
            }
        }
    }

    private void FindEnemySub()
    {
        if (Time.time > subFinderTimer)
        {
            isSubInSight = isEnemySubInView();

            subFinderTimer = Time.time + subFinderDelay;
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
        torpedoLauncher.whatAreOurProjectiles = whatAreOurProjectiles;
        torpedoLauncher.whatIsTarget = whatIsTarget;
        ssMissileLauncher.whatAreOurProjectiles = whatAreOurProjectiles;
        ssMissileLauncher.whatIsTarget = whatIsTarget;
    }

    private void OnDisable()
    {
        target = null;
        curSpd = 0f;
    }

    private Transform isEnemySubInView()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, subFinderRadius, transform.up, subFinderRange, whatIsTarget);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null)
            {
                if (hit.collider.GetComponent<Submarine>())
                {
                    return hit.transform;
                }
            }
        }

        return null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, subFinderRadius);
    }
}
#pragma warning restore 0649