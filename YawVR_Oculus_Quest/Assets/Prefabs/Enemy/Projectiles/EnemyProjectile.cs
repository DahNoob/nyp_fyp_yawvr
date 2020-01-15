using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : BaseProjectile , IPooledObject
{
    protected float m_projectileSpeed = 10.0f;
    protected GameObject m_projectileImpactEffect;

    public void OnObjectSpawn()
    {
        //Reset normal and angular velocities
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        //Clears the renderer positions so the trail stops acting out
        GetComponentInChildren<TrailRenderer>().Clear();
    }
    public void OnObjectDestroy()
    {
        this.gameObject.SetActive(false);
    }

    public override void Init(Transform _transform = null)
    {
        if (_transform != null)
            transform.SetPositionAndRotation(_transform.position, _transform.rotation);
        GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * m_projectileSpeed);
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Mech")
        {
            Debug.Log("PLAYER HIT");
            //GameObject efx = Instantiate(m_projectileImpactEffect, Persistent.instance.GO_DYNAMIC.transform);
            GameObject efx = ObjectPooler.instance.SpawnFromPool("PlayerProjectileImpact", collision.GetContact(0).point, Quaternion.identity);
            efx.transform.position = collision.GetContact(0).point;
            efx.GetComponent<ParticleSystem>().Emit(6);
            OnObjectDestroy();
        }
    }


}
