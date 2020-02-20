using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
#pragma warning disable 0649
public class UnitLayerMask
{
    public UnitType[] mask;

    public static bool CheckIfUnitIsInMask(UnitType unitLayer, UnitLayerMask mask)
    {
        if (CheckIfMaskHasMultiples(mask.mask) == true)
        {
            Debug.LogError("Mask has doubles");
            Debug.Break();
        }

        int layer = 1 << (int)unitLayer;
        int maskTest = GetMask(mask.mask);

        int check = maskTest & layer;

        //Debug.Log("Layer: " + System.Convert.ToString(layer, 2).PadLeft(4, '0') + " Mask: " + System.Convert.ToString(maskTest, 2).PadLeft(4, '0') + " Check: " + System.Convert.ToString(check, 2).PadLeft(4, '0'));

        if (check > 0)
        {
            return true;
        }

        return false;
    }

    private static bool CheckIfMaskHasMultiples(UnitType[] array)
    {
        int[] possibles = new int[System.Enum.GetValues(typeof(UnitType)).Length];

        for (int i = 0; i < array.Length; i++)
        {
            for (int j = 0; j < System.Enum.GetValues(typeof(UnitType)).Length; j++)
            {
                if ((int)array[i] == j)
                {
                    possibles[j]++;
                }
            }
        }

        for (int i = 0; i < possibles.Length; i++)
        {
            if (possibles[i] > 1)
            {
                return true;
            }
        }

        return false;
    }

    private static int GetMask(UnitType[] array)
    {
        int ret = 0;

        for (int i = 0; i < array.Length; i++)
        {
            ret += 1 << (int)array[i];
        }

        return ret;
    }
}

public enum UnitType { Aircraft, Submarine, Ship, Base }
#pragma warning disable 0649