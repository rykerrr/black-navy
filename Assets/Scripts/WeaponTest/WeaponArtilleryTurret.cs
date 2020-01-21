using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class WeaponArtilleryTurret : WeaponBase
{
    [Header("Artillery turret properties")]
    [SerializeField] private Transform barrel;
    [SerializeField] private float rotationSmoothing;

    private Transform prevBull;
    private Rigidbody2D bullRb;
    private Vector2 aimPos;
    private float projBaseGrav;

    private void Update()
    {
        if (target)
        {
            Fire();
        }
        else
        {
            if (FindTarget())
            {
                return;
            }
        }

        barrel.up = Vector2.MoveTowards(barrel.up, aimPos, rotationSmoothing * Time.deltaTime);
    }

    public override FireState Fire()
    {
        if(target == null)
        {
            return FireState.Failed;
        }

        int results = SolveBallisticArc(spawnLocation.position, projectilePrefab.GetComponent<CannonShell>().Speed * Time.deltaTime, target.position, -Physics2D.gravity.y * projBaseGrav, out Vector3 s0, out Vector3 s1);
        aimPos = s0.normalized;

        if (Time.time > fireTimer)
        {
            if (prevBull == null)
            {
                prevBull = Instantiate(projectilePrefab, spawnLocation.position, Quaternion.identity);
                int layerValue = whatAreOurProjectiles.layermask_to_layer();
                prevBull.gameObject.layer = layerValue;
                prevBull.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                prevBull.gameObject.SetActive(false);
                bullRb = prevBull.GetComponent<Rigidbody2D>();
            }

            if (results > 0 && VecApprox(barrel.up, aimPos))
            {
                prevBull.position = spawnLocation.position;
                prevBull.gameObject.SetActive(true);
                bullRb.AddForce((Vector2)barrel.up * projectilePrefab.GetComponent<CannonShell>().Speed * Time.deltaTime + new Vector2(0f, Random.Range(-inaccuracyOffset, inaccuracyOffset)), ForceMode2D.Impulse);
                prevBull.GetComponent<TrailRenderer>().material = prevBull.gameObject.layer == 8 ? t1Mat : t2Mat;
                prevBull = null;
            }

            fireTimer = delayBetweenFire + Time.time + Random.Range(-delayBetweenFire / 5f, delayBetweenFire / 3.4f);

            return FireState.Fired;
        }

        return FireState.OnDelay;
    }

    // https://www.forrestthewoods.com/blog/solving_ballistic_trajectories/
    private int SolveBallisticArc(Vector3 proj_pos, float proj_speed, Vector3 target, float gravity, out Vector3 s0, out Vector3 s1)
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
            return 0;

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

    private bool VecApprox(Vector2 vec1, Vector2 vec2)
    {
        if (Mathf.Abs(vec1.x - vec2.x) < 0.005f && Mathf.Abs(vec1.x - vec2.x) < 0.005f)
        {
            return true;
        }
        else return false;
    }

    private void OnEnable()
    {
        projBaseGrav = projectilePrefab.GetComponent<Rigidbody2D>().gravityScale;
    }
}
#pragma warning restore 0649