using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LaserGunWeapon : MechGunWeapon
{
    [Header("Laser Gun Configuration")]
    [SerializeField]
    protected Animator m_gatlingAnimator;

    public override bool Grip()
    {
        m_laserPointer.gameObject.SetActive(true);
        return true;
    }

    public override bool Activate(OVRInput.Controller _controller)
    {
        shootTick = 0;
        follower.m_followSpeed = m_followerSpeed;
        m_gatlingAnimator.SetFloat("Blend", 1);
        foreach (var asd in m_muzzleFlash.GetComponentsInChildren<ParticleSystem>())
        {
            asd.Play();
        }
        //m_laserPointer.gameObject.SetActive(true);
        return true;
    }

    public override bool Stop(OVRInput.Controller _controller)
    {
        m_gatlingAnimator.SetFloat("Blend", 0);
        foreach (var asd in m_muzzleFlash.GetComponentsInChildren<ParticleSystem>())
        {
            asd.Stop();
        }
        // m_laserPointer.gameObject.SetActive(false);
        return true;
    }

    public override bool Ungrip()
    {
        m_laserPointer.gameObject.SetActive(false);
        return true;
    }

    private void Update()
    {
        ////If it's reloading, then don't show?
        //weaponAmmoText.enabled = !ammoModule.m_isReloading;
    }
}
