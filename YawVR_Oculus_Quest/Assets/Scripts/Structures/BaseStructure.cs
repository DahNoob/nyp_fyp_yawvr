using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class BaseStructure : BaseEntity
{
    [Header("Structure Configuration")]
    [SerializeField]
    protected StructureInfo m_structureInfo;

    protected void Start()
    {
        health = m_structureInfo.maxHealth;
    }
    public override void Die()
    {
        Instantiate(m_structureInfo.dieEffect, transform.position, Quaternion.identity, Persistent.instance.GO_DYNAMIC.transform);
        InvokeDie();
        Destroy(gameObject);
    }

    public override void takeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
            Die();
    }
}