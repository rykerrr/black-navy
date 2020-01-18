using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
[RequireComponent(typeof(UnitHumanoid))]
[RequireComponent(typeof(Rigidbody2D))]
public abstract class UnitBase : Poolable
{
    [Header("Base unit properties")]
    [SerializeField] protected Rigidbody2D thisRb;
    [SerializeField] public UnitLayerMask whatUnitsToTarget;
    [SerializeField] public LayerMask whatAreOurProjectiles;
    [SerializeField] public LayerMask whatIsTarget;
    [SerializeField] protected Transform firePoint;
    [SerializeField] protected float lookCheckRadius;
    [SerializeField] private float targetCheckDelay;
    [SerializeField] protected float targetCheckRadius;
    [SerializeField] protected float yBaseAltitude;
    [SerializeField] protected float speed;

    public float BaseAltitude => yBaseAltitude;

    [Header("Base unit debug")]
    protected Rigidbody2D targRb;
    protected UnitHumanoid targHumanoid;
    [SerializeField] protected Transform target;
    private float findTargetTimer;
    protected float waterLevel;
    protected float curSpd;

    protected virtual void Awake()
    {
        waterLevel = GameConfig.Instance.WaterLevel;
        thisRb = GetComponent<Rigidbody2D>();
    }

    // maybe add water as well?
    protected bool CheckIfLookingAtTarget(float range)
    {
        if (target)
        {
            RaycastHit2D[] hits = Physics2D.CircleCastAll(firePoint.position, lookCheckRadius, transform.up, range, whatIsTarget);

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
        else
        {
            return false;
        }
    }
    protected bool FindTarget()
    {
        if (Time.time > findTargetTimer)
        {
            List<Collider2D> hit = (Physics2D.OverlapCircleAll(transform.position, targetCheckRadius, whatIsTarget)).ToList();
            List<Collider2D> availableTargets = new List<Collider2D>();

            foreach (Collider2D en in hit)
            {
                if (en)
                {
                    try
                    {
                        if (UnitLayerMask.CheckIfUnitIsInMask(en.GetComponent<UnitHumanoid>().type, whatUnitsToTarget) == true)
                        {
                            if ((en.transform.position - transform.position).magnitude <= targetCheckRadius)
                            {
                                availableTargets.Add(en);
                            }
                        }
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log(e);
                        Debug.Log(hit + " | " + hit.Count);

                        foreach(var obj in hit)
                        {
                            Debug.Log(obj.name);
                        }
                    }
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
                targRb = target.GetComponent<Rigidbody2D>();
                targHumanoid = target.GetComponent<UnitHumanoid>();

                return true;
            }

            /*if (hit != null)
            {
                target = hit.transform;
                return true;
            }*/

            findTargetTimer = targetCheckDelay + Time.time;
            return false;
        }
        else return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, targetCheckRadius);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, lookCheckRadius);
    }

    private void OnDisable()
    {
        target = null;
        curSpd = 0f;
        findTargetTimer = 0f;
        thisRb.velocity = Vector2.zero;
    }
}
#pragma warning restore 0649