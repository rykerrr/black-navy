using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class UnguidedMissile : UnguidedProjectile
{
    [Header("Unguided missile properties")]
    [SerializeField] private int fuel;

    private int curFuel;
    private bool boost;

    private void Start()
    {
        OnEnable();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (boost)
        {
            fuel--;

            if (isOutOfWater)
            {
                if (fuel >= 0)
                {
                    thisRb.AddForce(transform.up * speed * Time.deltaTime, ForceMode2D.Force);
                }
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

        thisRb.AddForce(transform.up * speed * Time.deltaTime, ForceMode2D.Impulse);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        curFuel = fuel;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        boost = false;
        thisRb.velocity = Vector2.zero;
        curFuel = 0;
    }
}
#pragma warning restore 0649