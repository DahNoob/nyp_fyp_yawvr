using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************  
** Name: Controller Follower behaviour
** Desc: Follows the movement of Oculus Controllers in the world
** Author: DahNoob
** Date: 02/12/2019, 2:05 PM
**************************
** Change History
**************************
** PR   Date                    Author    Description 
** --   --------                -------   ------------------------------------
** 1    02/12/2019, 2:05PM      DahNoob   Created
** 2    02/12/2019, 2:43PM      DahNoob   Implemented Version 1
** 3    03/12/2019, 3:41PM      DahNoob   Implemented Version 2 (One-time attach, HandTrigger to control)
*******************************/
public class ControllerFollower : MonoBehaviour
{
    [Header("Settings")]
    public bool m_enabled = true;

    [Header("Configuration")]
    public OVRInput.Controller m_controller;
    public Vector3 m_offsetPosition = Vector3.zero;
    public Vector3 m_offsetScale = Vector3.one;
    [Range(0.0f, 20.0f)]
    public float m_followSpeed = 8.0f;
    public Transform m_origin;

    [Header("References")]
    [SerializeField]
    private Transform m_playerTransform;

    //Local variables
    private Vector3 prevPosition, currPosition;
    private Quaternion currRotation;

    void Start()
    {
        prevPosition = gameObject.transform.localPosition;
    }

    void Update()
    {
        Vector3 controllerPos = OVRInput.GetLocalControllerPosition(m_controller);
        Vector3 objectPosGoal;
        Quaternion objectRotGoal;
        if (m_enabled)
        {
            float follow_speed = m_followSpeed * Time.deltaTime;
            Transform t = GetComponent<Transform>();
            objectPosGoal = m_playerTransform.localPosition + Quaternion.AngleAxis(m_playerTransform.localEulerAngles.y, Vector3.up) * (Vector3.Scale(controllerPos,m_offsetScale) + m_offsetPosition);
            objectRotGoal = m_playerTransform.localRotation * OVRInput.GetLocalControllerRotation(m_controller);
            //t.localPosition = Vector3.Lerp(t.localPosition, objectPosGoal, follow_speed);
            //t.localRotation = Quaternion.Slerp(t.localRotation, objectRotGoal, follow_speed);
            currPosition = objectPosGoal;
            currRotation = objectRotGoal;
        }
        else if(m_origin)
        {
            float follow_speed = m_followSpeed * Time.deltaTime;
            Transform t = GetComponent<Transform>();
            objectPosGoal = m_playerTransform.localPosition + Quaternion.AngleAxis(m_playerTransform.localEulerAngles.y, Vector3.up) * m_origin.localPosition;
            objectRotGoal = m_playerTransform.localRotation * m_origin.localRotation;
            //t.localPosition = Vector3.Lerp(t.localPosition, objectPosGoal, follow_speed);
            //t.localRotation = Quaternion.Slerp(t.localRotation, objectRotGoal, follow_speed);
            currPosition = objectPosGoal;
            currRotation = objectRotGoal;
        }
        //if (m_enabled)
        //{
        //    float follow_speed = m_followSpeed * Time.deltaTime;
        //    Transform t = GetComponent<Transform>();
        //    //Rigidbody rb = GetComponent<Rigidbody>();
        //    //Vector3 controllerPos = OVRInput.GetLocalControllerPosition(m_controller);
        //    //Vector3 objectPosGoal = new Vector3(controllerPos.x * m_offsetScale.x, controllerPos.y * m_offsetScale.y, controllerPos.z * m_offsetScale.z) + m_offsetPosition;
        //    //Quaternion objectRotGoal = OVRInput.GetLocalControllerRotation(m_controller);
        //    t.localPosition = Vector3.Lerp(t.localPosition, objectPosGoal, follow_speed);
        //    //currPosition = Vector3.Lerp(t.localPosition, objectPosGoal, follow_speed);
        //    //rb.MovePosition(Vector3.Lerp(t.localPosition, objectPosGoal, follow_speed));
        //    t.localRotation = Quaternion.Slerp(t.localRotation, objectRotGoal, follow_speed);
        //    //currRotation = Quaternion.Slerp(t.localRotation, objectRotGoal, follow_speed);
        //    //rb.MoveRotation(Quaternion.Slerp(t.localRotation, objectRotGoal, follow_speed));
        //}
    }

    void FixedUpdate()
    {
        transform.localPosition = Vector3.LerpUnclamped(transform.localPosition, currPosition, 0.05f);
        transform.localRotation = Quaternion.SlerpUnclamped(transform.localRotation, currRotation, 0.05f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (prevPosition != gameObject.transform.localPosition)
        {
            float magnitude = (prevPosition - gameObject.transform.localPosition).sqrMagnitude * 2;
            int shakeStrength = (int)Mathf.Lerp(0.0f, 255.0f, magnitude * 0.02f);
            //print("shakeStrength : " + shakeStrength);
            VibrationManager.SetControllerVibration(m_controller, 8, 4, shakeStrength);
        }
    }
}
