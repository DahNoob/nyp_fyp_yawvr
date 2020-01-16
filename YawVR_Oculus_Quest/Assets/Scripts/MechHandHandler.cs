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
    [SerializeField]
    private Transform m_weaponsTransform;

    //[Header("References")]
    //[SerializeField]
    //private Transform m_playerTransform;

    public ControllerFollower follower { private set; get; }
    public Transform weaponsTransform { get { return m_weaponsTransform; } private set { m_weaponsTransform = value; } }

    void Start()
    {
        follower = m_controller == OVRInput.Controller.RTouch ? PlayerHandler.instance.GetRightFollower() : PlayerHandler.instance.GetLeftFollower();
    }

    public float CalculateFollowerSpeed()
    {
        return follower.CalculateFollowerSpeed();
    }

    void Update()
    {
        if(m_enabled)
            m_handPivot.rotation = follower.transform.rotation;
    }

    void FixedUpdate()
    {
        if(m_enabled)
            m_handModel.SetFloat("Blend", Mathf.LerpUnclamped(m_handModel.GetFloat("Blend"), OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, m_controller), 0.13f));
    }

    public void Bump(Vector3 _posOffset, Vector3 _rotOffset = new Vector3())
    {
        follower.Bump(_posOffset, _rotOffset);
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

    public void SetPose(string _poseName, bool _value)
    {
        m_handModel.SetBool(_poseName, _value);
    }
}
