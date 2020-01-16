using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TriGatlingGun : MechGunWeapon
{
    [Header("Tri-Gatling Gun Configuration")]
    [SerializeField]
    protected Animator m_gatlingAnimator;

    public override bool Activate(OVRInput.Controller _controller)
    {
        follower.m_followSpeed = m_followerSpeed;
        m_gatlingAnimator.SetBool("Shooting", true);
        foreach (var asd in m_muzzleFlash.GetComponentsInChildren<ParticleSystem>())
        {
            asd.Play();
        }
        //m_laserPointer.gameObject.SetActive(true);
        return true;
    }

    public override bool Stop(OVRInput.Controller _controller)
    {
        m_gatlingAnimator.SetBool("Shooting", false);
        foreach (var asd in m_muzzleFlash.GetComponentsInChildren<ParticleSystem>())
        {
            asd.Stop();
        }
        // m_laserPointer.gameObject.SetActive(false);
        return true;
    }
}
