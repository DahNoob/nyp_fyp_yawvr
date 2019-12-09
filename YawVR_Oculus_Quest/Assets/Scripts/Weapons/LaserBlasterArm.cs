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
    protected Transform m_projectileRightOrigin;
    [SerializeField]
    protected Transform m_projectileLeftOrigin;
    [SerializeField]
    protected float m_shootInterval = 0.1f;
    [SerializeField]
    protected OVRInput.Controller m_controller;

    //Local variables
    private float shootTick;
    private OVRHapticsClip vibeClip;//vibe check dawg

    void Start()
    {
        if (!CustomUtility.IsObjectPrefab(m_projectilePrefab))
            throw new System.Exception("Error! Member <m_projectilePrefab> is not a prefab!");
        vibeClip = new OVRHapticsClip();
        for (int i = 0;i<30;++i)
        {
            vibeClip.WriteSample(i % 3 == 0 ? (byte)((30 - i) * 80) : (byte)0);
        }
    }

    public override bool Activate()
    {
        shootTick = 0;
        return true;
    }

    public override bool Hold()
    {
        shootTick += Time.deltaTime;
        if (shootTick > m_shootInterval)
        {
            shootTick -= m_shootInterval;
            Transform origin = m_controller == OVRInput.Controller.RTouch ? m_projectileRightOrigin : m_projectileLeftOrigin;
            Instantiate(m_projectilePrefab, origin.position, origin.rotation, Persistent.instance.GO_DYNAMIC.transform);
            VibrationManager.SetControllerVibration(m_controller, vibeClip);
        }
        return true;
    }

    public override bool Stop()
    {
        return true;
    }
}
