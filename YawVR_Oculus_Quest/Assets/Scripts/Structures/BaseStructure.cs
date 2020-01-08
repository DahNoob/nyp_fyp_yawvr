using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class BaseStructure : BaseEntity
{
    public delegate void OnStructureDestroy(BaseStructure _structure);
    public event OnStructureDestroy onStructureDestroy;

    [Header("Base Structure Configuration")]
    [SerializeField]
    private StructureInfo m_structureInfo;

    void Start()
    {
        health = m_structureInfo.health;
    }
    public override void Die()
    {
        Instantiate(m_structureInfo.destroyEffect, transform.position, Quaternion.identity, Persistent.instance.GO_DYNAMIC.transform);
        onStructureDestroy?.Invoke(this);
        Destroy(gameObject);
    }

    public override void takeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
            Die();
    }
}