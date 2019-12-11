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
** 3    11/12/2019, 3:34PM      DahNoob   Did abit of punching particles
*******************************/
public class NormalArmModule : MechArmModule
{
    [Header("Normal Arm Configuration")]
    [SerializeField]
    [Range(0.0f, 1.0f)]
    protected float m_followerMaxSpeed = 0.2f;
    [SerializeField]
    protected float m_punchSpeedEnter = 0.08f;
    [SerializeField]
    protected float m_punchSpeedExit = 0.045f;
    [SerializeField]
    private GameObject m_activateHolos;
    [SerializeField]
    private GameObject m_activateParticles;
    [SerializeField]
    private ParticleSystem m_punchingParticles;

    //Local variables
    private bool isPunching = false;

    void Start()
    {
        m_activateHolos.SetActive(false);
        m_activateParticles.SetActive(false);
        m_punchingParticles.Stop();
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
        float followerCurrentSpeed = follower.CalculateFollowerSpeed();
        if(!isPunching && followerCurrentSpeed > m_punchSpeedEnter)
        {
            isPunching = true;
            //m_punchingParticles.Play();
        }
        else if (isPunching && followerCurrentSpeed < m_punchSpeedExit)
        {
            isPunching = false;
            //m_punchingParticles.Stop();
        }
        return true;
    }

    public override bool Stop(OVRInput.Controller _controller)
    {
        m_activateHolos.SetActive(false);
        m_activateParticles.SetActive(false);
        isPunching = false;
        //m_punchingParticles.Stop();
        follower.m_followSpeed = m_followerSpeed;
        return true;
    }
}
