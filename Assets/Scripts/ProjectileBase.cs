using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
[RequireComponent(typeof(Rigidbody2D))]
public abstract class ProjectileBase : Poolable, IDamager
{
    [Header("Projectile base properties")]
    [SerializeField] protected Rigidbody2D thisRb;
    [SerializeField] protected Transform onHitParticlePrefab;
    [SerializeField] protected float speed;
    [SerializeField] private int damage;
    [SerializeField] public float inWaterGrav = -0.0005f;

    [Header("Debug")]
    [SerializeField] private float projectileLifetime;
    [SerializeField] private float lifeTimer;
    private TrailRenderer projTrail;
    private Vector2 veloc2;
    private float veloc1;
    protected float waterLevel;
    private float origGravScale;
    protected bool isOutOfWater;

    public float Speed => speed;
    public int Damage => damage;

    protected virtual void Awake()
    {
        waterLevel = GameConfig.Instance.WaterLevel;
        projectileLifetime = GameConfig.Instance.ProjectileLifeTime;
        lifeTimer = Time.time + projectileLifetime;
        origGravScale = thisRb.gravityScale;
        projTrail = GetComponent<TrailRenderer>();

        if (!thisRb)
        {
            thisRb = GetComponent<Rigidbody2D>();
        }
    }

    private void Update()
    {
        if (Time.time > lifeTimer && gameObject.activeInHierarchy && enabled)
        {
            //Debug.Log("Deleting, " + lifeTimer);
            //Debug.Log("Deleting: " + Time.time + " | " + projectileLifetime + " | " + (Time.time + projectileLifetime) + " | " + lifeTimer + " | " + gameObject.activeSelf + " | " + gameObject.activeInHierarchy);
            ReturnToPool();
        }
    }

    protected virtual void FixedUpdate()
    {
        isOutOfWater = IsOutOfWater();
    }

    protected virtual bool IsOutOfWater()
    {
        if (transform.position.y <= waterLevel)
        {
            thisRb.gravityScale = Mathf.SmoothDamp(thisRb.gravityScale, inWaterGrav, ref veloc1, Random.Range(50f, 90f) * Time.deltaTime);
            thisRb.velocity = Vector2.SmoothDamp(thisRb.velocity, thisRb.velocity / 2f, ref veloc2, Random.Range(10f, 25f) * Time.deltaTime);
            return false;
        }

        return true;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        Transform particleClone = Instantiate(onHitParticlePrefab, transform.position, Quaternion.identity) as Transform;
        Destroy(particleClone.gameObject, 1f);

        if(collision.GetComponent<UnitHumanoid>())
        {
            UnitHumanoid unitHit = collision.GetComponent<UnitHumanoid>();
            unitHit.TakeDamage(this);
        }

        ReturnToPool();
    }

    protected virtual void OnEnable()
    {
        lifeTimer = projectileLifetime + Time.time;
        thisRb.gravityScale = origGravScale;
        projTrail.gameObject.SetActive(true);
    }

    protected virtual void OnDisable()
    {
        lifeTimer = 0f;
        projTrail.gameObject.SetActive(false);
    }
}
#pragma warning restore 0649