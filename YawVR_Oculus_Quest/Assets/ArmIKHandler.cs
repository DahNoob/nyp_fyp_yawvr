using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmIKHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Animator m_armModel;
    [SerializeField]
    private Transform m_handTransform;
    [SerializeField]
    private OVRInput.Controller m_controller;
    [SerializeField]
    private UnityEngine.UI.Text m_armLengthText;

    [Header("Debugs")]
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float m_armStretch = 0.0f;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //2.55
        float dist = (transform.position - m_handTransform.position).magnitude;
        if (m_armLengthText)
            m_armLengthText.text = dist.ToString();
        m_armStretch = dist / 2.55f;
        m_armModel.SetFloat("Blend", m_armStretch);
        transform.LookAt(m_handTransform);
    }
}
