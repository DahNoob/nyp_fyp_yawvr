using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Entity Info", menuName = "Entity Info/Entity Info")]
public class EntityInfo : ScriptableObject
{
    [Header("Base Entity Info")]
    public int maxHealth = 100;
    public GameObject dieEffect;
}