using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public abstract class GuidedProjectile : ProjectileBase
{
    [Header("Guided projectile properties")]
    [SerializeField] public Transform target;
    [SerializeField] public LayerMask whatIsTarget;
    [SerializeField] protected UnitLayerMask whatUnitsToTarget;
    [SerializeField] protected float targetCheckRadius;
    [SerializeField] [Range(0.01f, 0.08f)] protected float rotationSmoothing;

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

    protected bool CheckIfLookingAtTarget()
    {
        RaycastHit2D ray = Physics2D.Raycast(transform.position, transform.up, 100f, whatIsTarget);
        Debug.DrawRay(transform.position, transform.up);

        if (ray.collider)
        {
            if (ray.collider.gameObject == target.gameObject)
            {
                return true;
            }
        }

        return false;
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, targetCheckRadius);
    }
}
#pragma warning restore 0649