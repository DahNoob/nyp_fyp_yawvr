using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestProjectile : BaseProjectile , IPooledObject
{
    [Header("Generic Configuration")]
    [SerializeField]
    protected GenericProjectileInfo m_projectileInfo;
    //[SerializeField]
    //protected float m_projectileSpeed = 14000.0f;
    //[SerializeField]
    //protected float m_lifeTime = 1.5f;

        [SerializeField]
    private Rigidbody rb;
    [SerializeField]
    private TrailRenderer tr;

    public void OnObjectSpawn()
    {
        //Reset normal and angular velocities
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        //Clears the renderer positions so the trail stops acting out
        tr.Clear();
       // StartCoroutine(delayDestroy());
    }
    public void OnObjectDestroy()
    {
        ObjectPooler.instance.DisableInPool(PoolObject.OBJECTTYPES.TEST_PROJECTILE);
        this.gameObject.SetActive(false);
    }

    public override void Init(Transform _transform = null)
    {
        if (_transform != null)
            transform.SetPositionAndRotation(_transform.position, _transform.rotation);
        rb.AddForce(transform.forward * m_projectileInfo.speed);
       // StartCoroutine(delayDestroy());
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
    }

    protected IEnumerator delayDestroy()
    {
        yield return new WaitForSeconds(m_projectileInfo.lifeTime);
        //Destroy(gameObject);
        OnObjectDestroy();
    }
}
