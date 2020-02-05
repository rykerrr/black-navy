using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class WeaponAntiAirTurret : WeaponBase
{
    [SerializeField] private Transform barrel;
    [SerializeField] private ParticleSystem firePartSys;
    [SerializeField] private int firePartCount;
    [SerializeField] private float rotationSmoothing;

    private Transform prevBull;
    private Rigidbody2D bullRb;
    private Vector3 aimPos;

    private float grav = 0f;

    protected override void Awake()
    {
        base.Awake();
        grav = -projectilePrefab.GetComponent<Rigidbody2D>().gravityScale * Physics2D.gravity.y;
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

            if (target.position.x < transform.position.x)
            {
                transform.eulerAngles = new Vector3(transform.eulerAngles.z, 180f, transform.eulerAngles.z);
            }

            if (!targRb) targRb = target.GetComponent<Rigidbody2D>();

            barrel.up = (target.position - transform.position + (Vector3) targRb.velocity * Time.deltaTime).normalized;
            Fire();
        }
        else
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0f, transform.eulerAngles.z);

            if (FindTarget())
            {
                return;
            }
        }

        //barrel.up = Vector2.MoveTowards(barrel.up, aimPos, rotationSmoothing * Time.deltaTime);
    }

    public override FireState Fire()
    {
        if (target == null)
        {
            return FireState.Failed;
        }

        int results = SolveBallisticArc(spawnLocation.position, projectilePrefab.GetComponent<CannonShell>().Speed * Time.deltaTime, target.position, -Physics2D.gravity.y * grav, out Vector3 s0, out Vector3 s1);
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

            prevBull.position = spawnLocation.position;
            prevBull.gameObject.SetActive(true);
            bullRb.AddForce(((Vector2)barrel.up * projectilePrefab.GetComponent<CannonShell>().Speed  + new Vector2(0f, Random.Range(-inaccuracyOffset, inaccuracyOffset)) * 0.03f), ForceMode2D.Impulse); // changed Time.deltaTime to 0.03f due to weird physics behaviour on different speeds, change it back if it's not the cause
            TrailRenderer projTrail = prevBull.GetComponent<TrailRenderer>();
            projTrail.material = prevBull.gameObject.layer == 8 ? t1Mat : t2Mat;
            prevBull = null;
            firePartSys.Emit(firePartCount);

            fireTimer = delayBetweenFire + Time.time + Random.Range(-delayBetweenFire / 5f, delayBetweenFire / 3.4f);

            currentAmmo--;
            return FireState.Fired;
        }

        return FireState.OnDelay;
    }
}
#pragma warning restore 0649