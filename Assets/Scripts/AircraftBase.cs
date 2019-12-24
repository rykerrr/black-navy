using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AircraftBase : UnitBase
{
    [Header("Base aircraft properties")]
    [SerializeField] private float yBaseAltitude;
    [SerializeField] private float timeToTakeOffFully;
    [SerializeField] private float waterOffset;

    private Vector3 retPosition;
    private Vector3 evadePosition;
    private float evadeTimer = 0f;
    private float enemyIsTooCloseEvadeTimer = 0f;
    private bool engaging;
    private bool evading;
    private bool takenOff = false;
    private bool returningToBaseAlt = false;

    private float floatveloc1;

    protected IEnumerator TakeOffRoutine()
    {
        takenOff = false;
        curSpd = 0f;

        yield return new WaitForSeconds(timeToTakeOffFully);

        returningToBaseAlt = true;
        retPosition = new Vector3(transform.up.x * 35f, yBaseAltitude + Random.Range(4f, 10f), transform.position.z);

        yield break;
    }

    protected void TakeOff()
    {
        StartCoroutine(TakeOffRoutine());
    }

    protected void LoadInAir()
    {
        takenOff = true;
        curSpd = speed;
    }

    protected virtual void ReturnToBaseAlt()
    {
        transform.up = Vector3.MoveTowards(transform.up, retPosition, Random.Range(0.08f, 0.12f));

        if (transform.position.y >= yBaseAltitude)
        {
            if (!takenOff)
            {
                takenOff = true;
            }

            returningToBaseAlt = false;
        }

        return;
    }

    protected virtual void Evade(float escapeRange)
    {
        if (Time.time > evadeTimer)
        {
            evading = false;
            return;
        }

        if ((target.position - transform.position).magnitude <= escapeRange)
        {
            enemyIsTooCloseEvadeTimer += Time.deltaTime;

            if (enemyIsTooCloseEvadeTimer >= 5f)
            {
                Debug.Log("Too close!");
                int rand = Random.Range(0, 30) > 20 ? -1 : 1;
                retPosition = new Vector3(transform.up.x * Random.Range(14f, 30f) * rand, yBaseAltitude + Random.Range(4f, 14f), transform.position.z);
                evading = false;
                returningToBaseAlt = true;
            }
        }
        else
        {
            enemyIsTooCloseEvadeTimer = 0;
        }

        transform.up = Vector3.MoveTowards(transform.up, (evadePosition - transform.position).normalized, 0.06f);
        return;
    }

    protected void CheckIfNearWater()
    {
        if (transform.position.y <= waterLevel + waterOffset && !returningToBaseAlt)
        {
            Debug.Log("returning to base alt is true");
            returningToBaseAlt = true;
            engaging = false;
            retPosition = new Vector3(transform.up.x * 70, yBaseAltitude + Random.Range(4f, 6f), transform.position.z);
        }
    }
}
