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

    public override bool Activate(OVRInput.Controller _controller)
    {
        throw new System.NotImplementedException();
    }

    public override bool Hold(OVRInput.Controller _controller)
    {
        throw new System.NotImplementedException();
    }

    public override bool Stop(OVRInput.Controller _controller)
    {
        throw new System.NotImplementedException();
    }
}
