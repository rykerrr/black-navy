using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649
public class UnitHumanoid : MonoBehaviour, IHealth
{
    [Header("Humanoid Properties")]
    [SerializeField] public UnitType type;
    [SerializeField] private Poolable thisUnit;
    [SerializeField] private Slider hpBar;
    [SerializeField] private int maxHealth;

    [SerializeField] private int health;

    public int Health { get { return health; } private set { health = value; } }
    public int MaxHealth { get { return maxHealth; } private set { maxHealth = value; } }

    private void Start()
    {
        hpBar.maxValue = MaxHealth;
        ChangeHealth(MaxHealth);
        health = maxHealth;
        thisUnit = GetComponent<Poolable>();
    }

    private void ChangeHealth(int newHealth)
    {
        if(newHealth <= 0)
        {
            LogUtils.DebugLog("rekt");
            Destroy(gameObject); // have to do this till i figure out a diff way, ReturnToPool fucks FindEnemy up in every other script
            // necessasry temporary casualty of war
        }

        health = newHealth;
        hpBar.value = health;
    }

    public void TakeDamage(IDamager damager)
    {
        ChangeHealth(health - damager.Damage);
    }
}
#pragma warning disable 0649