using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class WeaponBaseSurfaceGuidedMissile : WeaponBase
{
    public override FireState Fire()
    {
        if (Time.time > fireTimer)
        {
            Transform missileClone = Poolable.Get<GuidedSSMissile>(() => Poolable.CreateObj<GuidedSSMissile>(projectilePrefab.gameObject), new Vector3(spawnLocation.position.x + Random.Range(-0.2f, 0.2f), spawnLocation.position.y, spawnLocation.position.z), Quaternion.identity).transform;
            int layerValue = whatAreOurProjectiles.layermask_to_layer();
            missileClone.gameObject.layer = layerValue;
            GuidedSSMissile missile = missileClone.GetComponent<GuidedSSMissile>();
            missile.whatIsTarget = whatIsTarget;
            missile.BoostStage();
            missile.GetComponent<TrailRenderer>().material = layerValue == 8 ? t1Mat : t2Mat;
            fireTimer = delayBetweenFire + Time.time;

            return FireState.OutOfAmmo;
        }
        else
        {
            return FireState.OnDelay;
        }
    }
}
#pragma warning restore 0649