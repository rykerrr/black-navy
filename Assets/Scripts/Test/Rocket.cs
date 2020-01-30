using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
[RequireComponent(typeof(Rigidbody2D))]
public class Rocket : MonoBehaviour
{
    [Header("Set by script, shown for debug purposes")]
    [SerializeField] private Transform target;
    [SerializeField] private Rigidbody2D thisRb;
    [Header("Properties")]
    [SerializeField] public UnitLayerMask whatUnitsToTarget;
    [SerializeField] public LayerMask whatIsTarget;
    [SerializeField] private Transform afterBurner;
    [SerializeField] private Transform onHitParticlePrefab;
    [SerializeField] private Material targMat;
    [SerializeField] private float waterLevel;
    [SerializeField] private float boosterLength;
    [SerializeField] private float intermStageLength;
    [SerializeField] private float maxDistToTarget;
    [SerializeField] private float fuel;
    [SerializeField] private float rotationSmTime;
    [SerializeField] private float speed;
    [SerializeField] private float targetCheckRadius;
    [Header("Debug Properties")]
    [SerializeField] private bool showTargetAsRed;

    private Material normMat;
    private Vector3 veloc1 = Vector3.zero;
    private Vector2 veloc3 = Vector2.zero;
    private float veloc2;
    private bool attackingStage = false;
    private bool boosting = false;
    private bool intermStage = false;

    private void Start()
    {
        normMat = transform.GetComponent<MeshRenderer>().material;
        thisRb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Time.timeScale >= 0.1f)
        {
            if (IsOutOfWater())
            {
                if (intermStage || attackingStage)
                {
                    bool foundTarget;

                    if (attackingStage)
                    {
                        if (fuel >= 0)
                        {
                            fuel--;
                        }
                    }

                    if (target == null)
                    {
                        foundTarget = FindTarget();
                    }
                    else
                    {
                        if ((target.position - transform.position).magnitude >= maxDistToTarget)
                        {
                            foundTarget = false;
                        }
                        else
                        {
                            foundTarget = true;
                        }
                    }

                    if (foundTarget)
                    {
                        transform.up = Vector3.SmoothDamp(transform.up, (target.position - transform.position).normalized, ref veloc1, rotationSmTime);
                    }
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (boosting)
        {
            thisRb.velocity = speed * transform.up * Time.fixedDeltaTime * 1.5f;
        }
        else
        {
            if (IsOutOfWater())
            {
                if (attackingStage)
                {
                    if (fuel >= 0 && target)
                    {
                        thisRb.velocity = speed * transform.up * Time.fixedDeltaTime;
                    }
                }
            }
        }
    }

    public void BoostStage()
    {
        StartCoroutine(Booster());
    }

    private IEnumerator Booster()
    {
        boosting = true;
        MeshRenderer abSprite = afterBurner.GetComponent<MeshRenderer>();
        abSprite.enabled = true;
        yield return new WaitForSeconds(boosterLength);
        abSprite.enabled = false;
        thisRb.velocity = Vector3.zero;
        thisRb.AddForce(transform.up * speed * 0.03f / 1.5f, ForceMode2D.Impulse); // changed Time.deltaTime to 0.03f due to weird physics behaviour on different speeds, change it back if it's not the cause
        intermStage = true;
        boosting = false;
        yield return new WaitForSeconds(intermStageLength);
        intermStage = false;
        attackingStage = true;
    }

    private bool IsOutOfWater()
    {
        if (transform.position.y <= waterLevel)
        {
            thisRb.gravityScale = Mathf.SmoothDamp(thisRb.gravityScale, 0.004f, ref veloc2, Random.Range(0.4f, 1.5f));
            thisRb.velocity = Vector2.SmoothDamp(thisRb.velocity, thisRb.velocity / 2f, ref veloc3, Random.Range(0.4f, 1.2f));
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
#pragma warning restore 0649