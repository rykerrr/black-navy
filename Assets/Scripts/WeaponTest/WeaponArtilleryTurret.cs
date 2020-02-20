using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class WeaponArtilleryTurret : WeaponBase
{
    [Header("Artillery turret properties")]
    [SerializeField] private Transform barrel;
    [SerializeField] private ParticleSystem firePartSys;
    [SerializeField] private int firePartCount;
    [SerializeField] private float rotationSmoothing;

    private Transform prevBull;
    private Rigidbody2D bullRb;
    private Vector2 aimPos;
    private float projBaseGrav;
    private int projVol;

    protected override void Awake()
    {
        base.Awake();
        projVol = Mathf.CeilToInt(delayBetweenFire);
    }

    private void Update()
    {
        if (target)
        {
            if (!target.gameObject.activeInHierarchy)
            {
                target = null;
                return;
            }

            if(targHumanoid == null)
            {
                target = null;
                return;
            }

            if (targHumanoid.type == UnitType.Base)
            {
                FindTarget(3f);
            }

            if (target.position.x < transform.position.x)
            {
                transform.eulerAngles = new Vector3(transform.eulerAngles.z, 180f, transform.eulerAngles.z);
            }
            else if (target.position.x > transform.position.x)
            {
                transform.eulerAngles = new Vector3(transform.eulerAngles.z, 0f, transform.eulerAngles.z);
            }

            Fire();
        }
        else
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, owner.transform.eulerAngles.y, transform.eulerAngles.z);

            if (FindTarget())
            {
                return;
            }
        }

        barrel.up = Vector2.MoveTowards(barrel.up, aimPos, rotationSmoothing * Time.deltaTime);
    }

    public override FireState Fire()
    {
        if (target == null)
        {
            return FireState.Failed;
        }
        else
        {
            if (!target.gameObject.activeInHierarchy)
            {
                target = null;
                return FireState.Failed;
            }

            int results;

            results = SolveBallisticArc(spawnLocation.position, projectilePrefab.GetComponent<ProjectileBase>().Speed * 0.03f, target.position, -Physics2D.gravity.y * projBaseGrav, out Vector3 s0, out Vector3 s1);
            aimPos = s0.normalized;


            if (Time.time > fireTimer)
            {
                if (prevBull == null)
                {
                    prevBull = Instantiate(projectilePrefab, spawnLocation.position, Quaternion.identity);
                    int layerValue = whatAreOurProjectiles.layermask_to_layer();
                    prevBull.gameObject.layer = layerValue;
                    prevBull.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                    prevBull.gameObject.SetActive(false);
                    bullRb = prevBull.GetComponent<Rigidbody2D>();
                }

                if (results > 0 && VecApprox(barrel.up, aimPos))
                {
                    prevBull.position = spawnLocation.position;
                    prevBull.gameObject.SetActive(true);
                    bullRb.AddForce(((Vector2)barrel.up * projectilePrefab.GetComponent<ProjectileBase>().Speed + new Vector2(0f, Random.Range(-inaccuracyOffset, inaccuracyOffset))) * 0.03f, ForceMode2D.Impulse);
                    TrailRenderer projTrail = prevBull.GetComponent<TrailRenderer>();
                    projTrail.material = prevBull.gameObject.layer == 8 ? t1Mat : t2Mat;
                    firePartSys.Emit(firePartCount);
                    prevBull = null;

                    fireSound.Play();
                    //soundMngr.PlayEnviroSound(spawnLocation.gameObject, "cannonfire1", 30f, projVol);
                    fireTimer = delayBetweenFire + Time.time + Random.Range(-delayBetweenFire / 5f, delayBetweenFire / 3.4f);

                    return FireState.Fired;
                }


            }
            else
            {
                return FireState.OnDelay;
            }
        }

        return FireState.Failed;
    }

    private void OnEnable()
    {
        projBaseGrav = projectilePrefab.GetComponent<Rigidbody2D>().gravityScale;
    }
}
#pragma warning restore 0649