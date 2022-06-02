using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class GuidedSSMissile : GuidedProjectile // surface to surface
{
    [Header("Surface to surface missile properties")]
    [SerializeField] private Transform afterBurner;
    [SerializeField] private int fuel;
    [SerializeField] private float maxDistToTarget;
    [SerializeField] private float boosterLength;
    [SerializeField] private float intermStageLength;

    [Header("Debug props")]
    [SerializeField] private bool attackingStage = false;
    [SerializeField] private bool boosting = false;
    [SerializeField] private bool intermStage = false;

    private int curFuel;

    private void Start()
    {
        OnEnable(); 
    }

    private void Update()
    {
        if(enabled)
        {
            if(fuel <= 0)
            {
                LogUtils.DebugLog("hello there from the other side");
            }
        }

        if (Time.timeScale >= 0.1f)
        {
            if (isOutOfWater)
            {
                if (intermStage || attackingStage)
                {
                    if (attackingStage)
                    {
                        if (curFuel >= 0)
                        {
                            curFuel--;
                        }
                    }

                    if (target)
                    {
                        Vector3 dist = (target.position - transform.position).normalized;
                        transform.up = Vector3.MoveTowards(transform.up, dist, rotationSmoothing * Time.deltaTime);
                    }
                    else
                    {
                        FindTarget();
                    }
                }
            }
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        float curSpd = speed * Time.fixedDeltaTime;

        if (boosting)
        {
            thisRb.velocity = curSpd * transform.up * 1.5f;
        }
        else
        {
            if (isOutOfWater)
            {
                if (attackingStage)
                {
                    if (fuel >= 0 && target)
                    {
                        thisRb.velocity = curSpd * transform.up;
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
        intermStage = true;
        boosting = false;
        //thisRb.AddForce(transform.up * speed / 1.5f * Time.fixedDeltaTime, ForceMode2D.Impulse);
        yield return new WaitForSeconds(intermStageLength);
        intermStage = false;
        attackingStage = true;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        curFuel = fuel;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        thisRb.velocity = Vector3.zero;
        attackingStage = false;
        boosting = false;
        intermStage = false;
        StopAllCoroutines();
        target = null;
    }
}
#pragma warning restore 0649