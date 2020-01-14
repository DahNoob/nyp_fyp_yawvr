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
        //transform.localScale += Vector3.Scale(m_destructibleInfo.scaleVariancy, new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)));
        health = m_destructibleInfo.maxHealth;
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
