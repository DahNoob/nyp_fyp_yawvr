using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[RequireComponent(typeof(Rigidbody))]
abstract public class BaseProjectile : MonoBehaviour
{
    [Header("Base Configuration")]
    [SerializeField]
    protected string m_projectileName;

    abstract public void Init(Transform _transform);
    abstract protected void OnCollisionEnter(Collision collision);
}
