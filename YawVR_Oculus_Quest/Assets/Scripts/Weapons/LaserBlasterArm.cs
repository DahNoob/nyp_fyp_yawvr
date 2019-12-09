﻿using System.Collections;
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

    //Local variables
    private float shootTick;
    private OVRHapticsClip vibeClip;//vibe check dawg

    void Start()
    {
        if (!CustomUtility.IsObjectPrefab(m_projectilePrefab))
            throw new System.Exception("Error! Member <m_projectilePrefab> is not a prefab!");
        vibeClip = new OVRHapticsClip();
        for (int i = 0; i < 30; ++i)
        {
            vibeClip.WriteSample(i % 3 == 0 ? (byte)((30 - i) * 120) : (byte)0);
        }
    }

    public override bool Activate(OVRInput.Controller _controller)
    {
        shootTick = 0;
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
                VibrationManager.SetControllerVibration(_controller, vibeClip);
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
