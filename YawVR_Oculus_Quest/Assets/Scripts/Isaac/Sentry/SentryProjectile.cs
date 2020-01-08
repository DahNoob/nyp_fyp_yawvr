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

    public override void Init(Transform _transform = null)
    {
        if (_transform != null)
            transform.SetPositionAndRotation(_transform.position, _transform.rotation);
        GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * m_projectileSpeed);
        StartCoroutine(delayDestroy());
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        GameObject efx = Instantiate(m_projectileImpactEffect, Persistent.instance.GO_DYNAMIC.transform);
        efx.transform.position = collision.GetContact(0).point;
        efx.GetComponent<ParticleSystem>().Emit(6);
        if (collision.collider)
        {
            EnemyBase eb = collision.collider.GetComponent<EnemyBase>() ?? collision.collider.GetComponentInChildren<EnemyBase>();
            if (eb)
            {
                eb.takeDamage(5);
            }
        }
        Destroy(gameObject);

        //GameObject efx = Instantiate(m_projectileImpactEffect, transform.position, Quaternion.identity);

        //efx.GetComponent<ParticleSystem>().Emit(20);

        //if (collision.gameObject.tag == "Enemy")
        //{
        //    if (collision.collider)
        //    {
        //        Destroy(this.gameObject);
        //    }
        //}
    }

    protected IEnumerator delayDestroy()
    {
        yield return new WaitForSeconds(m_lifeTime);
        Destroy(gameObject);
    }
}
