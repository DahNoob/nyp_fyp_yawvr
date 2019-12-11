using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************  
** Name: Normal Arm Module
** Desc: Mech's Normal Arm module
** Author: DahNoob
** Date: 06/12/2019, 2:47 PM
**************************
** Change History
**************************
** PR   Date                    Author    Description 
** --   --------                -------   ------------------------------------
** 1    06/12/2019, 2:47PM      DahNoob   Created
** 2    09/12/2019, 11:58AM     DahNoob   Renamed it to Normal Arm Module
*******************************/
public class NormalArmModule : MechArmModule
{
    [Header("Normal Arm Configuration")]
    [SerializeField]
    [Range(0.0f, 1.0f)]
    protected float m_followerMaxSpeed = 0.2f;
    [SerializeField]
    private GameObject m_activateHolos;
    [SerializeField]
    private GameObject m_activateParticles;

    //Local variables
    private OVRHapticsClip vibeClip = new OVRHapticsClip();//vibe check dawg

    new void Awake()
    {
        base.Awake();
        for (int i = 0; i < 100; ++i)
        {
            vibeClip.WriteSample(i % 3 == 0 ? (byte)(Mathf.Min((100 - i) * 10, 254)) : (byte)0);
        }
    }

    void Start()
    {
        m_activateHolos.SetActive(false);
        m_activateParticles.SetActive(false);
    }

    public override bool Activate(OVRInput.Controller _controller)
    {
        m_activateHolos.SetActive(true);
        m_activateParticles.SetActive(true);
        follower.m_followSpeed = m_followerMaxSpeed;
        //VibrationManager.SetControllerVibration(m_controller, vibeClip);
        VibrationManager.SetControllerVibration(m_controller, 0.1f, 0.65f, false);
        return true;
    }

    public override bool Hold(OVRInput.Controller _controller)
    {
        return true;
    }

    public override bool Stop(OVRInput.Controller _controller)
    {
        m_activateHolos.SetActive(false);
        m_activateParticles.SetActive(false);
        follower.m_followSpeed = m_followerSpeed;
        return true;
    }
}
