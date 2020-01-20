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
        health = m_destructibleInfo.maxHealth + m_destructibleInfo.additionalHealth;
        int modelIndex = Random.Range(0, m_destructibleInfo.meshVariants.Length);
        GetComponent<MeshFilter>().mesh = m_destructibleInfo.meshVariants[modelIndex];
        GetComponent<MeshRenderer>().material = m_destructibleInfo.materialVariants[modelIndex];
        GetComponent<MeshCollider>().sharedMesh = m_destructibleInfo.meshVariants[modelIndex];
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
        else
        {
            StartCoroutine(shake());
        }
        if (health <= 0)
            Die();
    }

    IEnumerator shake()
    {
        Vector3 origP = transform.position;
        float lolz = 0.05f;
        while(lolz > 0 && isRooted)
        {
            transform.position = origP + new Vector3(Random.Range(-lolz, lolz), Random.Range(-lolz, lolz), Random.Range(-lolz, lolz));
            lolz -= 0.17f;
            yield return new WaitForSeconds(0.025f);
        }
        transform.position = origP;
    }
}
