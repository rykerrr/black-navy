using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
[RequireComponent(typeof(Rigidbody2D))]
public class GuidedBomb : MonoBehaviour
{
    [Header("Set by script, shown for debug purposes")]
    [SerializeField] public LayerMask whatIsEnemy;
    [SerializeField] public Transform target;
    [SerializeField] private Rigidbody2D thisRb;
    [Header("Properties that are also edited by the script")]
    [SerializeField] public float timeBeforeBoosters;
    [Header("Properties")]
    [SerializeField] private Transform onHitParticlePrefab;
    [SerializeField] private float forceGeneration;
    [SerializeField] private float bombSmTime;

    private Vector3 veloc1;
    private float boosterTimer;
    private bool targetIsVisual;
    private bool gliding = false;

    private void Start()
    {
        thisRb = GetComponent<Rigidbody2D>();
        boosterTimer = timeBeforeBoosters + Time.time;
    }

    private void Update()
    {
        targetIsVisual = CheckIfLookingAtTarget();
    }

    private void FixedUpdate()
    {
        if (target && transform.position.y >= target.position.y)
        {
            transform.up = Vector3.SmoothDamp(transform.up, (target.position - transform.position).normalized, ref veloc1, Time.time > boosterTimer ? bombSmTime / 1.5f : bombSmTime);

            if (targetIsVisual)
            {
                if(Time.time > boosterTimer)
                {

                    if(!gliding)
                    {
                        foreach (Transform child in transform)
                        {
                            child.GetComponent<MeshRenderer>().enabled = true;
                        }
                        gliding = true;
                        thisRb.velocity /= 4f;
                        thisRb.gravityScale = 0.3f;
                    }

                    thisRb.AddForce(transform.up * forceGeneration * 0.03f, ForceMode2D.Force); // changed Time.deltaTime to 0.03f due to weird physics behaviour on different speeds, change it back if it's not the cause
                }
            }
        }
    }

    private bool CheckIfLookingAtTarget()
    {
        RaycastHit2D ray = Physics2D.Raycast(transform.position, transform.up, 50f, whatIsEnemy);
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

    private void OnEnable()
    {
        boosterTimer = timeBeforeBoosters + Time.time;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Transform particleClone = Instantiate(onHitParticlePrefab, transform.position, Quaternion.identity) as Transform;
        Destroy(particleClone.gameObject, 1f);
        Destroy(gameObject);
    }
}
#pragma warning disable 0649