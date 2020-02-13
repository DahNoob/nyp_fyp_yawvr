using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Abstract base class for projectiles.
/// </summary>
[System.Serializable]
[RequireComponent(typeof(Rigidbody))]
abstract public class BaseProjectile : MonoBehaviour
{
    //[Header("Base Configuration")]
    //[SerializeField]
    //protected string m_projectileName;

    /// <summary>
    /// Inits the projectile based on a given transform, normally to copy the forward vector for firing of projectiles.
    /// </summary>
    /// <param name="_transform">Target transform</param>
    abstract public void Init(Transform _transform = null);

    /// <summary>
    /// On Collision Enter based on Unity's functions.
    /// </summary>
    /// <param name="collision">Reference collision.</param>
    abstract protected void OnCollisionEnter(Collision collision);
}
