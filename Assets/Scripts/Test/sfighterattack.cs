using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649
[RequireComponent(typeof(Rigidbody2D))]
public class sfighterattack : MonoBehaviour
{
    [Header("Set by script, shown for debug purposes")]
    [SerializeField] private Transform target;
    [SerializeField] private Rigidbody2D thisRb;
    [SerializeField] private float yBaseAltitude;
    [SerializeField] private float evadeYAltitude;
    [Header("Properties")]
    [SerializeField] public UnitLayerMask whatUnitsToTarget;
    [SerializeField] public LayerMask whatAreOurProjectiles;
    [SerializeField] public LayerMask whatIsTarget;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform rocketBay;
    [SerializeField] private Transform shellPrefab;
    [SerializeField] private Transform rocketPrefab;
    [SerializeField] private float waterLevel;
    [SerializeField] private float evasionLength;
    [SerializeField] private float lookCheckRadius;
    [SerializeField] private float aircraftEscapeRange;
    [SerializeField] private float shipEscapeRange;
    [SerializeField] private float cannonDelay;
    [SerializeField] private float rocketDelay;
    [SerializeField] private float speed;
    [SerializeField] private float distanceBeforeFire;
    [SerializeField] private float targetCheckDelay;
    [SerializeField] private float targetCheckRadius;
    [SerializeField] private float rocketReloadTime;
    [SerializeField] private float timeToTakeOffFully;
    [SerializeField] private int ammoPerMagazine;
    [SerializeField] private int maxRockets;

    // properties
    public float Speed => speed;

    private Rigidbody2D targRb;
    private ShipHumanoid targProps;
    [SerializeField] private Vector3 evadePos = Vector3.zero;
    private Vector3 retPosition;
    private float veloc4;
    private float findTargetTimer;
    private float cannonTimer;
    private float sortieLengthTimer;
    private float rocketTimer;
    private float evadeTimer;
    private float curSpd;
    private float enemyIsTooCloseEvadeTimer;
    private int currentShells;
    private int currentRockets;
    private int evadeVSI; // indicator whether falling or rising
    private bool evading = false;
    private bool returningToBaseAlt = false;
    [SerializeField] private bool takenOff = false;

    private void Start()
    {
        thisRb = GetComponent<Rigidbody2D>();
        yBaseAltitude = 30f;
        evadeYAltitude = yBaseAltitude + 10f;
        currentShells = ammoPerMagazine;
        currentRockets = maxRockets;
    }


    private void Update()
    {
        if (returningToBaseAlt)
        {
            transform.up = Vector3.MoveTowards(transform.up, retPosition, Random.Range(0.07f, 0.1f));

            if (transform.position.y >= yBaseAltitude)
            {
                if (!takenOff)
                {
                    takenOff = true;
                }

                returningToBaseAlt = false;
                enemyIsTooCloseEvadeTimer = 0;
                currentShells = ammoPerMagazine;
            }

            return;
        }

        if (!takenOff)
        {
            curSpd = Mathf.SmoothDamp(curSpd, speed, ref veloc4, timeToTakeOffFully / 3f);
        }
        else
        {
            if (transform.position.y <= waterLevel + 7f && !returningToBaseAlt)
            {
                returningToBaseAlt = true;
                evading = false;
                retPosition = new Vector3(transform.up.x * 30f, yBaseAltitude + Random.Range(4f, 6f), transform.position.z);
            }

            if (currentRockets == 0)
            {
                currentRockets = -1;
                StartCoroutine(ReloadRockets());
            }

            if (evading == true)
            {
                if (Time.time > evadeTimer)
                {
                    evading = false;
                    currentShells = ammoPerMagazine;
                    return;
                }

                if ((target.position - transform.position).magnitude <= aircraftEscapeRange)
                {
                    enemyIsTooCloseEvadeTimer += Time.deltaTime;

                    if (enemyIsTooCloseEvadeTimer >= 5f)
                    {
                        Debug.Log("Too close!");
                        int rand = Random.Range(0, 30) > 20 ? -1 : 1;
                        retPosition = new Vector3(transform.up.x * Random.Range(14f, 30f) * rand, yBaseAltitude + Random.Range(4f, 14f), transform.position.z);
                        evading = false;
                        returningToBaseAlt = true;
                    }
                }
                else
                {
                    enemyIsTooCloseEvadeTimer = 0;
                }

                transform.up = Vector3.MoveTowards(transform.up, (evadePos - transform.position).normalized, 0.06f);
                return;
            }

            if (currentShells <= 0)
            {
                evadeVSI = transform.up.x > 0 ? Mathf.CeilToInt(transform.up.x) : Mathf.FloorToInt(transform.up.x);
                evadeVSI *= -1;
                evadePos = new Vector3(transform.up.x * 100f * evadeVSI, evadeYAltitude * Random.Range(2f, 3f), transform.position.z);
                evading = true;
                evadeTimer = (Time.time + evasionLength) * 1.2f;
                return;
            }

            if (target != null)
            {
                if ((target.position - transform.position).magnitude >= targetCheckRadius)
                {
                    target = null;
                    return;
                }

                bool targetIsVisual = CheckIfLookingAtTarget();

                if (targProps.whatAmI == UnitType.Aircraft)
                {
                    Vector3 dist = (target.position - transform.position + (Vector3)targRb.velocity / 2f).normalized;
                    transform.up = Vector3.MoveTowards(transform.up, dist, 0.04f);

                    if ((target.position - transform.position).magnitude <= aircraftEscapeRange)
                    {
                        evadeVSI = transform.up.x > 0 ? Mathf.CeilToInt(transform.up.x) : Mathf.FloorToInt(transform.up.x);
                        float yAlt = evadeVSI == 1 ? (evadeYAltitude * evadeVSI) * Random.Range(2f, 3f) : (evadeYAltitude * evadeVSI) / Random.Range(8f, 14f);
                        evadePos = new Vector3(transform.up.x * 100f, yAlt, transform.position.z);
                        evading = true;
                        evadeTimer = Time.time + evasionLength;
                        return;
                    }
                }
                else // target's a ship yeet
                {
                    Vector3 dist = (target.position - transform.position).normalized;
                    transform.up = Vector3.MoveTowards(transform.up, dist, 0.04f);

                    if ((target.position - transform.position).magnitude <= shipEscapeRange)
                    {
                        evadeVSI = transform.up.x > 0 ? Mathf.CeilToInt(transform.up.x) : Mathf.FloorToInt(transform.up.x);
                        float yAlt = evadeVSI == 1 ? (evadeYAltitude * evadeVSI) / Random.Range(1.4f, 1.6f) : (evadeYAltitude * evadeVSI) / Random.Range(8f, 14f);
                        evadePos = new Vector3(transform.up.x * 100f, yAlt, transform.position.z);
                        evading = true;
                        evadeTimer = Time.time + evasionLength;
                        return;
                    }
                }

                if (targetIsVisual)
                {
                    if (Time.time > cannonTimer && currentShells > 0)
                    {
                        FireCannon();
                    }

                    if (Time.time > rocketTimer && currentRockets > 0)
                    {
                        FireRocket();
                    }
                }
            }
            else
            {
                if (FindTarget())
                {
                    targProps = target.GetComponent<ShipHumanoid>();
                    targRb = target.GetComponent<Rigidbody2D>();
                    return; // looping back to the start since target was found
                }
                else
                {
                    findTargetTimer = Time.time + targetCheckDelay;
                }

                transform.up = Vector3.MoveTowards(transform.up, new Vector2(transform.up.x, 0f), 0.01f);
            }
        }

    }

    private void FixedUpdate()
    {
        thisRb.velocity = transform.up * curSpd * Time.fixedDeltaTime;
    }

    private IEnumerator ReloadRockets()
    {
        yield return new WaitForSeconds(rocketReloadTime);

        currentRockets = maxRockets;

        yield break;
    }

    private IEnumerator TakeOffRoutine()
    {
        takenOff = false;
        curSpd = 0f;

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
        curSpd = speed;
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
            shellClone.right = new Vector2(transform.up.x + Random.Range(-0.1f, 0.1f), transform.up.y); /*(target.position - transform.position).normalized*/;
            int layerValue = whatAreOurProjectiles.layermask_to_layer();
            shellClone.gameObject.layer = layerValue;
            Rigidbody2D shellRb = shellClone.GetComponent<Rigidbody2D>();
            shellRb.velocity = thisRb.velocity;
            cannonTimer = cannonDelay + Time.time;
            currentShells--;
        }
        else Debug.Log("No target");
    }

    private void FireRocket()
    {
        if (target)
        {
            Transform rocketClone = Instantiate(rocketPrefab, rocketBay.position, transform.rotation) as Transform;
            int layerValue = whatAreOurProjectiles.layermask_to_layer();
            rocketClone.gameObject.layer = layerValue;
            UnguidedRocket rocket = rocketClone.GetComponent<UnguidedRocket>();
            rocket.ActivateBoost();
            currentRockets--;
            rocketTimer = rocketDelay + Time.time;
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
            target = availableTargets[0].transform;

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
        Gizmos.DrawWireSphere(transform.position, distanceBeforeFire);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, aircraftEscapeRange);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(firePoint.position, lookCheckRadius);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(evadePos, 4f);
    }
}
#pragma warning restore 0649