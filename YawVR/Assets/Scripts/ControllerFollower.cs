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

    //Local variables
    private Vector3 prevPosition, currPosition;
    private Quaternion currRotation;

    void Start()
    {
        prevPosition = gameObject.transform.localPosition;
    }

    void Update()
    {
        if(m_enabled)
        {
            float follow_speed = m_followSpeed * Time.deltaTime;
            Transform t = GetComponent<Transform>();
            //Rigidbody rb = GetComponent<Rigidbody>();
            Vector3 controllerPos = OVRInput.GetLocalControllerPosition(m_controller);
            Vector3 objectPosGoal = new Vector3(controllerPos.x * m_offsetScale.x, controllerPos.y * m_offsetScale.y, controllerPos.z * m_offsetScale.z) + m_offsetPosition;
            Quaternion objectRotGoal = OVRInput.GetLocalControllerRotation(m_controller);
            //t.localPosition = Vector3.Lerp(t.localPosition, objectPosGoal, follow_speed);
            //currPosition = Vector3.Lerp(t.localPosition, objectPosGoal, follow_speed);
            //rb.MovePosition(Vector3.Lerp(t.localPosition, objectPosGoal, follow_speed));
            //t.localRotation = Quaternion.Slerp(t.localRotation, objectRotGoal, follow_speed);
            //currRotation = Quaternion.Slerp(t.localRotation, objectRotGoal, follow_speed);
            //rb.MoveRotation(Quaternion.Slerp(t.localRotation, objectRotGoal, follow_speed));
        }
    }

    void FixedUpdate()
    {
        if(m_enabled)
        {
            Vector3 controllerPos = OVRInput.GetLocalControllerPosition(m_controller);
            Vector3 objectPosGoal = new Vector3(controllerPos.x * m_offsetScale.x, controllerPos.y * m_offsetScale.y, controllerPos.z * m_offsetScale.z) + m_offsetPosition;
            Quaternion objectRotGoal = OVRInput.GetLocalControllerRotation(m_controller);
            Transform t = GetComponent<Transform>();
            GetComponent<Rigidbody>().MovePosition(Vector3.Lerp(t.localPosition, objectPosGoal, m_followSpeed));
            GetComponent<Rigidbody>().MoveRotation(Quaternion.Slerp(t.localRotation, objectRotGoal, m_followSpeed));
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (prevPosition != gameObject.transform.localPosition)
        {
            float magnitude = (prevPosition - gameObject.transform.localPosition).sqrMagnitude * 2;
            int shakeStrength = (int)Mathf.Lerp(0.0f, 255.0f, magnitude * 0.02f);
            print("shakeStrength : " + shakeStrength);
            VibrationManager.SetControllerVibration(m_controller, 8, 4, shakeStrength);
        }
    }
}
