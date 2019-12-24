using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
#pragma warning disable 0649
public class Missile : MonoBehaviour
{
    [Header("Set by script, shown for debug purposes")]
    [SerializeField] public Transform target;
    [SerializeField] private Rigidbody2D thisRb;
    [Header("Properties")]
    [SerializeField] private UnitLayerMask whatUnitsToTarget;
    [SerializeField] public LayerMask whatIsTarget;
    [SerializeField] private Transform onHitParticlePrefab;
    [SerializeField] private float waterLevel;
    [SerializeField] private float targetCheckRadius;
    [SerializeField] private float rotationSmTime;
    [SerializeField] private float speed;
    [SerializeField] private float timeBeforeFire;
    [Header("Debug Properties")]
    [SerializeField] private bool showTargetAsRed;

    private Material normMat;
    private Vector3 veloc1 = Vector3.zero;
    private Vector2 veloc3 = Vector2.zero;
    private float veloc2;
    [SerializeField] float eulerAngle;
    float fireTimer;

    private void Start()
    {
        fireTimer = Time.time + timeBeforeFire;
        normMat = transform.GetComponent<MeshRenderer>().material;
        thisRb = GetComponent<Rigidbody2D>();
    }

    //Vector2 dir = target.position - transform.position;
    //Debug.DrawRay(transform.position, dir, Color.magenta);
    //angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    //Debug.Log(transform.rotation.eulerAngles.z + " | " + transform.rotation.z* Mathf.Rad2Deg);
    //testAngle = Mathf.Abs(transform.rotation.z* Mathf.Rad2Deg) - Mathf.Abs(angle);
    //Quaternion angleAxis = Quaternion.AngleAxis(angle, Vector3.forward);
    //eulerAngle = angleAxis.eulerAngles.z;
    //transform.rotation = Quaternion.Slerp(transform.rotation, angleAxis, 0.05f);

    // Update is called once per frame
    private void Update()
    {
        if (Time.time > timeBeforeFire)
        {
            if (IsOutOfWater())
            {
                if (Time.timeScale >= 0.1f)
                {
                    if (target)
                    {
                        Vector2 dir = target.position - transform.position;
                        Debug.DrawRay(transform.position, dir, Color.magenta);
                        Debug.DrawRay(transform.position, transform.up * 999f, Color.grey);
                        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
                        Quaternion angleAxis = Quaternion.AngleAxis(angle, Vector3.forward);
                        eulerAngle = angleAxis.eulerAngles.z;
                        transform.rotation = Quaternion.Slerp(transform.rotation, angleAxis, 1f);
                        //transform.rotation = angleAxis;
                        //transform.up = Vector3.SmoothDamp(transform.up, (target.position - transform.position).normalized, ref veloc1, rotationSmTime);
                        //angle = Vector3.Angle(transform.position, target.position);
                    }
                }
            }
        }
    }

    private void FixedUpdate()
    {
        thisRb.velocity = transform.up * speed * Time.fixedDeltaTime;
    }

    private bool IsOutOfWater()
    {
        if (transform.position.y <= waterLevel)
        {
            thisRb.gravityScale = Mathf.SmoothDamp(thisRb.gravityScale, 0.004f, ref veloc2, Random.Range(0.4f, 0.6f));
            thisRb.velocity = Vector2.SmoothDamp(thisRb.velocity, thisRb.velocity / 2f, ref veloc3, Random.Range(0.7f, 1.3f));
            return false;
        }

        return true;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Transform particleClone = Instantiate(onHitParticlePrefab, transform.position, Quaternion.identity) as Transform;
        Destroy(particleClone.gameObject, 1f);
        Destroy(gameObject);
    }
}
#pragma warning restore 0649