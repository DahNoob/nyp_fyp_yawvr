using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechArmHandler : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField]
    private Transform m_followerTransform;
    [SerializeField]
    private float m_armLength = 3.6f;

    [Header("Debugs")]
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float m_armStretch = 0.0f;

    //Local variables
    private Animator animator;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }
    void Start()
    {
        
    }

    void Update()
    {
        float dist = (transform.position - m_followerTransform.position).magnitude;
        m_armStretch = dist / m_armLength;
        animator.SetFloat("Blend", m_armStretch);
        transform.LookAt(m_followerTransform);
    }
}
