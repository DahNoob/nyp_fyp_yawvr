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
    public bool m_followCameraRotation = true;

    [Header("Configuration")]
    public OVRInput.Controller m_controller;
    public Vector3 m_offsetPosition = Vector3.zero;
    public Vector3 m_offsetScale = Vector3.one;
    [Range(0.0f, 1.0f)]
    public float m_followSpeed = 0.05f;
    public Transform m_origin;

    //Local variables
    private Vector3 prevPosition, goalPosition, currPosition;
    private Quaternion prevRotation, goalRotation, currRotation;

    void Start()
    {
        prevPosition = gameObject.transform.localPosition;
        print(m_origin.localEulerAngles);
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
        else if(m_origin)
        {
            float camRot = (m_followCameraRotation ? Camera.main.transform.localEulerAngles.y : 0.0f);
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
        currPosition += _posOffset;
        currRotation *= Quaternion.Euler(_rotOffset);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (prevPosition != gameObject.transform.localPosition)
        {
            float magnitude = CalculateFollowerSpeed();
            float shakeStrength = Mathf.Lerp(0.0f, 1.0f, magnitude);
            //print("shakeStrength : " + shakeStrength);
            //VibrationManager.SetControllerVibration(m_controller, 8, 4, shakeStrength);
            VibrationManager.SetControllerVibration(m_controller, 0.01f, magnitude);
        }
    }
}
