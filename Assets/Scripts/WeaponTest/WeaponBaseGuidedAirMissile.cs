using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBaseGuidedAirMissile : WeaponBase
{
    public override FireState Fire()
    {
        if (Time.time > fireTimer)
        {
            Transform missileClone = Poolable.Get<GuidedAAMissile>(() => Poolable.CreateObj<GuidedAAMissile>(projectilePrefab.gameObject), spawnLocation.position, owner.rotation).transform;
            int layerValue = whatAreOurProjectiles.layermask_to_layer();
            missileClone.gameObject.layer = layerValue;
            GuidedAAMissile missile = missileClone.GetComponent<GuidedAAMissile>();
            missile.whatIsTarget = whatIsTarget;
            missile.target = target;
            fireTimer = delayBetweenFire + Time.time;

            return FireState.Fired;
        }
        else
        {
            return FireState.OnDelay;
        }
    }
}
