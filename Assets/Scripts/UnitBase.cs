using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
[RequireComponent(typeof(UnitHumanoid))]
[RequireComponent(typeof(Rigidbody2D))]
public class UnitBase : Poolable
{
    [Header("Base unit properties")]
    [SerializeField] protected Rigidbody2D thisRb;
    [SerializeField] public UnitLayerMask whatUnitsToTarget;
    [SerializeField] public LayerMask whatAreOurProjectiles;
    [SerializeField] public LayerMask whatIsTarget;
    [SerializeField] private Transform lookCheckPoint;
    [SerializeField] private float lookCheckRadius;
    [SerializeField] private float targetCheckRadius;
    [SerializeField] protected float speed;

    protected Transform target;
    protected float waterLevel = GameConfig.Instance.WaterLevel;
    protected float curSpd;

    // maybe add water as well?
    protected bool CheckIfLookingAtTarget()
    {
        if(target)
        {
            RaycastHit2D[] hits = Physics2D.CircleCastAll(lookCheckPoint.position, lookCheckRadius, transform.up, 50f, whatIsTarget);

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
}
#pragma warning restore 0649