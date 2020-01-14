using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Generic Projectile Info", menuName = "Projectile Info/Generic Projectile Info")]
public class GenericProjectileInfo : ScriptableObject
{
    [Header("Generic Projectile Info")]
    public string projectileName;
    public float speed = 19000;
    public float lifeTime = 1.5f;
    public GameObject impactEffect;
    public int damage = 5;
}
