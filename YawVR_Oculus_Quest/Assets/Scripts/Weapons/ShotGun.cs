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
public class ShotGun : MechGunWeapon
{
    [Header("Shot Gun Configuration")]
    [SerializeField]
    protected int m_projectileAmount = 4;

    public override bool Grip()
    {
        return true;
    }

    public override bool Activate(OVRInput.Controller _controller)
    {
        shootTick = 0;
        follower.m_followSpeed = m_followerSpeed;
        return true;
    }

    public override bool Stop(OVRInput.Controller _controller)
    {
        return true;
    }

    public override bool Ungrip()
    {
        return true;
    }

    protected override void SpawnProjectile()
    {
        for (int i = 0; i < m_projectileAmount; ++i)
        {
            BaseProjectile derp = Instantiate(m_projectilePrefab, m_projectileOrigin.position, m_projectileOrigin.rotation * Quaternion.Euler(Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f)), Persistent.instance.GO_DYNAMIC.transform).GetComponent<BaseProjectile>();
            derp.Init();
        }
    }
}
