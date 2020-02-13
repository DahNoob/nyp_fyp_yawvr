using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is used as a base for creating a ScriptableObject asset for an entity's base info.
/// </summary>
[CreateAssetMenu(fileName = "Entity Info", menuName = "Entity Info/Entity Info")]
public class EntityInfo : ScriptableObject
{
    [Header("Base Entity Info")]
    public int maxHealth = 100;
    public GameObject dieEffect;
    public Sprite defaultIcon;
}