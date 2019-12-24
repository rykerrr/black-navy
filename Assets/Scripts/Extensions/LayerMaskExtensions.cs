using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public static class LayerMaskExtensions
{
    public static int layermask_to_layer(this LayerMask layerMask)
    {
        int layerNumber = 0;
        int layer = layerMask.value;
        while (layer > 0)
        {
            layer = layer >> 1;
            layerNumber++;
        }
        return layerNumber - 1;
    }
}
#pragma warning restore 0649