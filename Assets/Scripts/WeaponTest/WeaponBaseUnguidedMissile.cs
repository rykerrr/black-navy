using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class WeaponBaseUnguidedMissile : WeaponBase
{
    public override FireState Fire()
    {
        bool isTargVisual = CheckIfLookingAtTarget(lookCheckRange);

        if (isTargVisual)
        {
            if (Time.time > fireTimer)
            {
                Transform missileClone = Poolable.Get<UnguidedMissile>(() => Poolable.CreateObj<UnguidedMissile>(projectilePrefab.gameObject), spawnLocation.position, owner.rotation).transform;
                int layerValue = whatAreOurProjectiles.layermask_to_layer();
                missileClone.gameObject.layer = layerValue;
                UnguidedMissile missile = missileClone.GetComponent<UnguidedMissile>();
                TrailRenderer projTrail = missileClone.GetComponent<TrailRenderer>();
                projTrail.material = layerValue == 8 ? t1Mat : t2Mat;
                missile.ActivateBoost();
                fireTimer = delayBetweenFire + Time.time;
                fireSound.Play();

                return FireState.Fired;
            }
            else
            {
                return FireState.OnDelay;
            }
        }
        else return FireState.Failed;
    }
}
#pragma warning restore 0649