using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssaultGuns : MechGunWeapon
{
    public override bool Activate(OVRInput.Controller _controller)
    {
        follower.m_followSpeed = m_followerSpeed;
        foreach (var asd in m_muzzleFlash.GetComponentsInChildren<ParticleSystem>())
        {
            asd.Play();
        }
        return true;
    }

    public override bool Stop(OVRInput.Controller _controller)
    {
        foreach (var asd in m_muzzleFlash.GetComponentsInChildren<ParticleSystem>())
        {
            asd.Stop();
        }
        return true;
    }
}
