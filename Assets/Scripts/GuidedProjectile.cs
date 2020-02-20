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
    [SerializeField] protected float rotationSmoothing;

    protected Rigidbody2D targRb;
    protected UnitHumanoid targHumanoid;

    protected bool FindTarget()
    {
        List<Collider2D> hit = (Physics2D.OverlapCircleAll(transform.position, targetCheckRadius, whatIsTarget)).ToList();
        List<Collider2D> availableTargets = new List<Collider2D>();

        foreach (Collider2D en in hit)
        {
            try
            {
                if (UnitLayerMask.CheckIfUnitIsInMask(en.GetComponent<UnitHumanoid>().type, whatUnitsToTarget) == true)
                {
                    if ((en.transform.position - transform.position).magnitude <= targetCheckRadius && en.gameObject.activeInHierarchy)
                    {
                        availableTargets.Add(en);
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.Log(e);
                Debug.Log(hit + " | " + hit.Count);

                foreach (var obj in hit)
                {
                    Debug.Log(obj.name);
                }
            }
        }



        /*for (int i = 0; i < availableTargets.Count; i++)
        {
            Debug.Log(" Index: " + i + " Name: " + hit[i].name + " Dist: " + (hit[i].transform.position - transform.position).magnitude);
        }*/

        if (availableTargets.Count > 0)
        {
            availableTargets = availableTargets.OrderBy(en => Mathf.Abs((en.transform.position - transform.position).magnitude)).ToList();

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