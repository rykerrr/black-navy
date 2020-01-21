using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBaseUnguidedBombs : WeaponBase
{
    public override FireState Fire()
    {
        if(currentAmmo <= 0)
        {
            return FireState.OutOfAmmo;
        }

        if (Time.time > fireTimer)
        {
            Vector2 ranPos = new Vector2(Random.Range(spawnLocation.position.x - Random.Range(-inaccuracyOffset, inaccuracyOffset), spawnLocation.position.x + Random.Range(-inaccuracyOffset, inaccuracyOffset)), spawnLocation.position.y);
            Transform bombClone = Poolable.Get<UnguidedBomb>(() => Poolable.CreateObj<UnguidedBomb>(projectilePrefab.gameObject), ranPos, Quaternion.identity).transform;
            int layerValue = whatAreOurProjectiles.layermask_to_layer();
            bombClone.gameObject.layer = layerValue;
            bombClone.right = new Vector2(owner.up.x + Random.Range(-inaccuracyOffset, inaccuracyOffset), owner.up.y).normalized; /*(target.position - transform.position).normalized*/; // i have no fucking idea what is going on at this point
            Rigidbody2D bombRb = bombClone.GetComponent<Rigidbody2D>();
            bombRb.velocity = new Vector2(ownerRb.velocity.x / 1.5f, ownerRb.velocity.y / 1.1f);
            //Rigidbody2D shellRb = shellClone.GetComponent<Rigidbody2D>();
            fireTimer = delayBetweenFire + Time.time;
            currentAmmo--;
            return FireState.Fired;
        }
        else if(Time.time <= fireTimer) return FireState.OnDelay;

        return FireState.Failed;
    }
}
