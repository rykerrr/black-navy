using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
[RequireComponent(typeof(Rigidbody2D))]
public class Torpedo : MonoBehaviour
{
    [Header("Set by script, shown for debug purposes")]
    [SerializeField] public Transform target;
    [SerializeField] private Rigidbody2D thisRb;
    [Header("Properties")]
    [SerializeField] private UnitLayerMask whatUnitsToTarget;
    [SerializeField] public LayerMask whatIsTarget;
    [SerializeField] private Transform onHitParticlePrefab;
    [SerializeField] private Material targMat;
    [SerializeField] private float waterLevel;
    [SerializeField] private float rotationSmTime;
    [SerializeField] private float speed;
    [SerializeField] private float targetCheckRadius;
    [Header("Debug Properties")]
    [SerializeField] private bool showTargetAsRed;

    private Material normMat;
    private Vector3 veloc1 = Vector3.zero;
    private float veloc2;
    private float veloc3;

    private void Start()
    {
        normMat = transform.GetComponent<MeshRenderer>().material;
        thisRb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!target)
        {
            FindTarget();
        }

        if (Time.timeScale >= 0.1f && !IsOutOfWater())
        {
            if (target)
            {
                transform.up = Vector3.SmoothDamp(transform.up, (target.position - transform.position).normalized, ref veloc1, rotationSmTime);
            }
        }
    }

    private void FixedUpdate()
    {
        if (!IsOutOfWater())
        {
            thisRb.velocity = transform.up * speed * Time.fixedDeltaTime;
        }
    }

    private bool IsOutOfWater()
    {
        if (transform.position.y >= waterLevel)
        {
            thisRb.gravityScale = Mathf.SmoothDamp(thisRb.gravityScale, 3f, ref veloc2, Random.Range(1f, 1.5f));
            return true;
        }
        else
        {
            thisRb.gravityScale = Mathf.SmoothDamp(thisRb.gravityScale, 0f, ref veloc3, Random.Range(4f, 5.5f));
            return false;
        }
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Transform particleClone = Instantiate(onHitParticlePrefab, transform.position, Quaternion.identity) as Transform;
        Destroy(particleClone.gameObject, 1f);
        Destroy(gameObject);
    }
}
#pragma warning disable 0649