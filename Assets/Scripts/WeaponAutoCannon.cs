using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
#pragma warning disable 0649
public class WeaponAutoCannon
{
    [Header("Weapon properties")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform shellPrefab;
    [SerializeField] private Transform owner;
    [SerializeField] private float cannonDelay;
    [SerializeField] private float cannonReloadTime;
    [SerializeField] private float inaccuracyOffset;
    [SerializeField] private int maxShellsPerMag;

    [HideInInspector] public LayerMask whatAreOurProjectiles;
    [HideInInspector] public float currentShells;

    private float cannonTimer;

    public float ReloadTime => cannonReloadTime;
    public int MaxShells => maxShellsPerMag;

    public bool FireCannon(float inaccuracyOffset)
    {
        if (inaccuracyOffset == 0)
        {
            inaccuracyOffset = this.inaccuracyOffset;
        }

        if (currentShells <= 0)
        {
            return false; // means you have to reload
        }

        if (Time.time > cannonTimer)
        {
            Transform shellClone = Poolable.Get<CannonShell>(() => Poolable.CreateObj<CannonShell>(shellPrefab.gameObject), firePoint.position, Quaternion.identity).transform;
            int layerValue = whatAreOurProjectiles.layermask_to_layer();
            shellClone.gameObject.layer = layerValue;
            shellClone.right = new Vector2(owner.up.x + Random.Range(-inaccuracyOffset, inaccuracyOffset), owner.up.y).normalized; /*(target.position - transform.position).normalized*/; // i have no fucking idea what is going on at this point
            //Rigidbody2D shellRb = shellClone.GetComponent<Rigidbody2D>();
            cannonTimer = cannonDelay + Time.time;
            currentShells--;

            return true;
        }

        return false;
    }
}
#pragma warning restore 0649