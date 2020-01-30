using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
#pragma warning disable 0649
public class RegularCrate : Poolable
{
    [Header("Regular crate properties")]
    [SerializeField] private Transform parachute;
    [SerializeField] private SpawnCrateFrom spawnCrateFrom;
    [SerializeField] private float timeTilParachute;
    [SerializeField] private float constantOppositePushForce;
    [SerializeField] private float maxVelocityYSpeed;
    [SerializeField] private float brakeForceMult;
    [SerializeField] private float minParachuteAltitude;
    [SerializeField] private float parachuteCloseAltitude;
    [Header("Debug")]

    private Rigidbody2D thisRb;
    private float waterLevel;
    [SerializeField] private float balanceTimer = 1f;
    private bool hasParachuteOpen = false;
    private bool droppedOutOfParachute = false;

    private Vector2 veloc2;
    private float veloc1;

    private void Awake()
    {
        waterLevel = GameConfig.Instance.WaterLevel;
        thisRb = GetComponent<Rigidbody2D>();
    }

    protected virtual void FixedUpdate()
    {
        BalanceOutInWater();
    }

    protected virtual void BalanceOutInWater()
    {
        float grav = thisRb.gravityScale;
        Vector3 brakeVelocity = -thisRb.velocity.normalized * brakeForceMult;

        if (!hasParachuteOpen && droppedOutOfParachute)
        {
            if ((transform.position.y <= waterLevel + 0.5f && transform.position.y >= waterLevel - 0.5f))
            {
                grav = Mathf.SmoothDamp(thisRb.gravityScale, 0.05f, ref veloc1, 0.2f * Time.fixedDeltaTime) * balanceTimer;
            }
            else if (transform.position.y > waterLevel + 0.5f)
            {
                grav = Mathf.SmoothDamp(thisRb.gravityScale, 0.6f, ref veloc1, 0.3f * Mathf.Abs(transform.position.y) * Time.fixedDeltaTime);
            }
            else if (transform.position.y < waterLevel - 0.5f)
            {
                grav = Mathf.SmoothDamp(thisRb.gravityScale, -0.3f, ref veloc1, 0.5f * Mathf.Abs(transform.position.y) * Time.fixedDeltaTime);
            }

            balanceTimer *= 0.9999f;

            if (thisRb.velocity.y >= 3 * balanceTimer)
            {
                thisRb.AddForce(-thisRb.velocity * constantOppositePushForce);
            }
        }
        else if (hasParachuteOpen)
        {
            grav = Mathf.SmoothDamp(grav, 0.1f, ref veloc1, 0.9f * Time.fixedDeltaTime);

            if (transform.position.y <= waterLevel + parachuteCloseAltitude)
            {
                hasParachuteOpen = false;
                droppedOutOfParachute = true;
                parachute.gameObject.SetActive(false);
            }
        }

        thisRb.gravityScale = grav;

        if (Mathf.Abs(thisRb.velocity.y) >= maxVelocityYSpeed)
        {
            thisRb.AddForce(brakeVelocity * 0.03f); // changed Time.deltaTime to 0.03f due to weird physics behaviour on different speeds, change it back if it's not the cause
        }
    }

    private IEnumerator OpenParachuteInX(float time)
    {
        yield return new WaitForSeconds(time);

        if (transform.position.y >= waterLevel + minParachuteAltitude)
        {
            parachute.gameObject.SetActive(true); // open parachute if above x
            hasParachuteOpen = true;
        }
        else
        {
            hasParachuteOpen = false;
            droppedOutOfParachute = true;
        }
    }

    public void StartParachuteCounter(bool custom = false, float customTime = 0f)
    {
        float time = custom ? customTime : timeTilParachute;
        StartCoroutine(OpenParachuteInX(time));
    }

    private void OnEnable()
    {
        hasParachuteOpen = false;
        droppedOutOfParachute = false;
        balanceTimer = 1f;
        thisRb.gravityScale = 1f;

        if (spawnCrateFrom == SpawnCrateFrom.Air)
        {
            parachute.gameObject.SetActive(true); // open parachute if above x
            hasParachuteOpen = true;
        }
    }
}
#pragma warning restore 0649

public enum SpawnCrateFrom { UFO, Aircraft, Air, Nothing }