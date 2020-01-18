using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class CannonShell : UnguidedProjectile
{
    // [Header("Cannon shell properties")]

    private float gravity;

    private void Start()
    {
        thisRb = GetComponent<Rigidbody2D>();
        OnEnable();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        //thisRb.velocity = ((Vector2)transform.right * speed * Time.fixedDeltaTime);
        //Debug.Log("enabled cannon shell");
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        thisRb.velocity = Vector2.zero;
        //Debug.Log("disabled cannon shell");
    }
}
#pragma warning restore 0649