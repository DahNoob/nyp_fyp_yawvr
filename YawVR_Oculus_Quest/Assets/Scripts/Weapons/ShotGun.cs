using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/******************************  
** Name: Shot Gun
** Desc: Mech's Shot Gun
** Author: DahNoob
** Date: 09/12/2019, 11:59AM
**************************
** Change History
**************************
** PR   Date                    Author    Description 
** --   --------                -------   ------------------------------------
** 1    09/12/2019, 11:59AM     DahNoob   Created
** 2    ????                    DahNoob   forgot lol
** 3    16/01/2020, 10:23AM     DahNoob   Renamed it from LaserBlaster to ShotGun
*******************************/

/// <summary>
/// This class contains properties and values for the Shotgun
/// Inherits from MechGunWeapon
/// </summary>
public class ShotGun : MechGunWeapon
{
    [Header("Shot Gun Configuration")]
    [SerializeField]
    protected Animator m_shotGunAnimator;
    [SerializeField]
    protected ParticleSystem m_whirlWindParticle;
    [SerializeField]
    protected int m_projectileAmount = 4;
    [SerializeField]
    protected float m_cameraShakeAmount = 0.15f;

    public override bool Stop(OVRInput.Controller _controller)
    {
        return true;
    }

    override protected void Vibe()
    {
        VibrationManager.SetControllerVibration(m_controller, 0.1f, 0.7f);
    }

    /// <summary>
    /// Override function that spawns projectiles and plays the shotgun animator.
    /// Emits particles.
    /// </summary>
    override protected void SpawnProjectile()
    {
        for (int i = 0; i < m_projectileAmount; ++i)
        {
            base.SpawnProjectile();
        }
        PlayerHandler.instance.Shake(m_cameraShakeAmount);
        m_shotGunAnimator.Play("ShotGun_Shoot");
        m_whirlWindParticle.Emit(1);
    }
}
