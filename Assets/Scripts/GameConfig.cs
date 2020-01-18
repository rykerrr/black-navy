using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class GameConfig : Singleton<GameConfig>
{
    [Header("Game properties")]
    [SerializeField] private float waterLevel;
    [SerializeField] private float projectileLifetime;

    public float WaterLevel => waterLevel;
    public float ProjectileLifeTime => projectileLifetime;
}
#pragma warning restore 0649