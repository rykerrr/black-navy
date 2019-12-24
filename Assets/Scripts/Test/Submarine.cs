using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
[RequireComponent(typeof(Rigidbody2D))]
public class Submarine : MonoBehaviour
{
    [Header("Set by script, shown for debug purposes")]
    [SerializeField] private Transform target;
    [SerializeField] private Rigidbody2D thisRb;
    [Header("Properties")]
    [SerializeField] public UnitLayerMask whatUnitsToTarget;
    [SerializeField] public LayerMask whatAreOurProjectiles;
    [SerializeField] public LayerMask whatIsTarget;
    [SerializeField] private Transform torpedoBay;
    [SerializeField] private Transform missileFirePoints;
    [SerializeField] private Transform rocketPrefab;
    [SerializeField] private Transform torpedoPrefab;
    [SerializeField] private Material targMat;
    [SerializeField] public float rocketFireAngle;
    [SerializeField] private float speed;
    [SerializeField] private float rocketDelay;
    [SerializeField] private float torpedoDelay;
    [SerializeField] private float rocketYOffset;
    [SerializeField] private float targetCheckRadius;
    [Header("Debug Properties")]
    [SerializeField] private bool showTargetAsRed = false;

    private Material normMat;
    private Vector3 veloc1;
    private float rocketTimer;
    private float torpedoTimer;

    private void Start()
    {
        normMat = transform.GetComponent<MeshRenderer>().material;
        thisRb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Time.time > rocketTimer)
        {
            if (FindTarget() == true)
            {
                FireRocket();
            }
        }
        if (Time.time > torpedoTimer)
        {
            FireTorpedo();
        }
    }

    private void FixedUpdate()
    {
        if (target)
        {
            thisRb.velocity = Vector3.SmoothDamp(thisRb.velocity, Vector3.zero, ref veloc1, 2f);
        }
        else
        {
            thisRb.velocity = transform.up * speed * Time.fixedDeltaTime;
        }
    }

    private void FireRocket()
    {
        int missileSpawnIndex = Random.Range(0, missileFirePoints.childCount - 1);
        Transform missileSpawnTransf = missileFirePoints.GetChild(missileSpawnIndex);
        Transform rocketClone = Instantiate(rocketPrefab, new Vector3(missileSpawnTransf.position.x, missileSpawnTransf.position.y + rocketYOffset, missileSpawnTransf.position.z), Quaternion.identity) as Transform;
        rocketClone.localEulerAngles = new Vector3(0f, 0f, rocketFireAngle);
        int layerValue = whatAreOurProjectiles.layermask_to_layer();
        rocketClone.gameObject.layer = layerValue;
        Rocket rocket = rocketClone.GetComponent<Rocket>();
        rocket.whatIsTarget = whatIsTarget;
        rocket.BoostStage();
        rocketTimer = rocketDelay + Time.time;
    }

    private void FireTorpedo()
    {
        Transform torpClone = Instantiate(torpedoPrefab, torpedoBay.position, Quaternion.identity) as Transform;
        Torpedo torp = torpClone.GetComponent<Torpedo>();
        torp.whatIsTarget = whatIsTarget;
        int layerValue = whatAreOurProjectiles.layermask_to_layer();
        torpClone.gameObject.layer = layerValue;

        if (CheckIfLookingAtTarget(out Transform subTarget) == true)
        {
            if (subTarget)
            {
                torp.target = subTarget;
            }
        }

        torpClone.up = transform.up;

        torpedoTimer = torpedoDelay + Time.time;
    }

    private bool CheckIfLookingAtTarget(out Transform targ)
    {
        RaycastHit2D ray = Physics2D.CircleCast(transform.position, 5f, transform.up, 150f, whatIsTarget);

        if (ray.collider)
        {
            targ = ray.collider.transform;
            Debug.Log("Found a nigger!");
            return true;
        }

        targ = null;
        return false;
    }

    private bool FindTarget()
    {
        if (showTargetAsRed)
        {
            if (target)
            {
                target.GetComponent<MeshRenderer>().material = normMat;
            }
        }


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

            if (showTargetAsRed)
            {
                target.GetComponent<MeshRenderer>().material = targMat;
            }

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
        Gizmos.DrawWireCube(new Vector3((transform.position.x + 150f)/4, transform.position.y), new Vector2(150f, 10f));
    }
}
#pragma warning restore 0649