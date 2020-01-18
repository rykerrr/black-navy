using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretAngle : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private Transform target;
    [SerializeField] private Transform barrel;
    [SerializeField] private Transform bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireDelay;
    [SerializeField] private float timeToRotate;

    private Rigidbody2D bullRb = null;
    private Transform prevBull = null;
    private Vector2 aimPos = Vector2.zero;
    private float x;
    private float baseGrav;
    private float fireTimer;

    private void Start()
    {
        Rigidbody2D bullRb = bulletPrefab.GetComponent<Rigidbody2D>();
        baseGrav = bullRb.gravityScale;
    }

    private void Update()
    {
        if (target)
        {
            Fire();
        }

        barrel.up = Vector2.MoveTowards(barrel.up, aimPos, timeToRotate * Time.deltaTime);
    }

    private void Fire()
    {
        //int results = solve_ballistic_arc(firePoint.position, bulletPrefab.GetComponent<CannonShell>().Speed * Time.deltaTime, target.position, -Physics2D.gravity.y * baseGrav, out Vector3 s0, out Vector3 s1);
        //aimPos = s0.normalized;

        if (Time.time > fireTimer)
        {
            if (prevBull == null)
            {
                prevBull = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
                prevBull.gameObject.layer = 8;
                prevBull.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                prevBull.gameObject.SetActive(false);
                bullRb = prevBull.GetComponent<Rigidbody2D>();
            }

            //if (results > 0 && VecApprox(barrel.up, aimPos))
            //{
            //    prevBull.position = firePoint.position;
            //    prevBull.gameObject.SetActive(true);
            //    bullRb.AddForce((Vector2) barrel.up * bulletPrefab.GetComponent<CannonShell>().Speed * Time.deltaTime + new Vector2(0f, Random.Range(-2f, 2f)), ForceMode2D.Impulse);
            //    prevBull = null;
            //}

            //if (solve_ballistic_arc_lateral(prevBull.position, prevBull.GetComponent<ProjectileBase>().Speed * Time.deltaTime / Time.timeScale, target.position, maxHeight, out Vector3 projVeloc, out float newGrav))
            //{
            //    prevBull.gameObject.SetActive(true);
            //    bullRb.AddForce(projVeloc, ForceMode2D.Impulse);
            //    bullRb.gravityScale = baseGrav * newGrav;
            //    barrel.up = projVeloc.normalized;
            //    prevBull.gameObject.layer = 8;
            //    prevBull = null;
            //}

            fireTimer = fireDelay + Time.time;
        }
    }

    private bool VecApprox(Vector2 vec1, Vector2 vec2)
    {
        if (Mathf.Abs(vec1.x - vec2.x) < 0.005f && Mathf.Abs(vec1.x - vec2.x) < 0.005f)
        {
            return true;
        }
        else return false;
    }

    public static bool IsZero(double d)
    {
        const double eps = 1e-9;
        return d > -eps && d < eps;
    }

    // Solve quadratic equation: c0*x^2 + c1*x + c2. 
    // Returns number of solutions.
    public static int SolveQuadric(double c0, double c1, double c2, out double s0, out double s1)
    {
        s0 = double.NaN;
        s1 = double.NaN;

        double p, q, D;

        /* normal form: x^2 + px + q = 0 */
        p = c1 / (2 * c0);
        q = c2 / c0;

        D = p * p - q;

        if (IsZero(D))
        {
            s0 = -p;
            return 1;
        }
        else if (D < 0)
        {
            return 0;
        }
        else /* if (D > 0) */
        {
            double sqrt_D = System.Math.Sqrt(D);

            s0 = sqrt_D - p;
            s1 = -sqrt_D - p;
            return 2;
        }
    }

    // Solve cubic equation: c0*x^3 + c1*x^2 + c2*x + c3. 
    // Returns number of solutions.
    public static int SolveCubic(double c0, double c1, double c2, double c3, out double s0, out double s1, out double s2)
    {
        s0 = double.NaN;
        s1 = double.NaN;
        s2 = double.NaN;

        int num;
        double sub;
        double A, B, C;
        double sq_A, p, q;
        double cb_p, D;

        /* normal form: x^3 + Ax^2 + Bx + C = 0 */
        A = c1 / c0;
        B = c2 / c0;
        C = c3 / c0;

        /*  substitute x = y - A/3 to eliminate quadric term:  x^3 +px + q = 0 */
        sq_A = A * A;
        p = 1.0 / 3 * (-1.0 / 3 * sq_A + B);
        q = 1.0 / 2 * (2.0 / 27 * A * sq_A - 1.0 / 3 * A * B + C);

        /* use Cardano's formula */
        cb_p = p * p * p;
        D = q * q + cb_p;

        if (IsZero(D))
        {
            if (IsZero(q)) /* one triple solution */
            {
                s0 = 0;
                num = 1;
            }
            else /* one single and one double solution */
            {
                double u = System.Math.Pow(-q, 1.0 / 3.0);
                s0 = 2 * u;
                s1 = -u;
                num = 2;
            }
        }
        else if (D < 0) /* Casus irreducibilis: three real solutions */
        {
            double phi = 1.0 / 3 * System.Math.Acos(-q / System.Math.Sqrt(-cb_p));
            double t = 2 * System.Math.Sqrt(-p);

            s0 = t * System.Math.Cos(phi);
            s1 = -t * System.Math.Cos(phi + System.Math.PI / 3);
            s2 = -t * System.Math.Cos(phi - System.Math.PI / 3);
            num = 3;
        }
        else /* one real solution */
        {
            double sqrt_D = System.Math.Sqrt(D);
            double u = System.Math.Pow(sqrt_D - q, 1.0 / 3.0);
            double v = -System.Math.Pow(sqrt_D + q, 1.0 / 3.0);

            s0 = u + v;
            num = 1;
        }

        /* resubstitute */
        sub = 1.0 / 3 * A;

        if (num > 0) s0 -= sub;
        if (num > 1) s1 -= sub;
        if (num > 2) s2 -= sub;

        return num;
    }

    // Solve quartic function: c0*x^4 + c1*x^3 + c2*x^2 + c3*x + c4. 
    // Returns number of solutions.
    public static int SolveQuartic(double c0, double c1, double c2, double c3, double c4, out double s0, out double s1, out double s2, out double s3)
    {
        s0 = double.NaN;
        s1 = double.NaN;
        s2 = double.NaN;
        s3 = double.NaN;

        double[] coeffs = new double[4];
        double z, u, v, sub;
        double A, B, C, D;
        double sq_A, p, q, r;
        int num;

        /* normal form: x^4 + Ax^3 + Bx^2 + Cx + D = 0 */
        A = c1 / c0;
        B = c2 / c0;
        C = c3 / c0;
        D = c4 / c0;

        /*  substitute x = y - A/4 to eliminate cubic term: x^4 + px^2 + qx + r = 0 */
        sq_A = A * A;
        p = -3.0 / 8 * sq_A + B;
        q = 1.0 / 8 * sq_A * A - 1.0 / 2 * A * B + C;
        r = -3.0 / 256 * sq_A * sq_A + 1.0 / 16 * sq_A * B - 1.0 / 4 * A * C + D;

        if (IsZero(r))
        {
            /* no absolute term: y(y^3 + py + q) = 0 */

            coeffs[3] = q;
            coeffs[2] = p;
            coeffs[1] = 0;
            coeffs[0] = 1;

            num = SolveCubic(coeffs[0], coeffs[1], coeffs[2], coeffs[3], out s0, out s1, out s2);
        }
        else
        {
            /* solve the resolvent cubic ... */
            coeffs[3] = 1.0 / 2 * r * p - 1.0 / 8 * q * q;
            coeffs[2] = -r;
            coeffs[1] = -1.0 / 2 * p;
            coeffs[0] = 1;

            SolveCubic(coeffs[0], coeffs[1], coeffs[2], coeffs[3], out s0, out s1, out s2);

            /* ... and take the one real solution ... */
            z = s0;

            /* ... to build two quadric equations */
            u = z * z - r;
            v = 2 * z - p;

            if (IsZero(u))
                u = 0;
            else if (u > 0)
                u = System.Math.Sqrt(u);
            else
                return 0;

            if (IsZero(v))
                v = 0;
            else if (v > 0)
                v = System.Math.Sqrt(v);
            else
                return 0;

            coeffs[2] = z - u;
            coeffs[1] = q < 0 ? -v : v;
            coeffs[0] = 1;

            num = SolveQuadric(coeffs[0], coeffs[1], coeffs[2], out s0, out s1);

            coeffs[2] = z + u;
            coeffs[1] = q < 0 ? v : -v;
            coeffs[0] = 1;

            if (num == 0) num += SolveQuadric(coeffs[0], coeffs[1], coeffs[2], out s0, out s1);
            if (num == 1) num += SolveQuadric(coeffs[0], coeffs[1], coeffs[2], out s1, out s2);
            if (num == 2) num += SolveQuadric(coeffs[0], coeffs[1], coeffs[2], out s2, out s3);
        }

        /* resubstitute */
        sub = 1.0 / 4 * A;

        if (num > 0) s0 -= sub;
        if (num > 1) s1 -= sub;
        if (num > 2) s2 -= sub;
        if (num > 3) s3 -= sub;

        return num;
    }

    public static int solve_ballistic_arc(Vector3 proj_pos, float proj_speed, Vector3 target_pos, Vector3 target_velocity, float gravity, out Vector3 s0, out Vector3 s1)
    {

        // Initialize output parameters
        s0 = Vector3.zero;
        s1 = Vector3.zero;

        // Derivation 
        //
        //  For full derivation see: blog.forrestthewoods.com
        //  Here is an abbreviated version.
        //
        //  Four equations, four unknowns (solution.x, solution.y, solution.z, time):
        //
        //  (1) proj_pos.x + solution.x*time = target_pos.x + target_vel.x*time
        //  (2) proj_pos.y + solution.y*time + .5*G*t = target_pos.y + target_vel.y*time
        //  (3) proj_pos.z + solution.z*time = target_pos.z + target_vel.z*time
        //  (4) proj_speed^2 = solution.x^2 + solution.y^2 + solution.z^2
        //
        //  (5) Solve for solution.x and solution.z in equations (1) and (3)
        //  (6) Square solution.x and solution.z from (5)
        //  (7) Solve solution.y^2 by plugging (6) into (4)
        //  (8) Solve solution.y by rearranging (2)
        //  (9) Square (8)
        //  (10) Set (8) = (7). All solution.xyz terms should be gone. Only time remains.
        //  (11) Rearrange 10. It will be of the form a*^4 + b*t^3 + c*t^2 + d*t * e. This is a quartic.
        //  (12) Solve the quartic using SolveQuartic.
        //  (13) If there are no positive, real roots there is no solution.
        //  (14) Each positive, real root is one valid solution
        //  (15) Plug each time value into (1) (2) and (3) to calculate solution.xyz
        //  (16) The end.

        double G = gravity;

        double A = proj_pos.x;
        double B = proj_pos.y;
        double C = proj_pos.z;
        double M = target_pos.x;
        double N = target_pos.y;
        double O = target_pos.z;
        double P = target_velocity.x;
        double Q = target_velocity.y;
        double R = target_velocity.z;
        double S = proj_speed;

        double H = M - A;
        double J = O - C;
        double K = N - B;
        double L = -.5f * G;

        // Quartic Coeffecients
        double c0 = L * L;
        double c1 = 2 * Q * L;
        double c2 = Q * Q + 2 * K * L - S * S + P * P + R * R;
        double c3 = 2 * K * Q + 2 * H * P + 2 * J * R;
        double c4 = K * K + H * H + J * J;

        // Solve quartic
        double[] times = new double[4];
        int numTimes = SolveQuartic(c0, c1, c2, c3, c4, out times[0], out times[1], out times[2], out times[3]);

        // Sort so faster collision is found first
        System.Array.Sort(times);

        // Plug quartic solutions into base equations
        // There should never be more than 2 positive, real roots.
        Vector3[] solutions = new Vector3[2];
        int numSolutions = 0;

        for (int i = 0; i < numTimes && numSolutions < 2; ++i)
        {
            double t = times[i];
            if (t <= 0)
                continue;

            solutions[numSolutions].x = (float)((H + P * t) / t);
            solutions[numSolutions].y = (float)((K + Q * t - L * t * t) / t);
            solutions[numSolutions].z = (float)((J + R * t) / t);
            ++numSolutions;
        }

        // Write out solutions
        if (numSolutions > 0) s0 = solutions[0];
        if (numSolutions > 1) s1 = solutions[1];

        return numSolutions;
    }

    public bool solve_ballistic_arc_lateral(Vector3 proj_pos, float lateral_speed, Vector3 target_pos, float max_height, out Vector3 fire_velocity, out float gravity)
    {

        // Handling these cases is up to your project's coding standards
        Debug.Assert(proj_pos != target_pos && lateral_speed > 0 && max_height > proj_pos.y, "fts.solve_ballistic_arc called with invalid data");

        fire_velocity = Vector3.zero;
        gravity = float.NaN;

        Vector3 diff = target_pos - proj_pos;
        Vector3 diffXZ = new Vector3(diff.x, 0f, diff.z);
        float lateralDist = diffXZ.magnitude;

        if (lateralDist == 0)
            return false;

        float time = lateralDist / lateral_speed;

        fire_velocity = diffXZ.normalized * lateral_speed;

        // System of equations. Hit max_height at t=.5*time. Hit target at t=time.
        //
        // peak = y0 + vertical_speed*halfTime + .5*gravity*halfTime^2
        // end = y0 + vertical_speed*time + .5*gravity*time^s
        // Wolfram Alpha: solve b = a + .5*v*t + .5*g*(.5*t)^2, c = a + vt + .5*g*t^2 for g, v
        float a = proj_pos.y;       // initial
        float b = max_height;       // peak
        float c = target_pos.y;     // final

        gravity = -4 * (a - 2 * b + c) / (time * time);
        fire_velocity.y = -(3 * a - 4 * b + c) / time;

        return true;
    }


}
#pragma warning restore 0649