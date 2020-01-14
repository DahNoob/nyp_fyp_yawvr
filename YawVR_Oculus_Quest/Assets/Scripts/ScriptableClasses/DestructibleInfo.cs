using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Destructible Info", menuName = "Entity Info/Destructible Info")]
public class DestructibleInfo : EntityInfo
{
    [Header("Base Destructible Info")]
    public Vector3 scaleVariancy;
}