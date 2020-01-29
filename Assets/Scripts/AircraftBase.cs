using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public abstract class AircraftBase : UnitBase
{
    [Header("Base aircraft properties")]
    [SerializeField] protected float timeToTakeOffFully;
    [SerializeField] private float waterOffset;
    [SerializeField] protected float rotationSmoothing;
    [SerializeField] protected float ceilAltitude;

    [Header("Debug..")]
    [SerializeField] protected Vector3 retPosition;
    [SerializeField] protected Vector3 evadePosition;
    protected float evadeTimer = 0f;
    protected float evadeAlt;
    private float enemyIsTooCloseEvadeTimer = 0f;
    protected bool engaging;
    [SerializeField] protected bool evading;
    protected bool takenOff = false;
    [SerializeField] protected bool returningToBaseAlt = false;

    private float floatveloc1;

    protected virtual void Start()
    {
        OnEnable();
    }

    protected virtual IEnumerator TakeOffRoutine()
    {
        Debug.Log("taking off yes");
        takenOff = false;
        curSpd = 0f;

        yield return new WaitForSeconds(timeToTakeOffFully);

        returningToBaseAlt = true;
        retPosition = new Vector3(transform.up.x * 15, yBaseAltitude + Random.Range(4f, 10f), transform.position.z);

        yield break;
    }

    public void TakeOff()
    {
        Debug.Log("starting takeoff routine");
        StartCoroutine(TakeOffRoutine());
    }

    public void LoadInAir()
    {
        takenOff = true;
        curSpd = speed;
    }

    protected virtual void ReturnToBaseAlt()
    {
        transform.up = Vector3.MoveTowards(transform.up, (retPosition - transform.position).normalized, rotationSmoothing * Time.deltaTime * 3f);

        if (transform.position.y >= yBaseAltitude)
        {
            if (!takenOff)
            {
                if (ReturnToBaseRot())
                {
                    takenOff = true;
                    returningToBaseAlt = false;
                    return;
                }
            }
            else
            {
                if ((yBaseAltitude - transform.position.y) >= -2f && (yBaseAltitude - transform.position.y) <= 2f)
                {
                    returningToBaseAlt = false;
                    enemyIsTooCloseEvadeTimer = 0f;
                }
            }
        }

        return;
    }

    protected virtual bool ReturnToBaseRot()
    {
        int rot = transform.up.x >= 0 ? 1 : -1;
        Debug.Log(Mathf.CeilToInt(transform.up.x));
        transform.up = Vector3.MoveTowards(transform.up, new Vector2(transform.up.x > 0 ? Mathf.CeilToInt(transform.up.x) : Mathf.FloorToInt(transform.up.x), 0f), rotationSmoothing * Time.deltaTime * 1.8f);

        if (transform.up.y >= retPosition.y - 3f)
        {
            Debug.Log("super = true");
            //transform.up = new Vector2(transform.up.x > 0 ? 1 : -1, 0f);
            return true;
        }

        return false;
    }

    protected virtual void Evade(float escapeRange)
    {
        //Debug.Log(returningToBaseAlt + " | " + evading + " | " + transform.name);

        if (Time.time > evadeTimer)
        {
            evading = false;
            return;
        }

        if (target)
        {
            if ((target.position - transform.position).magnitude <= escapeRange * 1.4f)
            {
                enemyIsTooCloseEvadeTimer += Time.deltaTime;

                if (enemyIsTooCloseEvadeTimer >= 3f)
                {
                    Debug.Log("Too close!");
                    int rand = Random.Range(0, 30) > 20 ? -1 : 1;
                    retPosition = new Vector3(transform.up.x * Random.Range(14f, 30f) * rand, yBaseAltitude + Random.Range(4f, 14f), transform.position.z);
                    evading = false;
                    returningToBaseAlt = true;
                    return;
                }
            }
            else
            {
                enemyIsTooCloseEvadeTimer = 0;
            }
        }


        transform.up = Vector3.MoveTowards(transform.up, (evadePosition - transform.position).normalized, rotationSmoothing * Time.deltaTime);
        return;
    }

    protected bool CheckIfNearWater()
    {
        if (transform.position.y <= waterLevel + waterOffset)
        {
            returningToBaseAlt = true;
            evading = false;
            engaging = false;
            retPosition = new Vector3(transform.position.x * 3, yBaseAltitude + Random.Range(4f, 6f), transform.position.z);

            return true;
        }

        return false;
    }

    protected bool CheckifAboveCeil()
    {
        if(transform.position.y >= ceilAltitude)
        {
            returningToBaseAlt = true;
            evading = false;
            engaging = false;
            retPosition = new Vector3(transform.up.x * 20f, ceilAltitude / 2f, transform.position.z);
            curSpd = Mathf.SmoothDamp(curSpd, speed * 1.4f, ref floatveloc1, 1f);

            return true;
        }

        return false;
    }

    protected virtual void OnEnable()
    {
        evadeAlt = yBaseAltitude / 1.1f;
    }

    private void OnDrawGizmosSelected()
    {
        if (evadePosition != Vector3.zero && evadePosition != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(evadePosition, 3f);
        }
    }

    private void OnDisable()
    {
        evading = false;
        engaging = false;
        returningToBaseAlt = false;
        takenOff = false;
        target = null;
        curSpd = 0f;
        enemyIsTooCloseEvadeTimer = 0f;
    }
}
#pragma warning restore 0649