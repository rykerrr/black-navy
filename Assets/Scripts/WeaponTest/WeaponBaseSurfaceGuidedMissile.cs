using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class WeaponBaseSurfaceGuidedMissile : WeaponBase
{
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

    }

    public override FireState Fire()
    {
        if (target)
        {
            if (!target.gameObject.activeInHierarchy)
            {
                target = null;
                return FireState.Failed;
            }

            if (Time.time > fireTimer)
            {
                Transform missileClone = Poolable.Get<GuidedSSMissile>(() => Poolable.CreateObj<GuidedSSMissile>(projectilePrefab.gameObject), new Vector3(spawnLocation.position.x + Random.Range(-0.2f, 0.2f), spawnLocation.position.y, spawnLocation.position.z), Quaternion.identity).transform;
                int layerValue = whatAreOurProjectiles.layermask_to_layer();
                missileClone.gameObject.layer = layerValue;
                TrailRenderer projTrail = missileClone.GetComponent<TrailRenderer>();
                projTrail.material = layerValue == 8 ? t1Mat : t2Mat;
                projTrail.Clear();
                GuidedSSMissile missile = missileClone.GetComponent<GuidedSSMissile>();
                missile.target = target;
                missile.whatIsTarget = whatIsTarget;
                missile.BoostStage();
                fireTimer = delayBetweenFire + Time.time;
                soundMngr.PlayEnviroSound(spawnLocation.gameObject, "missile1", 10f);   

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