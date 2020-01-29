using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class WeaponBaseDepthChargeDropper : WeaponBase
{
    [SerializeField] private int chargesPerDrop;
    [SerializeField] private float forceOfThrow;

    private void Update()
    {
        if (!target)
        {
            if (Time.time > findTargetTimer)
            {
                if (FindTarget())
                {
                    Debug.Log("Found target");
                }
            }
        }
        else
        {
            Fire();
        }
    }

    public override FireState Fire()
    {
        if (target)
        {
            if (Time.time > fireTimer)
            {
                float forceMult = forceOfThrow;
                int curCharges = chargesPerDrop;
                int dir = 1;

                if (chargesPerDrop % 2 != 0)
                {
                    curCharges--;
                    DropCharge(0, forceMult);
                }

                for (int i = 0; i < curCharges; i++)
                {
                    DropCharge(dir, forceMult);
                    dir *= -1;
                    DropCharge(dir, forceMult);
                    dir *= -1;
                    i++;
                    forceMult *= 1.2f;
                }

                fireTimer = delayBetweenFire + Time.time + Random.Range(-delayBetweenFire / 5f, delayBetweenFire / 3.4f);
                Debug.Log("Fired");
                return FireState.Fired;
            }
            if (Time.time <= fireTimer)
            {
                Debug.Log("On fire timer");
                return FireState.OnDelay;
            }

            Debug.Log("target but failed?");
            return FireState.Failed;
        }

        Debug.Log("no target");
        return FireState.Failed;
    }

    private void DropCharge(float dir, float forceMult)
    {
        Transform chargeClone = Poolable.Get<UnguidedDepthCharge>(() => Poolable.CreateObj<UnguidedDepthCharge>(projectilePrefab.gameObject), spawnLocation.position, Quaternion.identity).transform;
        Debug.Log("here");
        int layerValue = whatAreOurProjectiles.layermask_to_layer();
        TrailRenderer projTrail = chargeClone.GetComponent<TrailRenderer>();
        projTrail.material = layerValue == 8 ? t1Mat : t2Mat;
        projTrail.Clear();
        chargeClone.gameObject.layer = layerValue;
        Debug.Log("yes | " + chargeClone);

        Rigidbody2D thisRb = chargeClone.GetComponent<Rigidbody2D>();

        thisRb.velocity = ((Vector2.right * dir + (Vector2)transform.up).normalized) * forceMult;
        Debug.Log("commeth once again");
        return;
    }
}
#pragma warning restore 0649