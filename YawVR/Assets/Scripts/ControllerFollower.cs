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
* 
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

    void Start()
    {
        
    }

    void Update()
    {
        float follow_speed = m_followSpeed * Time.deltaTime;
        Transform t = GetComponent<Transform>();
        Vector3 controllerPos = OVRInput.GetLocalControllerPosition(m_controller);
        Vector3 objectPosGoal = new Vector3(controllerPos.x * m_offsetScale.x, controllerPos.y * m_offsetScale.y, controllerPos.z * m_offsetScale.z) + m_offsetPosition;
        Quaternion objectRotGoal = OVRInput.GetLocalControllerRotation(m_controller);
        t.localPosition = Vector3.Lerp(t.localPosition, objectPosGoal, follow_speed);
        t.localRotation = Quaternion.Slerp(t.localRotation, objectRotGoal, follow_speed);
    }
}
