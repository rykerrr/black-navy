using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class BitmasksAndShifting : MonoBehaviour
{
    [Header("Properties")]
    [Range(0, 31)]
    [SerializeField] private int unitLayer;
    [Range(0, 31)]
    [SerializeField] private int unit2Layer;

    private int bitMask;

    private void Start()
    {
        bitMask = 1 << unitLayer | 1 << unit2Layer;
        //Debug.Log("Lr: " + unitLayer + " Mask: " + System.Convert.ToString(bitMask, 2).PadLeft(32, '0'));
    }
}
#pragma warning disable 0649