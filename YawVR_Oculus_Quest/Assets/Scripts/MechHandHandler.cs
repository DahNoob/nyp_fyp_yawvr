using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************  
** Name: MechHandHandler monobehaviour
** Desc: Handles things related to the allocated mech hand, to be attached to a MechHand
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
** 4    23/12/2019, 11:07AM     DahNoob   Renamed it from ControllerFollower to MechHandHandler
*******************************/
public class MechHandHandler : MonoBehaviour
{
    [Header("Configuration")]
    public OVRInput.Controller m_controller;
    [SerializeField]
    private bool m_enabled = false;
    [SerializeField]
    private Transform m_handPivot;
    [SerializeField]
    private Animator m_handModel;

    //[Header("References")]
    //[SerializeField]
    //private Transform m_playerTransform;

    public ControllerFollower m_follower { private set; get; }

    void Start()
    {
        m_follower = m_controller == OVRInput.Controller.RTouch ? PlayerHandler.instance.GetRightFollower() : PlayerHandler.instance.GetLeftFollower();
    }

    public float CalculateFollowerSpeed()
    {
        return m_follower.CalculateFollowerSpeed();
    }

    void Update()
    {
        if(m_enabled)
        {
            Quaternion controllerRot = OVRInput.GetLocalControllerRotation(m_controller);
            m_handPivot.localRotation = controllerRot;
            m_handModel.SetFloat("Blend", OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, m_controller));
        }
    }

    public void Bump(Vector3 _posOffset, Vector3 _rotOffset = new Vector3())
    {
        m_follower.Bump(_posOffset, _rotOffset);
    }

    public void SetEnabled(bool _enabled)
    {
        m_enabled = _enabled;
        if(!_enabled)
        {
            m_handPivot.localRotation = Quaternion.identity;
            m_handModel.SetFloat("Blend", 0);
        }
    }
}
