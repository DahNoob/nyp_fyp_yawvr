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

    //Local variables
    private OVRHapticsClip vibeClip;//vibe check dawg

    void Start()
    {
        vibeClip = new OVRHapticsClip();
        for (int i = 0; i < 100; ++i)
        {
            vibeClip.WriteSample(i % 3 == 0 ? (byte)((100 - i) * 10) : (byte)0);
        }
    }

    public override bool Activate(OVRInput.Controller _controller)
    {
        follower.m_followSpeed = m_followerMaxSpeed;
        return true;
    }

    public override bool Hold(OVRInput.Controller _controller)
    {
        return true;
    }

    public override bool Stop(OVRInput.Controller _controller)
    {
        follower.m_followSpeed = m_followerSpeed;
        return true;
    }
}
