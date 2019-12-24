using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class CannonShell : UnguidedProjectile
{
    // [Header("Cannon shell properties")]

    private void Start()
    {
        thisRb = GetComponent<Rigidbody2D>();
        thisRb.velocity = thisRb.velocity + (Vector2)transform.right * speed * Time.fixedDeltaTime;
    }
}
#pragma warning restore 0649