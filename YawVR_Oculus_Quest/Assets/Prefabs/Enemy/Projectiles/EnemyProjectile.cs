using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : BaseProjectile, IPooledObject
{
    protected float m_projectileSpeed = 10.0f;
    protected GameObject m_projectileImpactEffect;

    [Header("Components")]
    [SerializeField]
    private Rigidbody m_rigidbody;
    [SerializeField]
    private TrailRenderer m_trailRenderer;
    private float bulletDamage = 2.0f;

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
        this.gameObject.SetActive(false);
    }

    public override void Init(Transform _transform = null)
    {
        if (_transform != null)
            transform.SetPositionAndRotation(_transform.position, _transform.rotation);
        m_rigidbody.AddRelativeForce(Vector3.forward * m_projectileSpeed);
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Mech")
        {
            //Debug.Log("PLAYER HIT");
            //GameObject efx = Instantiate(m_projectileImpactEffect, Persistent.instance.GO_DYNAMIC.transform);
            GameObject efx = ObjectPooler.instance.SpawnFromPool(PoolObject.OBJECTTYPES.PLAYER_PROJECTILE_IMPACT, collision.GetContact(0).point, Quaternion.identity);
            efx.transform.position = collision.GetContact(0).point;
            efx.GetComponent<ParticleSystem>().Emit(6);
            PlayerHandler.instance.takeDamage((int)bulletDamage);
            OnObjectDestroy();
        }
    }


}
