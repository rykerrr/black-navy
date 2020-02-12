using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
#pragma warning disable 0649
public class WeaponBombDropper
{
    [Header("Weapon properties")]
    [SerializeField] private Transform bombBay;
    [SerializeField] private Transform bombPrefab;
    [SerializeField] private Transform owner;
    [SerializeField] private float bombDelay;
    [SerializeField] private float inaccuracyOffset;

    [HideInInspector] public LayerMask whatAreOurProjectiles;
    [HideInInspector] public Rigidbody2D ownerRb;
    private float bombTimer;

    public bool DropBomb()
    {
        if (Time.time > bombTimer)
        {
            Vector2 ranPos = new Vector2(Random.Range(bombBay.position.x - Random.Range(-inaccuracyOffset, inaccuracyOffset), bombBay.position.x + Random.Range(-inaccuracyOffset, inaccuracyOffset)), bombBay.position.y);
            Transform bombClone = Poolable.Get<UnguidedBomb>(() => Poolable.CreateObj<UnguidedBomb>(bombPrefab.gameObject), ranPos, Quaternion.identity).transform;
            int layerValue = whatAreOurProjectiles.layermask_to_layer();
            bombClone.gameObject.layer = layerValue;
            bombClone.right = new Vector2(owner.up.x + Random.Range(-inaccuracyOffset, inaccuracyOffset), owner.up.y).normalized/*(target.position - transform.position).normalized*/; // i have no fucking idea what is going on at this point
            Rigidbody2D bombRb = bombClone.GetComponent<Rigidbody2D>();
            bombRb.velocity = new Vector2(ownerRb.velocity.x / 1.5f, ownerRb.velocity.y / 1.1f);
            //Rigidbody2D shellRb = shellClone.GetComponent<Rigidbody2D>();
            bombTimer = bombDelay + Time.time;
            return true;
        }

        return false;
    }
}
#pragma warning restore 0649