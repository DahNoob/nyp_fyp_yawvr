using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class BaseStructure : BaseEntity
{
    [Header("Structure Configuration")]
    [SerializeField]
    protected StructureInfo m_structureInfo;
    [SerializeField]
    protected MeshRenderer m_meshRenderer;

    //Local variables
    private Vector3 origPos;
    private bool isShaking;

    protected void Start()
    {
        health = m_structureInfo.maxHealth;
        origPos = transform.position;
        isShaking = false;
    }
    public override void Die()
    {
        Instantiate(m_structureInfo.dieEffect, transform.position, Quaternion.identity, Persistent.instance.GO_DYNAMIC.transform);
        InvokeDie();
        Destroy(gameObject);
    }

    public override void takeDamage(int damage)
    {
        if(health > 0)
        {
            health -= damage;
            if (health <= 0)
                Die();
            else if (!isShaking)
                StartCoroutine(shake());
        }
        
    }

    void SetFlash(bool _isFlashing)
    {
        if (_isFlashing)
        {
            //for (int i = 0; i < meshRenderers.Length; ++i)
            //{
            //    meshRenderers[i].material = Persistent.instance.MAT_WHITE;
            //}
            m_meshRenderer.material = Persistent.instance.MAT_WHITE;
        }
        else
        {
            //for (int i = 0; i < meshRenderers.Length; ++i)
            //{
            //    meshRenderers[i].material = m_structureInfo.material;
            //}
            m_meshRenderer.material = m_structureInfo.material;
        }
    }

    IEnumerator shake()
    {
        float lolz = 0.1f;
        isShaking = true;
        SetFlash(true);
        while (lolz > 0)
        {
            transform.position = origPos + new Vector3(Random.Range(-lolz, lolz), Random.Range(-lolz, lolz), Random.Range(-lolz, lolz));
            lolz -= 0.05f;
            yield return new WaitForSeconds(0.025f);
        }
        isShaking = false;
        SetFlash(false);
        transform.position = origPos;
    }
}