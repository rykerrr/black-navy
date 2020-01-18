using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBaseUnguidedMissile : WeaponBase
{
    public override FireState Fire()
    {
        if (Time.time > fireTimer)
        {
            Transform missileClone = Poolable.Get<UnguidedMissile>(() => Poolable.CreateObj<UnguidedMissile>(projectilePrefab.gameObject), spawnLocation.position, owner.rotation).transform;
            int layerValue = whatAreOurProjectiles.layermask_to_layer();
            missileClone.gameObject.layer = layerValue;
            UnguidedMissile missile = missileClone.GetComponent<UnguidedMissile>();
            missile.ActivateBoost();
            fireTimer = delayBetweenFire + Time.time;

            return FireState.Fired;
        }
        else
        {
            return FireState.OnDelay;
        }
    }
}
