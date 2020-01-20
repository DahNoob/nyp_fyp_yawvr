using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class BaseStructure : BaseEntity
{
    [Header("Structure Configuration")]
    [SerializeField]
    protected StructureInfo m_structureInfo;

    //Local variables
    private Vector3 origPos;

    protected void Start()
    {
        health = m_structureInfo.maxHealth;
        origPos = transform.position;
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
        else
            StartCoroutine(shake());
    }

    IEnumerator shake()
    {
        float lolz = 0.05f;
        while (lolz > 0)
        {
            transform.position = origPos + new Vector3(Random.Range(-lolz, lolz), Random.Range(-lolz, lolz), Random.Range(-lolz, lolz));
            lolz -= 0.17f;
            yield return new WaitForSeconds(0.025f);
        }
        transform.position = origPos;
    }
}