using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************  
** Name: Pilot Controller Behaviour
** Desc: Detect player hands in control holes
** Author: DahNoob
** Date: 27/11/2019, 5:05 PM
**************************
** Change History
**************************
** PR   Date                    Author    Description 
** --   --------                -------   ------------------------------------
** 1    27/11/2019, 5:05 PM     DahNoob   Created and implemented
** 2    02/12/2019, 2:01 PM     DahNoob   Made hologram material change depending on grab status
*******************************/
public class PilotController : MonoBehaviour
{
    [SerializeField]
    //The corresponding controller
    private OVRInput.Controller m_controller;

    [Header("Configuration")]
    [SerializeField]
    private float m_handTriggerBegin = 0.55f;
    [SerializeField]
    private float m_handTriggerEnd = 0.35f;
    [SerializeField]
    private float m_indexTriggerBegin = 0.55f;
    [SerializeField]
    private float m_indexTriggerEnd = 0.35f;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float m_armMaxSpeed = 0.2f;

    [Header("References")]
    [SerializeField]
    MeshRenderer m_armObject;
    [SerializeField]
    ControllerFollower m_armFollower;
    [SerializeField]
    GameObject m_ringObject;
    [SerializeField]
    Transform m_holos;

    [Header("Offsets")]
    [SerializeField]
    Transform m_pivotOffset;
    [SerializeField]
    Transform m_ringOffset;

    //Local variables

    //private OVRGrabber grabber;
    private Color currArmInnerColor, currArmRimColor;
    private bool isAttached, isHandTriggered, isIndexTriggered = false;
    private MeshRenderer currentHoloArm;
    private List<MechArmModule> modules = new List<MechArmModule>();
    private int currModuleIndex = 0;

    private Color ORIG_ARM_INNER_COLOR;
    private Color ORIG_ARM_RIM_COLOR;
    private Color TRANSPARENT_COLOR = new Color(0, 0, 0, 0);
    private float ARM_MINSPEED;
    
    private void Start()
    {
        ORIG_ARM_INNER_COLOR = m_armObject.material.GetColor("_InnerColor");
        ORIG_ARM_RIM_COLOR = m_armObject.material.GetColor("_RimColor");
        currArmInnerColor = currArmRimColor = TRANSPARENT_COLOR;
        m_armObject.material.SetColor("_InnerColor", TRANSPARENT_COLOR);
        m_armObject.material.SetColor("_RimColor", TRANSPARENT_COLOR);
        ARM_MINSPEED = m_armFollower.m_followSpeed;
    }
    private void Update()
    {
        if (!isAttached)
            return;

        if ((isHandTriggered && OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, m_controller) < m_handTriggerEnd) ||
            (!isHandTriggered && OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, m_controller) > m_handTriggerBegin))
        {
            isHandTriggered = !isHandTriggered;
            if (isHandTriggered)
                VibrateCrescendo();
            else
                m_armFollower.m_followSpeed = ARM_MINSPEED;
        }
        if ((isIndexTriggered && OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, m_controller) < m_indexTriggerEnd) ||
            (!isIndexTriggered && OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, m_controller) > m_indexTriggerBegin))
        {
            isIndexTriggered = !isIndexTriggered;
            if (isIndexTriggered && isHandTriggered)
                VibrationManager.SetControllerVibration(m_controller, 8, 2, 100);
        }
        float deltaTime_xTwo = Time.deltaTime * 2.0f;
        float deltaTime_xFour = Time.deltaTime * 4.0f;

        m_armFollower.m_enabled = isHandTriggered;
        currArmInnerColor = Color.Lerp(currArmInnerColor, isHandTriggered ? ORIG_ARM_INNER_COLOR : TRANSPARENT_COLOR, deltaTime_xFour);
        currArmRimColor = Color.Lerp(currArmRimColor, isHandTriggered ? ORIG_ARM_RIM_COLOR : TRANSPARENT_COLOR, deltaTime_xFour);
        m_armFollower.m_followSpeed = Mathf.Lerp(m_armFollower.m_followSpeed, isIndexTriggered ? m_armMaxSpeed : ARM_MINSPEED, deltaTime_xTwo);
        m_armObject.material.SetColor("_InnerColor", currArmInnerColor);
        m_armObject.material.SetColor("_RimColor", currArmRimColor);
    }

    public void AttachArmModule(MechArmModule _armModule)
    {
        modules.Add(_armModule);
        MeshRenderer holoArm = m_controller.Equals(OVRInput.Controller.RTouch) ? _armModule.GetRightHoloObject().GetComponent<MeshRenderer>() :
            _armModule.GetLeftHoloObject().GetComponent<MeshRenderer>();
        holoArm.transform.SetParent(m_holos);
        holoArm.transform.localPosition = Vector3.zero;
        holoArm.transform.localRotation = Quaternion.identity;
    }

    void LateUpdate()
    {
        //if (isAttached)
        //    MoveGrabbedObject(grabber.transform.position, grabber.transform.rotation);
    }

    protected virtual void MoveGrabbedObject(Vector3 pos, Quaternion rot)
    {
        if (!isAttached)
        {
            return;
        }
        
        //Rigidbody grabbedRigidbody = m_grabbedObj.grabbedRigidbody;
        //Vector3 grabbablePosition = pos + rot * m_grabbedObjectPosOff;
        //Quaternion grabbableRotation = rot * m_grabbedObjectRotOff;
        Rigidbody body = GetComponent<Rigidbody>();

        body.MovePosition(pos);
        body.MoveRotation(rot);
    }

    void VibrateCrescendo()
    {
        OVRHapticsClip clip = new OVRHapticsClip();
        for (int i = 0; i < 80; ++i)
        {
            clip.WriteSample(i % 3 == 0 ? (byte)(i * 2.5f) : (byte)0);
        }
        VibrationManager.SetControllerVibration(m_controller, clip);
    }

    void OnTriggerEnter(Collider otherCollider)
    {
        // Get the grab trigger
        //print("ASD");
        if (isAttached)
            return;
        OVRGrabber grabber = otherCollider.GetComponent<OVRGrabber>() ?? otherCollider.GetComponentInParent<OVRGrabber>();
        if (grabber && grabber.GetController() == m_controller)
        {
            isAttached = true;
            //this.grabber = grabber;
            VibrationManager.SetControllerVibration(m_controller, 10, 2, 200);
            transform.Find("AnchorPivot").localPosition = m_pivotOffset.localPosition;
            transform.Find("AnchorPivot").localRotation = m_pivotOffset.localRotation;
            m_ringObject.transform.localPosition = m_ringOffset.localPosition;
            m_ringObject.transform.localRotation = m_ringOffset.localRotation;
            transform.SetParent(grabber.transform);
            //transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
    }

}
