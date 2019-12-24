using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class GuidedSSMissile : GuidedProjectile // surface to surface
{
    [Header("Surface to surface missile properties")]
    [SerializeField] private Transform afterBurner;
    [SerializeField] private float fuel;
    [SerializeField] private float maxDistToTarget;
    [SerializeField] private float boosterLength;
    [SerializeField] private float intermStageLength;

    private bool attackingStage = false;
    private bool boosting = false;
    private bool intermStage = false;

    private void Update()
    {
        if (Time.timeScale >= 0.1f)
        {
            if (isOutOfWater)
            {
                if (intermStage || attackingStage)
                {
                    bool foundTarget;

                    if (attackingStage)
                    {
                        if (fuel >= 0)
                        {
                            fuel--;
                        }
                    }

                    if (target == null)
                    {
                        foundTarget = FindTarget();
                    }
                    else
                    {
                        if ((target.position - transform.position).magnitude >= maxDistToTarget)
                        {
                            foundTarget = false;
                        }
                        else
                        {
                            foundTarget = true;
                        }
                    }

                    if (foundTarget)
                    {
                        Vector3 dist = (target.position - transform.position).normalized;
                        transform.up = Vector3.MoveTowards(transform.up, dist, rotationSmoothing);
                    }
                }
            }
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (boosting)
        {
            thisRb.velocity = speed * transform.up * Time.fixedDeltaTime * 1.5f;
        }
        else
        {
            if (isOutOfWater)
            {
                if (attackingStage)
                {
                    if (fuel >= 0 && target)
                    {
                        thisRb.velocity = speed * transform.up * Time.fixedDeltaTime;
                    }
                }
            }
        }
    }

    public void BoostStage()
    {
        StartCoroutine(Booster());
    }

    private IEnumerator Booster()
    {
        boosting = true;
        MeshRenderer abSprite = afterBurner.GetComponent<MeshRenderer>();
        abSprite.enabled = true;
        yield return new WaitForSeconds(boosterLength);
        abSprite.enabled = false;
        thisRb.velocity = Vector3.zero;
        thisRb.AddForce(transform.up * speed * Time.fixedDeltaTime / 1.5f, ForceMode2D.Impulse);
        intermStage = true;
        boosting = false;
        yield return new WaitForSeconds(intermStageLength);
        intermStage = false;
        attackingStage = true;
    }
}
#pragma warning restore 0649