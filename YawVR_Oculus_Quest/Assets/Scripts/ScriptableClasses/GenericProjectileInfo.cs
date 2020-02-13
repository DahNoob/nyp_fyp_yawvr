using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This class is used as a base for creating a ScriptableObject asset for an projectile's base info
/// </summary>
[CreateAssetMenu(fileName = "Generic Projectile Info", menuName = "Projectile Info/Generic Projectile Info")]
public class GenericProjectileInfo : ScriptableObject
{
    [Header("Generic Projectile Info")]
    public PoolObject.OBJECTTYPES projectileType;
    public string projectileName;
    public float speed = 19000;
    public float speedVariancy = 0;
    public float lifeTime = 1.5f;
    //public GameObject impactEffect;
    public PoolObject.OBJECTTYPES impactType;
    public int damage = 5;
}
