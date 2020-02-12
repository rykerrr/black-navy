using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBaseGuidedAirMissile : WeaponBase
{
    public override FireState Fire()
    {
        bool isTargVisual = CheckIfLookingAtTarget(lookCheckRange);

        if (isTargVisual)
        {
            if (Time.time > fireTimer)
            {
                Transform missileClone = Poolable.Get<GuidedAAMissile>(() => Poolable.CreateObj<GuidedAAMissile>(projectilePrefab.gameObject), spawnLocation.position, owner.rotation).transform;
                int layerValue = whatAreOurProjectiles.layermask_to_layer();
                missileClone.gameObject.layer = layerValue;
                GuidedAAMissile missile = missileClone.GetComponent<GuidedAAMissile>();
                missile.whatIsTarget = whatIsTarget;
                missile.target = target;
                TrailRenderer projTrail = missileClone.GetComponent<TrailRenderer>();
                projTrail.material = layerValue == 8 ? t1Mat : t2Mat;
                projTrail.enabled = true;
                fireTimer = delayBetweenFire + Time.time + Random.Range(-delayBetweenFire / 5f, delayBetweenFire / 3.4f);
                fireSound.Play();

                return FireState.Fired;
            }
            else
            {
                if (!target.gameObject.activeInHierarchy)
                {
                    target = null;
                    return FireState.Failed;
                }

                return FireState.OnDelay;
            }
        }
        else return FireState.Failed;

    }
}
