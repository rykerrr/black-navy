using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class StrategicBomberThatWorksWithWeapon : AircraftBase
{
    [Header("Strategic bomber properties")]
    [SerializeField] private Transform bombDropPos;
    [SerializeField] private float bombDropRange;
    [SerializeField] private float sortieEndRange;

    [Header("Debug")]
    [SerializeField] private bool resettingSortie = false;
    [SerializeField] private float veloc1;

    [SerializeField] private bool bombing = true;
    [SerializeField] private bool returningToBaseRot = false;

    protected override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        int vsi = thisRb.velocity.y / Mathf.Abs(thisRb.velocity.y) >= 0 ? vsi = 1 : vsi = -1;

        if (vsi == 0)
        {
            Debug.Log("possible error @vsi");
            Debug.Break();
        }

        if (returningToBaseAlt)
        {
            ReturnToBaseAlt();
            return;
        }

        if (resettingSortie)
        {
            ResettingSortie();
            return;
        }

        if (returningToBaseRot)
        {
            ReturnToBaseRot();
            return;
        }

        if (!takenOff)
        {
            curSpd = Mathf.SmoothDamp(curSpd, speed, ref veloc1, timeToTakeOffFully / 3f);
        }
        else
        {
            Vector2 pos1 = new Vector2(bombDropPos.position.x + bombDropRange, transform.position.y);
            Vector2 pos2 = new Vector2(bombDropPos.position.x - bombDropRange, transform.position.y);
            bool isNearWater = CheckIfNearWater();
            bool isAboveCeil = CheckifAboveCeil();

            if (target)
            {
                if (!target.gameObject.activeInHierarchy)
                {
                    target = null;
                    return;
                }

                curSpd = Mathf.SmoothDamp(curSpd, speed, ref veloc1, 2);

                if (target.position.x >= pos2.x && target.position.x <= pos1.x)
                {
                    if (Mathf.CeilToInt(target.up.x) == Mathf.CeilToInt(transform.up.x))
                    {
                        curSpd = Mathf.SmoothDamp(curSpd, speed * 1.1f, ref veloc1, 2);
                    }
                    else
                    {
                        curSpd = Mathf.SmoothDamp(curSpd, speed * 0.85f, ref veloc1, 2);
                    }

                    Debug.DrawRay(transform.position, (target.position - transform.position) * 3f);

                    Debug.Log(FireWeapons());

                    bombing = false;
                }
                else
                {
                    if (Mathf.Abs(target.position.x - transform.position.x) >= sortieEndRange)
                    {
                        if (!bombing)
                        {
                            Debug.Log("Beyond sortie range, " + (target.up.x <= -0.95f ? transform.up.x <= -0.95f ? false : true : transform.up.x >= 0.95f ? true : false) + " " + bombing);
                            evadePosition = new Vector2(transform.position.x - (7 * transform.up.x), transform.position.y + 0.2f);
                            resettingSortie = true;
                        }
                    }
                }
            }
            else
            {
                if (FindTarget())
                {
                    return;
                }
                else
                {
                    transform.up = Vector3.MoveTowards(transform.up, new Vector3(transform.up.x, 0f, transform.position.z), rotationSmoothing);
                }
            }
        }
    }

    private void ResettingSortie()
    {
        transform.up = Vector2.MoveTowards(transform.up, (evadePosition - transform.position).normalized, rotationSmoothing * Time.deltaTime);
        curSpd = Mathf.SmoothDamp(curSpd, speed * 1.4f, ref veloc1, 1.2f);

        if ((evadePosition - transform.position).sqrMagnitude <= 0.5f)
        {
            returningToBaseRot = true;
            resettingSortie = false;
        }

        return;
    }

    protected override bool ReturnToBaseRot()
    {
        if (base.ReturnToBaseRot())
        {
            Debug.Log("child = true");
            returningToBaseRot = false;
            ReloadWeapons();
            bombing = true;

            return true;
        }

        return false;
    }

    private void FixedUpdate()
    {
        thisRb.velocity = transform.up * curSpd * Time.fixedDeltaTime;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        bombing = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector2 pos1 = new Vector2(bombDropPos.position.x + bombDropRange, transform.position.y);
        Vector2 pos2 = new Vector2(bombDropPos.position.x - bombDropRange, transform.position.y);
        Gizmos.DrawWireSphere(new Vector2(bombDropPos.position.x, transform.position.y), 0.5f);
        Gizmos.DrawWireSphere(pos1, 1f);
        Gizmos.DrawWireSphere(pos2, 1f);
        Gizmos.DrawLine(pos1, transform.position);
        Gizmos.DrawLine(pos2, transform.position);
        Gizmos.DrawLine(pos1, new Vector3(pos1.x, 0f));
        Gizmos.DrawLine(pos2, new Vector3(pos2.x, 0f));

        if (target)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(target.position, 5f);
        }
    }
}
#pragma warning restore 0649