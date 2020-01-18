using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBaseCannon : WeaponBase
{
    public override FireState Fire()
    {
        if (currentAmmo <= 0)
        {
            return FireState.OutOfAmmo;
        }

        if (Time.time > fireTimer)
        {
            Transform shellClone = Poolable.Get<CannonShell>(() => Poolable.CreateObj<CannonShell>(projectilePrefab.gameObject), spawnLocation.position, Quaternion.identity).transform;
            int layerValue = whatAreOurProjectiles.layermask_to_layer();
            shellClone.gameObject.layer = layerValue;
            shellClone.right = new Vector2(owner.up.x + Random.Range(-inaccuracyOffset, inaccuracyOffset), owner.up.y).normalized; /*(target.position - transform.position).normalized*/; // i have no fucking idea what is going on at this point
            //Rigidbody2D shellRb = shellClone.GetComponent<Rigidbody2D>();
            fireTimer = delayBetweenFire + Time.time;
            currentAmmo--;

            return FireState.Fired;
        }
        else
        {
            return FireState.OnDelay;
        }
    }
}
