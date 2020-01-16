using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LooseDestructible : BaseEntity
{
    [Header("Destructible Stats")]
    [SerializeField]
    protected DestructibleInfo m_destructibleInfo;
    [SerializeField]
    protected bool isRooted = true;

    void Start()
    {
        gameObject.GetComponent<Rigidbody>().constraints = isRooted ? RigidbodyConstraints.FreezeAll : RigidbodyConstraints.None;
        transform.localScale *= Random.Range(m_destructibleInfo.scaleVariancyMin, m_destructibleInfo.scaleVariancyMax);
        health = m_destructibleInfo.maxHealth;
        int modelIndex = Random.Range(0, m_destructibleInfo.meshVariants.Length);
        GetComponent<MeshFilter>().mesh = m_destructibleInfo.meshVariants[modelIndex];
        GetComponent<MeshRenderer>().material = m_destructibleInfo.materialVariants[modelIndex];
    }

    public override void Die()
    {
        Instantiate(m_destructibleInfo.dieEffect, transform.position, Quaternion.identity, Persistent.instance.GO_DYNAMIC.transform);
        Destroy(gameObject);
    }

    public override void takeDamage(int damage)
    {
        health -= damage;
        if(isRooted && health < m_destructibleInfo.maxHealth)
        {
            gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            isRooted = false;
        }
        if (health <= 0)
            Die();
    }
}
