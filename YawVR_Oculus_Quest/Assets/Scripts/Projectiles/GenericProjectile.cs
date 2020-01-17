using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericProjectile : BaseProjectile, IPooledObject
{
    [Header("Generic Configuration")]
    [SerializeField]
    protected GenericProjectileInfo m_projectileInfo;
    //[SerializeField]
    //protected float m_projectileSpeed = 14000.0f;
    //[SerializeField]
    //protected float m_lifeTime = 1.5f;

    [Header("Components")]
    [SerializeField]
    private Rigidbody m_rigidbody;
    [SerializeField]
    private TrailRenderer m_trailRenderer;

    public void OnObjectSpawn()
    {
        //Reset normal and angular velocities
        m_rigidbody.velocity = Vector3.zero;
        m_rigidbody.angularVelocity = Vector3.zero;
        //Clears the renderer positions so the trail stops acting out
        m_trailRenderer.Clear();
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
       m_rigidbody.AddForce(transform.forward * m_projectileInfo.speed);
        StartCoroutine(delayDestroy());
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        //GameObject efx = Instantiate(m_projectileInfo.impactEffect, Persistent.instance.GO_DYNAMIC.transform);
        GameObject efx = ObjectPooler.instance.SpawnFromPool(PoolObject.OBJECTTYPES.PLAYER_PROJECTILE_IMPACT, collision.GetContact(0).point, Quaternion.identity);
        //efx.transform.position = collision.GetContact(0).point;
        //efx.GetComponent<ParticleSystem>().Emit(6);
        if (collision.collider)
        {
            BaseEntity eb = collision.collider.GetComponent<BaseEntity>() ?? collision.collider.GetComponentInChildren<BaseEntity>();
            if (eb)
            {
                eb.takeDamage(m_projectileInfo.damage);
            }
        }
        OnObjectDestroy();
        //Destroy(gameObject);
    }

    protected IEnumerator delayDestroy()
    {
        yield return new WaitForSeconds(m_projectileInfo.lifeTime);
        //Destroy(gameObject);
        OnObjectDestroy();
    }
}
