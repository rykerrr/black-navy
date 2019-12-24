using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class UnguidedMissile : UnguidedProjectile
{
    [Header("Unguided missile properties")]
    [SerializeField] private float forceGen;

    private bool boost;

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (boost)
        {
            if (isOutOfWater)
            {
                thisRb.AddForce(transform.up * forceGen * Time.deltaTime, ForceMode2D.Force);
            }
        }
    }

    public void ActivateBoost()
    {
        boost = true;

        if (!thisRb)
        {
            thisRb = GetComponent<Rigidbody2D>();
        }

        thisRb.AddForce(transform.up * forceGen * Time.deltaTime, ForceMode2D.Impulse);
    }
}
#pragma warning restore 0649