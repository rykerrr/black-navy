using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
#pragma warning disable 0649
public class ParasiteFighter : MonoBehaviour
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
    [SerializeField] private Transform rocketBay;
    [SerializeField] private Transform rocketPrefab;
    [SerializeField] private Transform onHitParticlePrefab;
    [SerializeField] private float waterLevel;
    [SerializeField] private float evasionLength;
    [SerializeField] private float lookCheckRadius;
    [SerializeField] private float aircraftEscapeRange;
    [SerializeField] private float shipEscapeRange;
    [SerializeField] private float rocketDelay;
    [SerializeField] private float speed;
    [SerializeField] private float distanceBeforeFire;
    [SerializeField] private float targetCheckDelay;
    [SerializeField] private float targetCheckRadius;
    [SerializeField] private int amountOfRockets;

    // properties
    public float Speed => speed;

    private Rigidbody2D targRb;
    private ShipHumanoid targProps;
    [SerializeField] private Vector3 evadePos = Vector3.zero;
    private Vector3 retPosition;
    private Vector3 baseTrUp;
    private float veloc4;
    private float findTargetTimer;
    private float sortieLengthTimer;
    private float rocketTimer;
    private float evadeTimer;
    private float curSpd;
    private float enemyIsTooCloseEvadeTimer;
    private int currentRockets;
    private int evadeVSI; // indicator whether falling or rising
    private bool evading = false;
    private bool returningToBaseAlt = false;
    private bool kamikazeMode = false;

    private void Start()
    {
        thisRb = GetComponent<Rigidbody2D>();
        yBaseAltitude = 20f;
        evadeYAltitude = yBaseAltitude + 10f;
        baseTrUp = transform.up;
        currentRockets = amountOfRockets;
        curSpd = 0;
    }


    private void Update()
    {
        if (!kamikazeMode)
        {
            if (currentRockets <= 0)
            {
                kamikazeMode = true;
                gameObject.layer = LayerMaskExtensions.layermask_to_layer(whatAreOurProjectiles);
                GetComponent<Collider2D>().isTrigger = true;
            }

            curSpd = Mathf.SmoothDamp(curSpd, speed, ref veloc4, 6f);

            if (returningToBaseAlt)
            {
                transform.up = Vector3.MoveTowards(transform.up, retPosition, Random.Range(0.07f, 0.1f));

                if (transform.position.y >= yBaseAltitude)
                {
                    returningToBaseAlt = false;
                    enemyIsTooCloseEvadeTimer = 0;
                }

                return;
            }

            if (transform.position.y <= waterLevel + 4f && !returningToBaseAlt)
            {
                returningToBaseAlt = true;
                evading = false;
                retPosition = new Vector3(transform.up.x * 30f, yBaseAltitude + Random.Range(4f, 6f), transform.position.z);
            }

            if (currentRockets == 0)
            {
                currentRockets = -1;
                LogUtils.DebugLog("OUT OF ROCKETS!");
            }

            if (evading == true)
            {
                if (Time.time > evadeTimer)
                {
                    evading = false;
                    return;
                }

                if ((target.position - transform.position).magnitude <= aircraftEscapeRange)
                {
                    enemyIsTooCloseEvadeTimer += Time.deltaTime;

                    if (enemyIsTooCloseEvadeTimer >= 5f)
                    {
                        LogUtils.DebugLog("Too close!");
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

            if (target != null)
            {
                if((target.position - transform.position).magnitude >= targetCheckRadius)
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
        else
        {
            if (target)
            {
                if ((target.position - transform.position).magnitude >= targetCheckRadius)
                {
                    target = null;
                    return;
                }

                Vector3 dist = (target.position - transform.position).normalized;
                transform.up = Vector3.MoveTowards(transform.up, dist, 0.04f);

                curSpd = Mathf.SmoothDamp(curSpd, speed * 1.4f, ref veloc4, 7f);
            }

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
        }
    }

    private void FixedUpdate()
    {
        thisRb.velocity = transform.up * curSpd * Time.fixedDeltaTime;
    }

    private bool CheckIfLookingAtTarget()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(rocketBay.position, lookCheckRadius, transform.up, 50f, whatIsTarget);

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

    private void FireRocket()
    {
        if (target)
        {
            Transform rocketClone = Instantiate(rocketPrefab, rocketBay.position, transform.rotation) as Transform;
            rocketClone.up = transform.up;
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
            LogUtils.DebugLog(" Index: " + i + " Name: " + hit[i].name + " Dist: " + (hit[i].transform.position - transform.position).magnitude);
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

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (kamikazeMode)
        {
            Transform particleClone = Instantiate(onHitParticlePrefab, transform.position, Quaternion.identity) as Transform;
            Destroy(particleClone.gameObject, 1f);
            Destroy(gameObject);
        }
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
        Gizmos.DrawWireSphere(rocketBay.position, lookCheckRadius);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(evadePos, 4f);
    }
}
#pragma warning restore 0649