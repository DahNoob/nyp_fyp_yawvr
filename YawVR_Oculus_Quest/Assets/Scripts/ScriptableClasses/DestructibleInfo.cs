using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Destructible Info", menuName = "Entity Info/Destructible Info")]
public class DestructibleInfo : EntityInfo
{
    [Header("Base Destructible Info")]
    [Range(0.0f,10.0f)]
    public float scaleVariancyMin;
    [Range(0.0f, 10.0f)]
    public float scaleVariancyMax;
    [Header("Model Variants")]
    public Mesh[] meshVariants;
    public Material[] materialVariants;
}