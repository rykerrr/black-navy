using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
#pragma warning disable 0649
public class AirSuperiorityFighter : MonoBehaviour
{
    [Header("Set by script, shown for debug purposes")]
    [SerializeField] private Transform target;
    [SerializeField] private Rigidbody2D targRb;
    [SerializeField] private Rigidbody2D thisRb;
    [Header("Properties")]
    [SerializeField] public UnitLayerMask whatUnitsToTarget;
    [SerializeField] public LayerMask whatAreOurProjectiles;
    [SerializeField] public LayerMask whatIsTarget;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform missileBay;
    [SerializeField] private Transform shellPrefab;
    [SerializeField] private Transform missilePrefab;
    [SerializeField] private float timeToTakeOffFully;
    [SerializeField] private float waterLevel;
    [SerializeField] private float minEngageRange;
    [SerializeField] private float engageRange;
    [SerializeField] private float cannonDelay;
    [SerializeField] private float missileDelay;
    [SerializeField] private float startSpeed;
    [SerializeField] private float startRotSmTime;
    [SerializeField] private float targetCheckDelay;
    [SerializeField] private float targetCheckRadius;
    [SerializeField] private float lookCheckRadius;
    [SerializeField] private float reloadTime;
    [SerializeField] private int ammoPerMagazine;

    // properties
    public float Speed => speed;

    private Vector3 retPosition;
    private float yBaseAltitude;
    private float floatveloc1;
    private float findTargetTimer;
    private float evasionTimer;
    private float cannonTimer;
    private float missileTimer;
    private float speed;
    private int cannonAmmo;
    private bool returningToBaseAlt;
    private bool engaging;
    private bool takenOff = false;

    private void Start()
    {
        speed = startSpeed;
        thisRb = GetComponent<Rigidbody2D>();
        yBaseAltitude = 40f;
        cannonAmmo = ammoPerMagazine;
    }


    private void Update()
    {
        if (returningToBaseAlt)
        {
            Debug.Log("returning to base alt");
            transform.up = Vector3.MoveTowards(transform.up, retPosition, Random.Range(0.08f, 0.12f));

            if (transform.position.y >= yBaseAltitude)
            {
                if (!takenOff)
                {
                    takenOff = true;
                }

                returningToBaseAlt = false;
                evasionTimer = 0;
            }

            return;
        }

        if (!takenOff)
        {
            speed = Mathf.SmoothDamp(speed, startSpeed, ref floatveloc1, timeToTakeOffFully / 3f);
        }
        else
        {
            if (!target)
            {
                speed = Mathf.SmoothDamp(speed, startSpeed, ref floatveloc1, 2f);
            }

            if (transform.position.y <= waterLevel + 17 && !returningToBaseAlt)
            {
                Debug.Log("returning to base alt is true");
                returningToBaseAlt = true;
                engaging = false;
                retPosition = new Vector3(transform.up.x * 70, yBaseAltitude + Random.Range(4f, 6f), transform.position.z);
                ReloadEverything();
            }

            if (cannonAmmo == 0)
            {
                Debug.Log("reload time");
                cannonAmmo = -1;
                engaging = false;

                StartCoroutine(ReloadEverything());
            }

            if (target)
            {
                if ((target.position - transform.position).magnitude >= targetCheckRadius)
                {
                    target = null;
                    return;
                }

                bool targetIsVisual = CheckIfLookingAtTarget();

                if (!engaging) // too close to engage or ammo is poof
                {
                    if (evasionTimer > 6f)
                    {
                        returningToBaseAlt = true;
                        retPosition = new Vector3(transform.up.x * 70, yBaseAltitude + Random.Range(4f, 6f), transform.position.z);
                        evasionTimer = 0;
                        return;
                    }

                    Debug.Log("too close to engage");
                    speed = Mathf.SmoothDamp(speed, startSpeed * 1.4f, ref floatveloc1, 2f);
                    transform.up = Vector3.MoveTowards(transform.up, new Vector2(transform.up.x * 140, yBaseAltitude * 1.1f), 0.06f);

                    if ((target.position - transform.position).magnitude >= engageRange && cannonAmmo > 0)
                    {
                        Debug.Log("engaging is true");
                        engaging = true;
                    }

                    evasionTimer += Time.deltaTime;
                    return;
                }

                if ((target.position - transform.position).magnitude <= minEngageRange)
                {
                    Debug.Log("engaging is true");
                    engaging = false;
                }


                Vector3 dist = (target.position - transform.position + (Vector3)targRb.velocity).normalized;
                transform.up = Vector3.MoveTowards(transform.up, dist, 0.04f);

                if (targetIsVisual)
                {
                    Debug.Log("target is in sight");
                    speed = Mathf.SmoothDamp(speed, startSpeed * 0.8f, ref floatveloc1, 2f);

                    if (Time.time > missileTimer)
                    {
                        LaunchMissile();
                    }

                    if (Time.time > cannonTimer)
                    {
                        if (cannonAmmo > 0)
                        {
                            FireCannon();
                        }
                    }
                }
                else
                {
                    speed = Mathf.SmoothDamp(speed, startSpeed * 2f, ref floatveloc1, 1f);
                }

            }
            else
            {
                transform.up = Vector3.MoveTowards(transform.up, new Vector2(transform.up.x, 0f), 0.01f);
            }

            if (Time.time >= findTargetTimer && !target)
            {
                FindTarget();

                if (target)
                {
                    targRb = target.GetComponent<Rigidbody2D>();
                }

                findTargetTimer = Time.time + targetCheckDelay;
            }
        }
    }

    private void FixedUpdate()
    {
        thisRb.velocity = transform.up * speed * Time.fixedDeltaTime;
    }

    private IEnumerator ReloadEverything()
    {
        yield return new WaitForSeconds(reloadTime);

        cannonAmmo = ammoPerMagazine;
        engaging = true;

        yield break;
    }

    private IEnumerator TakeOffRoutine()
    {
        takenOff = false;
        speed = 0f;

        yield return new WaitForSeconds(timeToTakeOffFully);

        returningToBaseAlt = true;
        retPosition = new Vector3(transform.up.x * 35f, yBaseAltitude + Random.Range(4f, 10f), transform.position.z);

        yield break;
    }

    public void TakeOff()
    {
        StartCoroutine(TakeOffRoutine());
    }

    public void LoadInAir()
    {
        takenOff = true;
        speed = startSpeed;
    }

    private bool CheckIfLookingAtTarget()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(firePoint.position, lookCheckRadius, transform.up, 50f, whatIsTarget);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null)
            {
                if (hit.transform.gameObject == target.gameObject)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void FireCannon()
    {
        if (target)
        {
            Transform shellClone = Instantiate(shellPrefab, firePoint.position, Quaternion.identity) as Transform;
            shellClone.right = transform.up;
            int layerValue = whatAreOurProjectiles.layermask_to_layer();
            shellClone.gameObject.layer = layerValue;
            Rigidbody2D shellRb = shellClone.GetComponent<Rigidbody2D>();
            shellRb.velocity = thisRb.velocity;
            cannonTimer = cannonDelay + Time.time;
            cannonAmmo--;
        }
        else Debug.Log("No target");
    }

    private void LaunchMissile()
    {
        if (target)
        {
            Transform missileClone = Instantiate(missilePrefab, missileBay.position, transform.rotation) as Transform;
            int layerValue = whatAreOurProjectiles.layermask_to_layer();
            missileClone.gameObject.layer = layerValue;
            Missile missile = missileClone.GetComponent<Missile>();
            missile.whatIsTarget = whatIsTarget;
            missile.target = target;
            missileTimer = missileDelay + Time.time;
        }
    }

    private bool FindTarget()
    {
        List<Collider2D> hit = (Physics2D.OverlapCircleAll(transform.position, targetCheckRadius, whatIsTarget)).ToList();
        List<Collider2D> availableTargets = new List<Collider2D>();

        foreach (Collider2D en in hit)
        {
            if (UnitLayerMask.CheckIfUnitIsInMask(en.GetComponent<ShipHumanoid>().whatAmI, whatUnitsToTarget) == true)
            {
                availableTargets.Add(en);
            }
        }

        /*for (int i = 0; i < availableTargets.Count; i++)
        {
            Debug.Log(" Index: " + i + " Name: " + hit[i].name + " Dist: " + (hit[i].transform.position - transform.position).magnitude);
        }*/

        availableTargets = availableTargets.OrderBy(en => Mathf.Abs((en.transform.position - transform.position).magnitude)).ToList();
        if (availableTargets.Count > 0)
        {
            target = availableTargets[availableTargets.Count - 1].transform;
            Debug.Log(target);
            Debug.Log(target.GetComponent<ShipHumanoid>());
            Debug.Log(target.GetComponent<ShipHumanoid>().whatAmI);
            Debug.Log(UnitLayerMask.CheckIfUnitIsInMask(target.GetComponent<ShipHumanoid>().whatAmI, whatUnitsToTarget));

            return true;
        }

        /*if (hit != null)
        {
            target = hit.transform;
            return true;
        }*/

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, targetCheckRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, engageRange);
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, minEngageRange);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(firePoint.position, lookCheckRadius);
    }
}
#pragma warning restore 0649