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
    [Header("Constraints")]
    [SerializeField]
    [Range(0, 180)]
    private float m_rightLimit = 80.0f;
    [SerializeField]
    [Range(0, 180)]
    private float m_leftLimit = 80.0f;
    [SerializeField]
    [Range(0, 180)]
    private float m_upperLimit = 80.0f;
    [SerializeField]
    [Range(0, 180)]
    private float m_lowerLimit = 80.0f;


    [Header("Debugs")]
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float m_armStretch = 0.0f;

    //Local variables
    private Animator animator;
    private float upperLimit;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }
    void Start()
    {
        m_leftLimit = 360.0f - m_leftLimit;
    }

    void Update()
    {
        float dist = (transform.position - m_followerTransform.position).magnitude;
        m_armStretch = dist / m_armLength;
        animator.SetFloat("Blend", m_armStretch);
        transform.LookAt(m_followerTransform);
        Vector3 asd = transform.localEulerAngles;
        transform.localEulerAngles = new Vector3(asd.x, asd.y > 180.0f ? Mathf.Max(m_leftLimit, asd.y) : Mathf.Min(m_rightLimit, asd.y), asd.z);
#if UNITY_EDITOR
        m_followerTransform.GetComponent<ControllerFollower>().m_armEuler = transform.localEulerAngles;
#endif
    }
}
