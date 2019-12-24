using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
[RequireComponent(typeof(Rigidbody2D))]
public class Shell : MonoBehaviour
{
    [Header("Set by script, shown for debug purposes")]
    [SerializeField] private Transform target;
    [SerializeField] private Rigidbody2D thisRb;
    [Header("Properties")]
    //[SerializeField] private LayerMask whatIsTarget;
    //[SerializeField] private float targetCheckRadius;
    [SerializeField] private Transform onHitParticlePrefab;
    [SerializeField] private float waterLevel;
    [SerializeField] private float initialSpeed;

    private Vector2 veloc2 = Vector2.zero;
    private float veloc1;

    private void Start()
    {
        thisRb = GetComponent<Rigidbody2D>();
        thisRb.velocity = thisRb.velocity + (Vector2) transform.right * initialSpeed * Time.fixedDeltaTime;
    }

    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        //thisRb.velocity = transform.right * speed * Time.fixedDeltaTime;
        IsOutOfWater();
    }

    // technically don't need this as bullets won't need to know their target
    /*private bool FindTarget()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, targetCheckRadius, whatIsTarget);

        if (hit != null)
        {
            target = hit.transform;
            Debug.Log("Found target!");
            return true;
        }

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, targetCheckRadius);
    }*/

    private bool IsOutOfWater()
    {
        if (transform.position.y <= waterLevel)
        {
            thisRb.gravityScale = Mathf.SmoothDamp(thisRb.gravityScale, 0.004f, ref veloc1, Random.Range(0.4f, 0.9f));
            thisRb.velocity = Vector2.SmoothDamp(thisRb.velocity, thisRb.velocity / 2f, ref veloc2, Random.Range(0.2f, 0.6f));
            return false;
        }

        return true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Transform particleClone = Instantiate(onHitParticlePrefab, transform.position, Quaternion.identity) as Transform;
        Destroy(particleClone.gameObject, 1f);
        Destroy(gameObject);
    }
}
#pragma warning restore 0649