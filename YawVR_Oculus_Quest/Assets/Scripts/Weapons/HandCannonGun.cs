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

    override protected void Vibe()
    {
        VibrationManager.SetControllerVibration(m_controller, 0.1f, 0.7f);
    }

    override protected void SpawnProjectile()
    {
        base.SpawnProjectile();
        m_handCannonAnimator.Play("HandCannonGun_Shoot");
    }
}
