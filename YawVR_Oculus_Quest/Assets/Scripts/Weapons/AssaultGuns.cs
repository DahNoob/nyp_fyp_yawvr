using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Assault Gun class that derives from MechGunWeapon.
/// </summary>
public class AssaultGuns : MechGunWeapon
{
    /// <summary>
    /// Called when the trigger button is held down.
    /// </summary>
    /// <param name="_controller"></param>
    /// <returns>True always.</returns>
    public override bool Activate(OVRInput.Controller _controller)
    {
        //Not really calling the base's Activate function since AssaultGun is an auto gun
        follower.m_followSpeed = m_followerSpeed;
        //foreach (var asd in m_muzzleFlash.GetComponentsInChildren<ParticleSystem>())
        //{
        //    asd.Play();
        //}
        return true;
    }

    /// <summary>
    /// Called when the trigger button is released
    /// </summary>
    /// <param name="_controller"></param>
    /// <returns>True always.</returns>
    public override bool Stop(OVRInput.Controller _controller)
    {
        //foreach (var asd in m_muzzleFlash.GetComponentsInChildren<ParticleSystem>())
        //{
        //    asd.Stop();
        //}
        return true;
    }
}
