using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
#pragma warning disable 0649
public class WeaponGuidedSSMissileLauncher
{
    [Header("Weapon properties")]
    [SerializeField] private Transform missileFirePoint;
    [SerializeField] private Transform missilePrefab;
    [SerializeField] private float rocketYOffset;
    [SerializeField] private float missileDelay;

    [HideInInspector] public LayerMask whatIsTarget;
    [HideInInspector] public LayerMask whatAreOurProjectiles;
    [HideInInspector] public float rocketFireAngle;
    private float missileTimer;

    public void LaunchSSMissile()
    {
        if (Time.time > missileTimer)
        {
            Transform missileClone = Poolable.Get<GuidedSSMissile>(() => Poolable.CreateObj<GuidedSSMissile>(missilePrefab.gameObject), new Vector3(missileFirePoint.position.x + Random.Range(-0.2f, 0.2f), missileFirePoint.position.y, missileFirePoint.position.z), Quaternion.identity).transform;
            missileClone.localEulerAngles = new Vector3(0f, 0f, rocketFireAngle);
            int layerValue = whatAreOurProjectiles.layermask_to_layer();
            missileClone.gameObject.layer = layerValue;
            GuidedSSMissile missile = missileClone.GetComponent<GuidedSSMissile>();
            missile.whatIsTarget = whatIsTarget;
            missile.BoostStage();
            missileTimer = missileDelay + Time.time;
        }
    }
}
#pragma warning restore 0649