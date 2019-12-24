using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
[RequireComponent(typeof(Rigidbody2D))]
public abstract class ProjectileBase : Poolable
{
    [Header("Projectile base properties")]
    [SerializeField] protected Rigidbody2D thisRb;
    [SerializeField] protected Transform onHitParticlePrefab;
    [SerializeField] protected float speed;

    private Vector2 veloc2;
    private float veloc1;
    protected float waterLevel = GameConfig.Instance.WaterLevel;
    protected bool isOutOfWater;

    protected virtual void FixedUpdate()
    {
        isOutOfWater = IsOutOfWater();
    }

    private bool IsOutOfWater()
    {
        if (transform.position.y <= waterLevel)
        {
            thisRb.gravityScale = Mathf.SmoothDamp(thisRb.gravityScale, 0.004f, ref veloc1, Random.Range(0.4f, 0.9f));
            thisRb.velocity = Vector2.SmoothDamp(thisRb.velocity, thisRb.velocity / 2f, ref veloc2, Random.Range(0.2f, 0.6f));
            return false;
        }

        return true;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        Transform particleClone = Instantiate(onHitParticlePrefab, transform.position, Quaternion.identity) as Transform;
        Destroy(particleClone.gameObject, 1f);
        ReturnToPool();
    }

}
#pragma warning restore 0649