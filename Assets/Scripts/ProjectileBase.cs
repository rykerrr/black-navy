using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
[RequireComponent(typeof(Rigidbody2D))]
public abstract class ProjectileBase : Poolable, IDamager
{
    [Header("Projectile base properties")]
    [SerializeField] protected Rigidbody2D thisRb;
    [SerializeField] private AudioSource waterEnterSound;
    [SerializeField] protected Transform onHitParticlePrefab;
    [SerializeField] protected float speed;
    [SerializeField] private int damage;
    [SerializeField] public float inWaterGrav = -0.0005f;

    [Header("Debug")]
    [SerializeField] private float projectileLifetime;
    [SerializeField] private float lifeTimer;
    private SoundManager soundMngr;
    private TrailRenderer projTrail;
    private AudioSource hitSound;

    protected SpriteRenderer graphics;
    private Vector2 veloc2;
    private float veloc1;
    protected float waterLevel;
    private float origGravScale;
    protected bool isOutOfWater;

    public float Speed => speed;
    public int Damage => damage;

    protected virtual void Awake()
    {
        soundMngr = SoundManager.Instance;
        waterLevel = GameConfig.Instance.WaterLevel;
        projectileLifetime = GameConfig.Instance.ProjectileLifeTime;
        lifeTimer = Time.time + projectileLifetime;
        origGravScale = thisRb.gravityScale;
        projTrail = GetComponent<TrailRenderer>();
        hitSound = GetComponent<AudioSource>();

        if (!thisRb)
        {
            thisRb = GetComponent<Rigidbody2D>();
        }

        if((graphics = GetComponent<SpriteRenderer>()) == null)
        {
            if ((graphics = GetComponentInChildren<SpriteRenderer>()) == null)
            {
                LogUtils.DebugLog("No graphics??");
            }
        }
    }

    private void Update()
    {
        if (Time.time > lifeTimer && gameObject.activeInHierarchy && enabled)
        {
            //LogUtils.DebugLog("Deleting, " + lifeTimer);
            //LogUtils.DebugLog("Deleting: " + Time.time + " | " + projectileLifetime + " | " + (Time.time + projectileLifetime) + " | " + lifeTimer + " | " + gameObject.activeSelf + " | " + gameObject.activeInHierarchy);
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
            if(isOutOfWater)
            {
                waterEnterSound.Play();
            }

            thisRb.gravityScale = Mathf.SmoothDamp(thisRb.gravityScale, inWaterGrav, ref veloc1, Random.Range(50f, 90f) * Time.deltaTime);
            thisRb.velocity = Vector2.SmoothDamp(thisRb.velocity, thisRb.velocity / 2f, ref veloc2, Random.Range(10f, 25f) * Time.deltaTime);
            return false;
        }

        return true;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        Transform particleClone = Instantiate(onHitParticlePrefab, transform.position, Quaternion.identity) as Transform;

        if (GameConfig.Instance.BufferSound)
        {
            soundMngr.PlayExplosion(transform.position, hitSound);
        }
        else
        {
            AudioSource partSound = particleClone.GetComponent<AudioSource>();

            partSound.clip = hitSound.clip;
            partSound.volume = hitSound.volume;
            partSound.pitch = hitSound.pitch;
            partSound.dopplerLevel = hitSound.dopplerLevel;
            partSound.spatialBlend = hitSound.spatialBlend;

            partSound.Play();
        }

        //LogUtils.DebugLog(partSound);
        Destroy(particleClone.gameObject, hitSound.clip.length * 1.4f);

        if (collision.GetComponent<UnitHumanoid>())
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