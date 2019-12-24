using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
[RequireComponent(typeof(UnitBase))]
public class UnitHumanoid : MonoBehaviour, IHealth
{
    [Header("Humanoid Properties")]
    [SerializeField] public UnitType type;
    [SerializeField] private int maxHealth;

    private int health;

    public int Health { get { return health; } private set { health = value; } }
    public int MaxHealth { get { return maxHealth; } private set { maxHealth = value; } }

    public void ChangeHealth(int newHealth)
    {
        Health = newHealth;

        if(Health <= 0)
        {
            Debug.Log("Yeeted!");
        }
    }
}
#pragma warning disable 0649