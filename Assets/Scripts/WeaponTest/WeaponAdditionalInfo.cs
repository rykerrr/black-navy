using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class WeaponAdditionalInfo : MonoBehaviour
{
    public Sprite UIImage;
    public List<UnitBase> strongAgainst;

    [Multiline] public string desc;
}
#pragma warning restore 0649