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
    [SerializeField] private int maxShellsPerMag;

    [HideInInspector] public LayerMask whatAreOurProjectiles;
    [HideInInspector] public float currentShells;

    private Rigidbody2D thisRb;
    private float cannonTimer;

    public bool FireCannon(Transform target, float inaccuracyOffset)
    {
        if (currentShells <= 0)
        {
            return false; // means you have to reload
        }

        if (Time.time > cannonTimer)
        {
            Transform shellClone = Poolable.Get<CannonShell>(() => Poolable.CreateObj<CannonShell>(shellPrefab.gameObject)).transform;
            shellClone.right = new Vector2(owner.up.x + Random.Range(-inaccuracyOffset, inaccuracyOffset), owner.up.y); /*(target.position - transform.position).normalized*/;
            int layerValue = whatAreOurProjectiles.layermask_to_layer();
            shellClone.gameObject.layer = layerValue;
            Rigidbody2D shellRb = shellClone.GetComponent<Rigidbody2D>();
            shellRb.velocity = thisRb.velocity;
            cannonTimer = cannonDelay + Time.time;
            currentShells--;
        }

        return true;
    }
}
#pragma warning restore 0649