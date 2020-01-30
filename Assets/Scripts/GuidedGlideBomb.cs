using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class GuidedGlideBomb : GuidedProjectile
{
    [Header("Guided glide bomb properties")]
    [SerializeField] private float forceGeneration;

    [HideInInspector] public float timeBeforeBoosters;
    private float boosterTimer;
    private bool targetIsVisual = false;
    private bool gliding = false;

    private void Update()
    {
        targetIsVisual = CheckIfLookingAtTarget();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if(isOutOfWater)
        {
            if (target && transform.position.y >= target.position.y)
            {
                Vector3 dist = (target.position - transform.position).normalized;
                transform.up = Vector3.MoveTowards(transform.up, dist, rotationSmoothing * Time.deltaTime);

                if (targetIsVisual)
                {
                    if (Time.time > boosterTimer)
                    {

                        if (!gliding)
                        {
                            foreach (Transform child in transform)
                            {
                                child.GetComponent<MeshRenderer>().enabled = true;
                            }
                            gliding = true;
                            thisRb.velocity /= 4f;
                            thisRb.gravityScale = 0.3f;
                        }

                        thisRb.AddForce(transform.up * forceGeneration * 0.03f, ForceMode2D.Force); // changed Time.deltaTime to 0.03f due to weird physics behaviour on different speeds, change it back if it's not the cause
                    }
                }
            }
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        boosterTimer = timeBeforeBoosters + Time.time;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        thisRb.velocity = Vector3.zero;
        gliding = false;
        target = null;
        boosterTimer = 0f;
        StopAllCoroutines();
    }
}
#pragma warning restore 0649