using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class defines the properties an GenericProjectile should have.
/// Inherits from BaseProjectile, IPooledObject.
/// </summary>
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

    /// <summary>
    /// Called when this projectile is spawned from the object pool.
    /// </summary>
    public void OnObjectSpawn()
    {
        //Reset normal and angular velocities
        m_rigidbody.velocity = Vector3.zero;
        m_rigidbody.angularVelocity = Vector3.zero;
        //Clears the renderer positions so the trail stops acting out
        m_trailRenderer.Clear();
    }

    /// <summary>
    /// Called when this projectile should be "destroyed"
    /// </summary>
    public void OnObjectDestroy()
    {
        ObjectPooler.instance.DisableInPool(m_projectileInfo.projectileType);
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// Init function for projectile.
    /// </summary>
    /// <param name="_transform"></param>
    public override void Init(Transform _transform = null)
    {
        if (_transform != null)
            transform.SetPositionAndRotation(_transform.position, _transform.rotation);
        m_rigidbody.AddForce(transform.forward * (m_projectileInfo.speed + Random.Range(-m_projectileInfo.speedVariancy,m_projectileInfo.speedVariancy)));
        StartCoroutine(delayDestroy());
    }

    /// <summary>
    /// Defines what happens when this projectile collides with collision.
    /// </summary>
    /// <param name="collision">The object that is colliding with this projectile.</param>
    protected override void OnCollisionEnter(Collision collision)
    {
        //GameObject efx = Instantiate(m_projectileInfo.impactEffect, Persistent.instance.GO_DYNAMIC.transform);
        GameObject efx = ObjectPooler.instance.SpawnFromPool(m_projectileInfo.impactType, collision.GetContact(0).point, Quaternion.identity);
        efx.GetComponent<ImpactSound>()?.StartImpact();
        //efx.transform.position = collision.GetContact(0).point;
        //efx.GetComponent<ParticleSystem>().Emit(6);
        if (collision.collider)
        {
            BaseEntity eb = collision.collider.GetComponent<BaseEntity>();
            if (eb)
            {
                //print(m_projectileInfo.projectileName + " damage is " + m_projectileInfo.damage);
                eb.takeDamage(m_projectileInfo.damage);
                //print(eb.name + " is " + eb.GetHealth());
               // PlayerUISoundManager.instance.PlaySound(PlayerUISoundManager.UI_SOUNDTYPE.HIT_MARKER, 0, 0.1f);
            }
        }
        OnObjectDestroy();
        //Destroy(gameObject);
    }

    /// <summary>
    /// Coroutine that destroys the projectile after it has not hit something, to save resources.
    /// </summary>
    /// <returns></returns>
    protected IEnumerator delayDestroy()
    {
        yield return new WaitForSeconds(m_projectileInfo.lifeTime);
        if(gameObject.activeSelf)
            OnObjectDestroy();
        //Destroy(gameObject);
    }
}
