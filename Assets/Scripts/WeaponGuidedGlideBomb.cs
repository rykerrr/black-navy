using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
#pragma warning disable 0649
public class WeaponGuidedGlideBomb
{
    [Header("Weapon properties")]
    [SerializeField] private Transform bombPrefab;
    [SerializeField] private Transform bombBay;
    [SerializeField] private Transform owner;
    [SerializeField] private Rigidbody2D thisRb;
    [SerializeField] private float dropDelay;

    [HideInInspector] public LayerMask whatAreOurProjectiles;
    [HideInInspector] public LayerMask whatIsTarget;

    private float dropTimer;

    public void DropBomb(Transform target)
    {
        float yOffsetDelay = Mathf.Clamp(1 / (30 / Mathf.Abs(owner.position.y)), 0f, 1f);
        Transform bombClone = Poolable.Get<GuidedGlideBomb>(() => Poolable.CreateObj<GuidedGlideBomb>(bombPrefab.gameObject), bombBay.position, owner.rotation).transform;
        Rigidbody2D bombRb = bombClone.GetComponent<Rigidbody2D>();
        GuidedGlideBomb bomb = bombClone.GetComponent<GuidedGlideBomb>();
        int layerValue = whatAreOurProjectiles.layermask_to_layer();
        bombClone.gameObject.layer = layerValue;
        bomb.target = target;
        bomb.timeBeforeBoosters = Mathf.Clamp(bomb.timeBeforeBoosters - yOffsetDelay, 0.6f, 2f);
        Debug.Log(bomb.timeBeforeBoosters + " | " + yOffsetDelay);
        bomb.whatIsTarget = whatIsTarget;
        bombRb.velocity = thisRb.velocity;

        dropTimer = dropDelay + Time.time;
    }
}
#pragma warning restore 0649