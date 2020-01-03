using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentryProjectile : BaseProjectile
{
    [Header("Generic Configuration")]
    [SerializeField]
    protected float m_projectileSpeed = 14000.0f;
    [SerializeField]
    protected float m_lifeTime = 1.5f;
    [SerializeField]
    protected GameObject m_projectileImpactEffect;

    public float projectileSpeed
    {
        get
        {
            return m_projectileSpeed;
        }
        set
        {
            m_projectileSpeed = value;
        }
    }

    public override void Init(Transform _transform = null)
    {
        if (_transform != null)
            transform.SetPositionAndRotation(_transform.position, _transform.rotation);
        GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * m_projectileSpeed);

    }

    protected override void OnCollisionEnter(Collision collision)
    {
        GameObject efx = Instantiate(m_projectileImpactEffect, transform.position,Quaternion.identity);
        efx.transform.position = collision.GetContact(0).point;
        
        efx.GetComponent<ParticleSystem>().Emit(20);

        if(collision.gameObject.tag == "Enemy")
        {
            Debug.Log("hit enemy");
        }

        Destroy(gameObject);
    }
}
