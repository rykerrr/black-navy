using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
#pragma warning disable 0649
public abstract class WeaponBase : Poolable
{
    public bool unlocked = false;

    [SerializeField] private UnitLayerMask whatUnitsToTarget;
    [SerializeField] protected AudioSource afterEffect;
    [SerializeField] protected Material t1Mat;
    [SerializeField] protected Material t2Mat;
    [SerializeField] private float targetCheckRadius;
    [SerializeField] private float targetCheckDelay;
    [SerializeField] protected int shopCost;

    public WeaponType typeOfWeapon;
    public MountType typeOfMountRequired;
    public Transform owner;
    public Transform projectilePrefab;
    public Transform spawnLocation;
    public float delayBetweenFire;
    public float inaccuracyOffset;
    public float reloadTime;
    public float lookCheckRadius;
    public float lookCheckRange;
    public int damage;
    public int maxAmmo;

    public UnitLayerMask whatUnitsCanBeTargetted => whatUnitsToTarget;
    public int ShopCost => shopCost;

    [HideInInspector] public LayerMask whatIsTarget;
    [HideInInspector] public LayerMask whatAreOurProjectiles;
    [SerializeField] public Transform target;
    [HideInInspector] public Rigidbody2D ownerRb;

    [HideInInspector] public int currentAmmo;

    protected UnitHumanoid targHumanoid;
    protected AudioSource fireSound;

    //protected SoundManager soundMngr;
    protected Rigidbody2D targRb;
    protected float findTargetTimer;
    protected float fireTimer;

    public abstract FireState Fire();

    protected virtual void Awake()
    {
        //soundMngr = SoundManager.Instance;
        fireSound = GetComponent<AudioSource>();

        if (fireSound == null)
        {
            if (transform.parent)
            {
                LogUtils.DebugLogWarning("No audio source on weapon? | " + transform.parent.name + " , " + name);
            }
        }
    }

    public void LoadAmmo()
    {
        currentAmmo = maxAmmo;
    }

    protected bool CheckIfLookingAtTarget(float range)
    {
        if (target)
        {
            if (!target.gameObject.activeInHierarchy)
            {
                target = null;
                return false;
            }

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

    protected virtual bool FindTarget(float addTime = 0f)
    {
        LogUtils.DebugLog("yes");
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
                            if ((en.transform.position - transform.position).magnitude <= targetCheckRadius && en.gameObject.activeInHierarchy)
                            {
                                availableTargets.Add(en);
                            }
                        }
                    }
                    catch (System.Exception e)
                    {
                        LogUtils.DebugLog(e);
                        LogUtils.DebugLog(hit + " | " + hit.Count);

                        foreach (var obj in hit)
                        {
                            LogUtils.DebugLog(obj.name);
                        }
                    }
                }
            }


            /*for (int i = 0; i < availableTargets.Count; i++)
            {
                LogUtils.DebugLog(" Index: " + i + " Name: " + hit[i].name + " Dist: " + (hit[i].transform.position - transform.position).magnitude);
            }*/

            if (availableTargets.Count > 0)
            {
                Collider2D targetBase;
                if (targetBase = availableTargets.Find(obj => obj.GetComponent<Base>()))
                {
                    availableTargets.Remove(targetBase);
                }

                availableTargets = availableTargets.OrderBy(en => Mathf.Abs((en.transform.position - transform.position).magnitude)).ToList();

                if (targetBase)
                {
                    availableTargets.Add(targetBase);
                    LogUtils.DebugLog("found base + base is target: " + availableTargets[availableTargets.Count - 1]);
                }

                target = availableTargets[0].transform;
                LogUtils.DebugLog(availableTargets.Count);
                LogUtils.DebugLog(target);
                targHumanoid = target.GetComponent<UnitHumanoid>();
                targRb = target.GetComponent<Rigidbody2D>();

                return true;
            }

            /*if (hit != null)
            {
                target = hit.transform;
                return true;
            }*/

            findTargetTimer = targetCheckDelay + Time.time + addTime;
            return false;
        }
        else return false;
    }

    // https://www.forrestthewoods.com/blog/solving_ballistic_trajectories/
    protected int SolveBallisticArc(Vector3 proj_pos, float proj_speed, Vector3 target, float gravity, out Vector3 s0, out Vector3 s1)
    {
        s0 = Vector3.zero;
        s1 = Vector3.zero;

        Vector3 diff = target - proj_pos;
        Vector3 diffXZ = new Vector3(diff.x, 0f, diff.z);
        float groundDist = diffXZ.magnitude;

        float speed2 = proj_speed * proj_speed;
        float speed4 = proj_speed * proj_speed * proj_speed * proj_speed;
        float y = diff.y;
        float x = groundDist;
        float gx = gravity * x;

        float root = speed4 - gravity * (gravity * x * x + 2 * y * speed2);

        // No solution
        if (root < 0)
        {
            LogUtils.DebugLog("no solution");
            return 0;
        }

        root = Mathf.Sqrt(root);

        float lowAng = Mathf.Atan2(speed2 - root, gx);
        float highAng = Mathf.Atan2(speed2 + root, gx);
        int numSolutions = lowAng != highAng ? 2 : 1;

        Vector3 groundDir = diffXZ.normalized;
        s0 = groundDir * Mathf.Cos(lowAng) * proj_speed + Vector3.up * Mathf.Sin(lowAng) * proj_speed;
        if (numSolutions > 1)
            s1 = groundDir * Mathf.Cos(highAng) * proj_speed + Vector3.up * Mathf.Sin(highAng) * proj_speed;

        return numSolutions;
    }

    protected bool VecApprox(Vector2 vec1, Vector2 vec2)
    {
        if (Mathf.Abs(vec1.x - vec2.x) < 0.005f && Mathf.Abs(vec1.x - vec2.x) < 0.005f)
        {
            return true;
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
public enum WeaponType { SurfaceToSurfaceMissile, AirToAirMissile, Autocannon, NavalArtillery, AntiAirCloseInWeaponSystem, UnguidedMissile, SmartBomb, UnguidedBomb, Torpedo, DepthCharge }
#pragma warning restore 0649