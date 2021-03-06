﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/******************************  
** Name: Hand Cannon Gun
** Desc: Hand Cannon Gun
** Author: DahNoob
** Date: 31/01/2020, 10:00 AM
**************************
** Change History
**************************
** PR   Date                    Author    Description 
** --   --------                -------   ------------------------------------
** 1   31/01/2020, 10:00 AM     DahNoob   Created
*******************************/

/// <summary>
/// This class provides values and functionalities for the HandCannonGun.
/// </summary>
public class HandCannonGun : MechGunWeapon
{
    [Header("Hand Cannon Gun Configuration")]
    [SerializeField]
    protected Animator m_handCannonAnimator;
    [SerializeField]
    protected float m_cameraShakeAmount = 0.07f;

    public override bool Stop(OVRInput.Controller _controller)
    {
        return true;
    }

    /// <summary>
    /// Rumbles the controller.
    /// </summary>
    override protected void Vibe()
    {
        VibrationManager.SetControllerVibration(m_controller, 0.1f, 0.7f);
        PlayerHandler.instance.BuzzYaw(0.1f, 50, (int)m_yawBuzzAmplitudes.z, (int)m_yawBuzzAmplitudes.y, (int)m_yawBuzzAmplitudes.x);

    }

    /// <summary>
    /// Spawns a projectile and plays the hand cannon's animator.
    /// </summary>
    override protected void SpawnProjectile()
    {
        base.SpawnProjectile();
        m_handCannonAnimator.Play("HandCannonGun_Shoot");
    }
}
