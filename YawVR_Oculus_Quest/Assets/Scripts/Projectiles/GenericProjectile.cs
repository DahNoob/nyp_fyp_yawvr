﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericProjectile : BaseProjectile
{
    [Header("Generic Configuration")]
    [SerializeField]
    protected float m_projectileSpeed = 14000.0f;
    [SerializeField]
    protected float m_lifeTime = 1.5f;
    [SerializeField]
    protected GameObject m_projectileImpactEffect;

    public override void Init(Transform _transform)
    {
        transform.SetPositionAndRotation(_transform.position, _transform.rotation);
        GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * m_projectileSpeed);
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        GameObject efx = Instantiate(m_projectileImpactEffect, Persistent.instance.GO_DYNAMIC.transform);
        efx.transform.position = collision.GetContact(0).point;
        efx.GetComponent<ParticleSystem>().Emit(6);
        Destroy(gameObject);
    }
}