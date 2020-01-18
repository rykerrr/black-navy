using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
[RequireComponent(typeof(Rigidbody2D))]
public class bombr : MonoBehaviour
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
    [SerializeField] private Transform bombBay;
    [SerializeField] private Transform bombPrefab;
    [SerializeField] private float waterLevel;
    [SerializeField] private float timeToTakeOffFully;
    [SerializeField] private float speed;
    [SerializeField] private float rotationSmTime;
    [SerializeField] private float targetCheckDelay;
    [SerializeField] private float distanceBeforeBomb;
    [SerializeField] private float xDistanceBeforeDive;
    [SerializeField] private float targetCheckRadius;
    [SerializeField] private float dropDelay;

    // properties
    public float Speed => speed;

    private Rigidbody2D targRb;
    private Vector3 veloc1 = Vector3.zero;
    private Vector3 veloc2 = Vector3.zero;
    private Vector3 veloc3 = Vector3.zero;
    private Vector3 baseTrUp;
    private Vector3 retPosition;
    private float veloc4;
    private float findTarget;
    private float curSpd;
    private float dropTimer;
    private float evasionTimer;
    private bool diving = false;
    private bool evading = false;
    private bool returningToBaseAlt = false;
    private bool takenOff = false;

    private void OnValidate()
    {
        yBaseAltitude = transform.position.y;
        evadeYAltitude = yBaseAltitude + 10f;
    }

    private void Start()
    {
        thisRb = GetComponent<Rigidbody2D>();
        yBaseAltitude = transform.position.y;
        baseTrUp = transform.up;
        curSpd = speed;
        yBaseAltitude = 55f;
        evadeYAltitude = yBaseAltitude + 10f;
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
            curSpd = Mathf.SmoothDamp(curSpd, speed, ref veloc4, timeToTakeOffFully / 3f);
        }
        else
        {
            if (target)
            {
                if (transform.position.y <= waterLevel + 4f && !returningToBaseAlt)
                {
                    Debug.Log("returning to base alt is true");
                    returningToBaseAlt = true;
                    retPosition = new Vector3(transform.up.x * 70, yBaseAltitude + Random.Range(4f, 6f), transform.position.z);
                }

                if (Time.time > dropTimer && !evading)
                {
                    if (Mathf.Abs(target.position.x - transform.position.x) <= xDistanceBeforeDive)
                    {
                        diving = true;
                    }
                }

                if (evading)
                {
                    if (Time.time > evasionTimer)
                    {
                        Debug.Log("Yeet");
                        evading = false;
                    }

                    if (transform.position.y >= evadeYAltitude)
                    {
                        if (target)
                        {
                            Debug.Log("Higher than evadeYAltitude");
                            curSpd = Mathf.SmoothDamp(curSpd, speed * 2.4f, ref veloc4, 2f);
                            diving = true;
                            evading = false;
                            return;
                        }

                        returningToBaseAlt = true;
                        retPosition = new Vector3(transform.up.x * 30f, yBaseAltitude);
                        return;
                    }

                    transform.up = Vector3.SmoothDamp(transform.up, new Vector2(transform.up.x * 40f, evadeYAltitude), ref veloc2, rotationSmTime * 4f);
                    //transform.localEulerAngles = Vector3.SmoothDamp(transform.localEulerAngles, new Vector3(0f, 0f, -90f), ref veloc3, rotationSmTime);
                }

                if (diving)
                {
                    curSpd = Mathf.SmoothDamp(curSpd, speed / 1.3f, ref veloc4, 2f);
                    Vector3 dist = (new Vector3(target.position.x, target.position.y + Random.Range(6f, 12f), target.position.z) - transform.position).normalized;
                    transform.up = Vector3.SmoothDamp(transform.up, dist, ref veloc1, rotationSmTime / 1.6f);

                    if ((target.position - transform.position).magnitude <= distanceBeforeBomb)
                    {
                        DropBomb();
                        Debug.Log("Should be evading?");
                        diving = false;
                        evading = true;
                        evasionTimer = Time.time + 10f;
                    }
                }
            }
            else
            {
                if (Time.time >= findTarget)
                {
                    FindTarget();
                    findTarget = Time.time + targetCheckDelay;

                    if (target)
                    {
                        targRb = target.GetComponent<Rigidbody2D>();
                    }
                }

                if(transform.position.y >= yBaseAltitude)
                {
                    transform.up = Vector3.MoveTowards(transform.up, new Vector2(transform.up.x, 0f), 0.01f);
                }
                else
                {
                    returningToBaseAlt = true;
                    retPosition = new Vector3(transform.up.x * 30f, yBaseAltitude);
                }
            }
        }

    }

    private void FixedUpdate()
    {
        thisRb.velocity = transform.up * curSpd * Time.fixedDeltaTime;
    }

    private void DropBomb()
    {
        if (target)
        {
            if (bombPrefab.GetComponent<GuidedBomb>())
            {
                float yOffsetDelay = Mathf.Clamp(1 / (30 / Mathf.Abs(transform.position.y)), 0f, 1f);
                Transform bombClone = Instantiate(bombPrefab, bombBay.position, transform.rotation) as Transform;
                Rigidbody2D bombRb = bombClone.GetComponent<Rigidbody2D>();
                GuidedBomb bomb = bombClone.GetComponent<GuidedBomb>();
                int layerValue = whatAreOurProjectiles.layermask_to_layer();
                bombClone.gameObject.layer = layerValue;
                bomb.target = target;
                bomb.timeBeforeBoosters = Mathf.Clamp(bomb.timeBeforeBoosters - yOffsetDelay, 0.6f, 2f);
                Debug.Log(bomb.timeBeforeBoosters + " | " + yOffsetDelay);
                bomb.whatIsEnemy = whatIsTarget;
                bombRb.velocity = thisRb.velocity;
            }
            else if (bombPrefab.GetComponent<Torpedo>())
            {
                Transform torpClone = Instantiate(bombPrefab, bombBay.position, Quaternion.identity) as Transform;
                Torpedo torp = torpClone.GetComponent<Torpedo>();
                torp.whatIsTarget = whatIsTarget;
                int layerValue = whatAreOurProjectiles.layermask_to_layer();
                torpClone.gameObject.layer = layerValue;

                torpClone.up = transform.up;
            }
        }
        else Debug.Log("No target");

        dropTimer = dropDelay + Time.time;
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
        Gizmos.DrawWireSphere(transform.position, xDistanceBeforeDive);
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, distanceBeforeBomb);
    }
}
#pragma warning disable 0649