using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
[RequireComponent(typeof(Rigidbody2D))]
public class NormalShipWeapons : MonoBehaviour
{
    [Header("Set by script, shown for debug purposes")]
    [SerializeField] private Transform target;
    [SerializeField] private Rigidbody2D thisRb;
    [Header("Properties")]
    [SerializeField] public UnitLayerMask whatUnitsToTarget;
    [SerializeField] public LayerMask whatAreOurProjectiles;
    [SerializeField] public LayerMask whatIsTarget;
    [SerializeField] private List<Transform> yeet;
    [SerializeField] private Transform missileFirePoints;
    [SerializeField] private Transform cannonFirePoint;
    [SerializeField] private Transform rocketPrefab;
    [SerializeField] private Transform shellPrefab;
    [SerializeField] private Material targMat;
    [SerializeField] public float rocketFireAngle;
    [SerializeField] private float speed;
    [SerializeField] private float rocketDelay;
    [SerializeField] private float cannonDelay;
    [SerializeField] private float rocketYOffset;
    [SerializeField] private float targetCheckRadius;
    [Header("Debug Properties")]
    [SerializeField] private bool showTargetAsRed = false;

    private Vector3 veloc1;
    private Material normMat;
    private float rocketTimer;
    private float cannonTimer;

    private void Start()
    {
        normMat = transform.GetComponent<MeshRenderer>().material;
        thisRb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        FindTarget();

        if (Time.time > rocketTimer)
        {
            FireRocket();
        }

        if (target)
        {
            if (Time.time > cannonTimer)
            {
                FireCannon();
            }
        }
    }

    private void FixedUpdate()
    {
        if (target && (target.GetComponent<ShipHumanoid>().whatAmI != UnitType.Aircraft || target.GetComponent<ShipHumanoid>().whatAmI != UnitType.Submarine))
        {
            thisRb.velocity = Vector3.SmoothDamp(thisRb.velocity, Vector3.zero, ref veloc1, 2f);
        }
        else
        {
            thisRb.velocity = transform.right * speed * Time.fixedDeltaTime;
        }
    }

    private void Test()
    {
        Debug.Log(UnitLayerMask.CheckIfUnitIsInMask(UnitType.Ship, whatUnitsToTarget));
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

    private void FireCannon()
    {
        Transform shellClone = Instantiate(shellPrefab, cannonFirePoint.position, Quaternion.identity) as Transform;
        shellClone.gameObject.layer = whatAreOurProjectiles.layermask_to_layer();
        shellClone.right = -(shellClone.position - target.position).normalized;
        Shell shell = shellClone.GetComponent<Shell>();
        cannonTimer = Time.time + cannonDelay;
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
    }
}
#pragma warning restore 0649