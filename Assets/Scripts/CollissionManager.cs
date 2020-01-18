using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class CollissionManager : MonoBehaviour
{
    private void Start()
    {
        Physics2D.IgnoreLayerCollision(10, 10); // friendly ships
        Physics2D.IgnoreLayerCollision(8, 8); // friendly projectiles
        Physics2D.IgnoreLayerCollision(9, 9); // enemy ships
        Physics2D.IgnoreLayerCollision(9, 10); // friendly ships and enemy ships
        Physics2D.IgnoreLayerCollision(11, 11); // enemy projectiles
        Physics2D.IgnoreLayerCollision(9, 11); // enemy ships and enemy projectiles
        Physics2D.IgnoreLayerCollision(10, 8); // friendly ships and friendly projectiles
        Physics2D.IgnoreLayerCollision(8, 11); // friendly projectiles and enemy projectiles
    }
}
#pragma warning restore 0649