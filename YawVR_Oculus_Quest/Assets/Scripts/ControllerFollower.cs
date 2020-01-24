using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************  
** Name: ControllerFollower monobehaviour
** Desc: Just follows the controller's position and rotation
** Author: DahNoob
** Date: 23/12/2019, 11:48PM
**************************
** Change History
**************************
** PR   Date                    Author    Description 
** --   --------                -------   ------------------------------------
** 1    23/12/2019, 11:48PM     DahNoob   Created (ish), actually duplicated from MechHandHandler
*******************************/
public class ControllerFollower : MonoBehaviour
{
    [Header("Settings")]
    public bool m_enabled = false;

    [Header("Configuration")]
    public OVRInput.Controller m_controller;
    public Vector3 m_offsetPosition = Vector3.zero;
    public Vector3 m_offsetScale = Vector3.one;
    [Range(0.0f, 1.0f)]
    public float m_followSpeed = 0.05f;
    public Transform m_origin;

    [Header("Debug")]
    public Vector3 m_armEuler;

    //Local variables
    private Vector3 prevPosition, goalPosition, currPosition;
    private Quaternion prevRotation, goalRotation, currRotation;

    void Start()
    {
        prevPosition = gameObject.transform.localPosition;
    }

    void Update()
    {
        Vector3 controllerPos = OVRInput.GetLocalControllerPosition(m_controller);

        if (m_enabled)
        {
            Transform t = GetComponent<Transform>();           
            goalPosition = /*Quaternion.AngleAxis(m_playerTransform.localEulerAngles.y, Vector3.up) * */(Vector3.Scale(controllerPos,m_offsetScale) + m_offsetPosition);
            goalRotation = OVRInput.GetLocalControllerRotation(m_controller);
        }
        else
        {
            float camRot = Camera.main.transform.localEulerAngles.y;
            Transform t = GetComponent<Transform>();
            goalPosition = /*Quaternion.AngleAxis(m_playerTransform.localEulerAngles.y, Vector3.up) * */m_origin.localPosition;
            goalRotation = m_origin.localRotation;
        }
    }

    void FixedUpdate()
    {
        prevPosition = currPosition;
        prevRotation = currRotation;

        currPosition = Vector3.LerpUnclamped(currPosition, goalPosition, m_followSpeed);
        currRotation = Quaternion.SlerpUnclamped(currRotation, goalRotation, m_followSpeed);
    }

    void LateUpdate()
    {
        transform.localPosition =/* m_playerTransform.localPosition + */currPosition;//Vector3.LerpUnclamped(transform.localPosition, currPosition, m_followSpeed);
        transform.localRotation = /*m_playerTransform.localRotation * */currRotation;//Quaternion.SlerpUnclamped(transform.localRotation, currRotation, m_followSpeed);
    }

    public float CalculateFollowerSpeed()
    {
        return (currPosition - prevPosition).sqrMagnitude;
    }

    public void Bump(Vector3 _posOffset, Vector3 _rotOffset = new Vector3())
    {
        currPosition += Vector3.RotateTowards(_posOffset, transform.forward, 0.0f, 0.0f);
        currRotation *= Quaternion.Euler(_rotOffset);
    }
}
