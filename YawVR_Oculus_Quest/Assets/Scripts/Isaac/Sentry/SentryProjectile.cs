using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentryProjectile : BaseProjectile , IPooledObject
{
    [Header("Generic Configuration")]
    [SerializeField]
    protected GenericProjectileInfo m_projectileInfo;
    //[SerializeField]
    //protected float m_projectileSpeed = 14000.0f;
    //[SerializeField]
    //protected float m_lifeTime = 1.5f;
    [SerializeField]
    protected GameObject m_projectileImpactEffect;

    public void OnObjectSpawn()
    {
        //Reset velocity
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        GetComponentInChildren<TrailRenderer>().Clear();
        StartCoroutine(delayDestroy());
    }
    public void OnObjectDestroy()
    {
        ObjectPooler.instance.DisableInPool(PoolObject.OBJECTTYPES.PLAYER_PROJECTILE);
        this.gameObject.SetActive(false);
    }


    public override void Init(Transform _transform = null)
    {
        if (_transform != null)
            transform.SetPositionAndRotation(_transform.position, _transform.rotation);
        GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * m_projectileInfo.speed);
        StartCoroutine(delayDestroy());
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        //GameObject efx = Instantiate(m_projectileInfo.impactEffect, Persistent.instance.GO_DYNAMIC.transform);
        //efx.transform.position = collision.GetContact(0).point;
        //efx.GetComponent<ParticleSystem>().Emit(6);
        //if (collision.collider)
        //{
        //    EnemyBase eb = collision.collider.GetComponent<EnemyBase>() ?? collision.collider.GetComponentInChildren<EnemyBase>();
        //    if (eb)
        //    {
        //        eb.takeDamage(m_projectileInfo.damage);
        //    }
        //}
        //Destroy(gameObject);

        //GameObject efx = Instantiate(m_projectileImpactEffect, transform.position, Quaternion.identity);
        GameObject efx = ObjectPooler.instance.SpawnFromPool(PoolObject.OBJECTTYPES.PLAYER_PROJECTILE_IMPACT, collision.GetContact(0).point, Quaternion.identity);
        efx.GetComponent<ParticleSystem>().Emit(20);

        if (collision.gameObject.tag == "Enemy")
        {
            if (collision.collider)
            {
                //Destroy(this.gameObject);
                OnObjectDestroy();
            }
        }
    }

    protected IEnumerator delayDestroy()
    {
        yield return new WaitForSeconds(0.5f);
        //Destroy(gameObject);
        OnObjectDestroy();
    }
}
