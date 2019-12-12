using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/******************************  
** Name: Laser Blaster Module
** Desc: Mech's Laser Blaster Module (like stormtrooper's pew pew guns)
** Author: DahNoob
** Date: 09/12/2019, 11:59AM
**************************
** Change History
**************************
** PR   Date                    Author    Description 
** --   --------                -------   ------------------------------------
** 1    09/12/2019, 11:59AM     DahNoob   Created
*******************************/
public class LaserBlasterArm : MechArmModule
{
    [Header("Laser Blaster Configuration")]
    [SerializeField]
    protected GameObject m_projectilePrefab;
    [SerializeField]
    protected Transform m_projectileOrigin;
    [SerializeField]
    protected float m_shootInterval = 0.1f;
    [SerializeField]
    protected ParticleSystem m_shootParticle;

    //Local variables
    private float shootTick;

    void Start()
    {
        if (!CustomUtility.IsObjectPrefab(m_projectilePrefab))
            throw new System.Exception("Error! Member <m_projectilePrefab> is not a prefab!");
    }

    public override bool Activate(OVRInput.Controller _controller)
    {
        shootTick = 0;
        follower.m_followSpeed = m_followerSpeed;
        return true;
    }

    public override bool Hold(OVRInput.Controller _controller)
    {
        shootTick += Time.deltaTime;
        if (shootTick > m_shootInterval)
        {
            shootTick -= m_shootInterval;
            if (PlayerHandler.instance.DecreaseEnergy(m_energyReduction))
            {
                Instantiate(m_projectilePrefab, m_projectileOrigin.position, m_projectileOrigin.rotation, Persistent.instance.GO_DYNAMIC.transform);
                //VibrationManager.SetControllerVibration(m_controller, vibeClip);
                VibrationManager.SetControllerVibration(m_controller, 0.01f, 0.4f);
                m_shootParticle.Emit(6);
                return true;
            }
        }
        return false;
    }

    public override bool Stop(OVRInput.Controller _controller)
    {
        return true;
    }
}
