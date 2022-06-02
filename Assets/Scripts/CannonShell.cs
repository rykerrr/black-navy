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

    private void Update()
    {
        graphics.transform.right = thisRb.velocity;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        //thisRb.velocity = ((Vector2)transform.right * speed * Time.fixedDeltaTime);
        //LogUtils.DebugLog("enabled cannon shell");
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        thisRb.velocity = Vector2.zero;
        //LogUtils.DebugLog("disabled cannon shell");
    }
}
#pragma warning restore 0649