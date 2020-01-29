using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBaseGuidedBombs : WeaponBase
{
    public override FireState Fire()
    {
        if (currentAmmo <= 0)
        {
            return FireState.OutOfAmmo;
        }

        if (Time.time > fireTimer)
        {
            float yOffsetDelay = Mathf.Clamp(1 / (30 / Mathf.Abs(owner.position.y)), 0f, 1f);
            Transform bombClone = Poolable.Get<GuidedGlideBomb>(() => Poolable.CreateObj<GuidedGlideBomb>(projectilePrefab.gameObject), spawnLocation.position, owner.rotation).transform;
            Rigidbody2D bombRb = bombClone.GetComponent<Rigidbody2D>();
            GuidedGlideBomb bomb = bombClone.GetComponent<GuidedGlideBomb>();
            int layerValue = whatAreOurProjectiles.layermask_to_layer();
            TrailRenderer projTrail = bombClone.GetComponent<TrailRenderer>();
            projTrail.material = layerValue == 8 ? t1Mat : t2Mat;
            bombClone.gameObject.layer = layerValue;
            bomb.target = target;
            bomb.timeBeforeBoosters = Mathf.Clamp(bomb.timeBeforeBoosters - yOffsetDelay, 0.6f, 2f);
            bomb.whatIsTarget = whatIsTarget;
            bombRb.velocity = ownerRb.velocity;

            fireTimer = delayBetweenFire + Time.time;

            return FireState.Fired;
        }
        else if (Time.time <= fireTimer) return FireState.OnDelay;

        return FireState.Failed;
    }
}
