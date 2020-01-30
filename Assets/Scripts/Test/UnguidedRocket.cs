using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
[RequireComponent(typeof(Rigidbody2D))]
public class UnguidedRocket : MonoBehaviour
{
    [Header("Shown for debug purposes, set by script")]
    [SerializeField] private Rigidbody2D thisRb;
    [Header("Properties")]
    [SerializeField] private Transform onHitParticlePrefab;
    [SerializeField] private float waterLevel;
    [SerializeField] private float forceGen;

    private Vector2 veloc3 = Vector2.zero;
    private float veloc2;

    bool boost = false;

    private void Awake()
    {
        thisRb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (boost)
        {
            if (IsOutOfWater())
            {
                thisRb.AddForce(transform.up * forceGen * 0.03f, ForceMode2D.Force); // changed Time.deltaTime to 0.03f due to weird physics behaviour on different speeds, change it back if it's not the cause
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

        thisRb.AddForce(transform.up * forceGen * 0.03f, ForceMode2D.Impulse); // changed Time.deltaTime to 0.03f due to weird physics behaviour on different speeds, change it back if it's not the cause
    }

    private bool IsOutOfWater()
    {
        if (transform.position.y <= waterLevel)
        {
            thisRb.gravityScale = Mathf.SmoothDamp(thisRb.gravityScale, 0.004f, ref veloc2, Random.Range(0.4f, 1.5f));
            thisRb.velocity = Vector2.SmoothDamp(thisRb.velocity, thisRb.velocity / 2f, ref veloc3, Random.Range(0.4f, 1.2f));
            return false;
        }

        return true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Transform particleClone = Instantiate(onHitParticlePrefab, transform.position, Quaternion.identity) as Transform;
        Destroy(particleClone.gameObject, 1f);
        Destroy(gameObject);
    }
}
#pragma warning restore 0649