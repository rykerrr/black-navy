using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class WeaponBaseGuidedTorpedo : WeaponBase
{
    //[Header("WeaponProperties")]

    public override FireState Fire()
    {
        Transform isInView = isEnemySubInView();
        if (isInView)
        {
            target = isInView;
        }

        if (Time.time >= fireTimer)
        {
            Transform torpClone = Poolable.Get<GuidedTorpedo>(() => Poolable.CreateObj<GuidedTorpedo>(projectilePrefab.gameObject), spawnLocation.position, Quaternion.identity).transform;
            int layerValue = whatAreOurProjectiles.layermask_to_layer();
            TrailRenderer projTrail = torpClone.GetComponent<TrailRenderer>();
            projTrail.material = layerValue == 8 ? t1Mat : t2Mat;
            torpClone.gameObject.layer = layerValue;
            projTrail.Clear();
            GuidedTorpedo torp = torpClone.GetComponent<GuidedTorpedo>();
            torp.target = target;
            torp.whatIsTarget = whatIsTarget;
            torpClone.gameObject.layer = layerValue;
            torpClone.GetComponent<Rigidbody2D>().velocity = owner.up * torp.Speed / 2f * Time.deltaTime;

            torpClone.up = owner.up;

            fireTimer = delayBetweenFire + Time.time + Random.Range(-delayBetweenFire / 5f, delayBetweenFire / 3.4f);
            return FireState.Fired;
        }
        else
        {
            return FireState.OnDelay;
        }
    }

    private Transform isEnemySubInView()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(spawnLocation.position, lookCheckRadius, owner.up, lookCheckRange, whatIsTarget);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null)
            {
                if (hit.collider.GetComponent<Submarine>())
                {
                    return hit.transform;
                }
            }
        }

        return null;
    }
}
#pragma warning restore 0649