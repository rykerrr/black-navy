using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class GameConfig : Singleton<GameConfig>
{
    [Header("Game properties")]
    [SerializeField] private float waterLevel;

    public float WaterLevel => waterLevel;
}
#pragma warning restore 0649