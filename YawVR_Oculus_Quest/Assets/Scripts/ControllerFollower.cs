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
    public bool m_followCameraRotation = false;

    [Header("Configuration")]
    public OVRInput.Controller m_controller;
    public Vector3 m_offsetPosition = Vector3.zero;
    public Vector3 m_offsetScale = Vector3.one;
    [Range(0.0f, 1.0f)]
    public float m_followSpeed = 0.05f;
    public Transform m_origin;

    [Header("References")]
    [SerializeField]
    private Transform m_playerTransform;

    [Header("Debug")]
    [SerializeField]
    private UnityEngine.UI.Text m_shakeStrengthText;
    [SerializeField]
    private UnityEngine.UI.Text m_currArmSpeedText;
    [SerializeField]
    private UnityEngine.UI.Text m_maxArmSpeedText;

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
        //Vector3 objectPosGoal;
        //Quaternion objectRotGoal;
        

        if (m_enabled)
        {
            //float camRot = (m_followCameraRotation ? Camera.main.transform.localEulerAngles.y : 0.0f);
            Transform t = GetComponent<Transform>();           
            goalPosition = Quaternion.AngleAxis(m_playerTransform.localEulerAngles.y, Vector3.up) * (Vector3.Scale(controllerPos,m_offsetScale) + m_offsetPosition);
            goalRotation = OVRInput.GetLocalControllerRotation(m_controller);// * Quaternion.AngleAxis(camRot, Vector3.up);
            //t.localPosition = Vector3.Lerp(t.localPosition, objectPosGoal, follow_speed);
            //t.localRotation = Quaternion.Slerp(t.localRotation, objectRotGoal, follow_speed);
            //currPosition = objectPosGoal;
            //currRotation = objectRotGoal;
        }
        else if(m_origin)
        {
            float camRot = (m_followCameraRotation ? Camera.main.transform.localEulerAngles.y : 0.0f);
            Transform t = GetComponent<Transform>();
            goalPosition = Quaternion.AngleAxis(m_playerTransform.localEulerAngles.y, Vector3.up) * m_origin.localPosition;
            goalRotation = m_origin.localRotation;// * Quaternion.AngleAxis(camRot, Vector3.up);
            //t.localPosition = Vector3.Lerp(t.localPosition, objectPosGoal, follow_speed);
            //t.localRotation = Quaternion.Slerp(t.localRotation, objectRotGoal, follow_speed);
            //currPosition = objectPosGoal;
            //currRotation = objectRotGoal;
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
        prevPosition = currPosition;
        prevRotation = currRotation;

        currPosition = Vector3.LerpUnclamped(currPosition, goalPosition, m_followSpeed);
        currRotation = Quaternion.SlerpUnclamped(currRotation, goalRotation, m_followSpeed);

        if(m_controller == OVRInput.Controller.RTouch)
        {
            float speed = CalculateFollowerSpeed();
            m_currArmSpeedText.text = speed.ToString();
            float derp = float.Parse(m_maxArmSpeedText.text);
            m_maxArmSpeedText.text = speed > derp ? speed.ToString() : m_maxArmSpeedText.text;
        }
        
    }

    void LateUpdate()
    {
        transform.localPosition = m_playerTransform.localPosition + currPosition;//Vector3.LerpUnclamped(transform.localPosition, currPosition, m_followSpeed);
        transform.localRotation = m_playerTransform.localRotation * currRotation;//Quaternion.SlerpUnclamped(transform.localRotation, currRotation, m_followSpeed);
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
            if (m_shakeStrengthText)
                m_shakeStrengthText.text = shakeStrength.ToString();
        }
    }
}
