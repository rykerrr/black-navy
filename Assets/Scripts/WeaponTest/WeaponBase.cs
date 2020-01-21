using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
#pragma warning disable 0649
public abstract class WeaponBase : Poolable
{
    [SerializeField] private UnitLayerMask whatUnitsToTarget;
    [SerializeField] protected Material t1Mat;
    [SerializeField] protected Material t2Mat;
    [SerializeField] private float targetCheckRadius;
    [SerializeField] private float targetCheckDelay;

    public Transform owner;
    public Transform projectilePrefab;
    public Transform spawnLocation;
    public float delayBetweenFire;
    public float inaccuracyOffset;
    public float reloadTime;
    public float lookCheckRadius;
    public float lookCheckRange;
    public int maxAmmo;

    [HideInInspector] public LayerMask whatIsTarget;
    [HideInInspector] public LayerMask whatAreOurProjectiles;
    [HideInInspector] public Transform target;
    [HideInInspector] public Rigidbody2D ownerRb;

    [HideInInspector] public int currentAmmo;

    protected UnitHumanoid targHumanoid;
    protected Rigidbody2D targRb;
    protected float findTargetTimer;
    protected float fireTimer;

    public abstract FireState Fire();

    public void LoadAmmo()
    {
        currentAmmo = maxAmmo;
    }

    protected bool CheckIfLookingAtTarget(float range)
    {
        if (target)
        {
            RaycastHit2D[] hits = Physics2D.CircleCastAll(spawnLocation.position, lookCheckRadius, owner.up, range, whatIsTarget);
            Debug.DrawRay(transform.position, (target.position - transform.position).normalized * 99f, Color.cyan);

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

    protected virtual bool FindTarget()
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

                        foreach (var obj in hit)
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
    }
}

public enum FireState { OutOfAmmo, Fired, OnDelay, Failed }
#pragma warning restore 0649