using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
[RequireComponent(typeof(Rigidbody2D))]
public class BallisticSubmarine : MonoBehaviour
{
    [Header("Set by script, shown for debug purposes")]
    [SerializeField] private Transform target;
    [SerializeField] private Rigidbody2D thisRb;
    [Header("Properties")]
    [SerializeField] public UnitLayerMask whatUnitsToTarget;
    [SerializeField] public LayerMask whatAreOurProjectiles;
    [SerializeField] public LayerMask whatIsTarget;
    [SerializeField] private Transform nukeFirePoint;
    [SerializeField] private Transform nukePrefab;
    [SerializeField] private Material targMat;
    [SerializeField] public float rocketFireAngle;
    [SerializeField] private float speed;
    [SerializeField] private float nukeDelay;
    [SerializeField] private float nukeYOffset;
    [SerializeField] private float targetCheckRadius;
    [Header("Debug Properties")]
    [SerializeField] private bool showTargetAsRed = false;

    private Material normMat;
    private Vector3 veloc1;
    private float nukeTimer;

    private void Start()
    {
        normMat = transform.GetComponent<MeshRenderer>().material;
        thisRb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Time.time > nukeTimer)
        {
            if (FindTarget() == true)
            {
                FireNuke();
            }
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

    private void FireNuke()
    {
        Transform nukeClone = Instantiate(nukePrefab, nukeFirePoint.position, Quaternion.identity) as Transform;
        nukeClone.localEulerAngles = new Vector3(0f, 0f, rocketFireAngle);
        int layerValue = whatAreOurProjectiles.layermask_to_layer();
        nukeClone.gameObject.layer = layerValue;
        Rocket nuke = nukeClone.GetComponent<Rocket>();
        nuke.BoostStage();
        nukeTimer = nukeDelay + Time.time;
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