using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#pragma warning disable 0649
public class WeaponBaseCannon : WeaponBase
{
    public override FireState Fire()
    {
        bool isTargVisual = CheckIfLookingAtTarget(lookCheckRange);

        if (currentAmmo <= 0)
        {
            return FireState.OutOfAmmo;
        }

        if (isTargVisual)
        {
            if (Time.time > fireTimer)
            {
                Transform shellClone = Poolable.Get<CannonShell>(() => Poolable.CreateObj<CannonShell>(projectilePrefab.gameObject), spawnLocation.position, Quaternion.identity).transform;
                int layerValue = whatAreOurProjectiles.layermask_to_layer();
                shellClone.gameObject.layer = layerValue;
                shellClone.right = new Vector2(owner.up.x + UnityEngine.Random.Range(-inaccuracyOffset, inaccuracyOffset), owner.up.y).normalized; /*(target.position - transform.position).normalized*/; // i have no fucking idea what is going on at this point
                                                                                                                                                                                                          //Rigidbody2D shellRb = shellClone.GetComponent<Rigidbody2D>();
                shellClone.GetComponent<Rigidbody2D>().AddForce(owner.up * shellClone.GetComponent<CannonShell>().Speed * Time.deltaTime, ForceMode2D.Impulse);
                shellClone.GetComponent<TrailRenderer>().material = layerValue == 8 ? t1Mat : t2Mat;
                //SetTrail(shellClone);

                fireTimer = delayBetweenFire + Time.time + UnityEngine.Random.Range(-delayBetweenFire / 5f, delayBetweenFire / 3.4f);
                currentAmmo--;

                return FireState.Fired;
            }
            else
            {
                return FireState.OnDelay;
            }
        }
        else return FireState.Failed;
    }

    //private void SetTrail(Transform shellClone)
    //{
    //    Color32 trailCol = new Color32(Convert.ToByte(gameObject.layer == 8 ? 0 : 255), 0, Convert.ToByte(gameObject.layer == 8 ? 255 : 0), 255); // just assigns them via layer based on which team layer it is
    //    Gradient colrGrad = new Gradient();
    //    GradientColorKey[] colorKeys = new GradientColorKey[1];
    //    GradientAlphaKey[] alphaKeys = new GradientAlphaKey[1];
    //    alphaKeys[0] = new GradientAlphaKey(1f, 0f);
    //    colorKeys[0] = new GradientColorKey(trailCol, 0f);
    //    colrGrad.colorKeys = colorKeys;
    //    colrGrad.alphaKeys = alphaKeys;
    //    shellClone.GetComponent<TrailRenderer>().colorGradient = colrGrad;
    //}
}
#pragma warning restore 0649