using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVRTouchSample;

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

    [Header("References")]
    [SerializeField]
    MeshRenderer m_armObject;
    [SerializeField]
    ControllerFollower m_armFollower;

    //Local variables
    private Color origArmInnerColor;
    private Color origArmRimColor;

    private Color currArmInnerColor, currArmRimColor;

    private Color TRANSPARENT_COLOR = new Color(0, 0, 0, 0);

    private void Start()
    {
        origArmInnerColor = currArmInnerColor = m_armObject.material.GetColor("_InnerColor");
        origArmRimColor = currArmRimColor = m_armObject.material.GetColor("_RimColor");
    }
    private void Update()
    {
        bool isGrabbed = GetComponent<OVRGrabbable>().isGrabbed;
        m_armFollower.m_enabled = isGrabbed;
        //m_armObject.gameObject.SetActive(isGrabbed);
        //if(isGrabbed)
        //{
        float deltaTime_xFour = Time.deltaTime * 4.0f;
        currArmInnerColor = Color.Lerp(currArmInnerColor, isGrabbed ? origArmInnerColor : TRANSPARENT_COLOR, deltaTime_xFour);
        currArmRimColor = Color.Lerp(currArmRimColor, isGrabbed ? origArmRimColor : TRANSPARENT_COLOR, deltaTime_xFour);
        m_armObject.material.SetColor("_InnerColor", currArmInnerColor);
        m_armObject.material.SetColor("_RimColor", currArmRimColor);
        //}
    }
}
