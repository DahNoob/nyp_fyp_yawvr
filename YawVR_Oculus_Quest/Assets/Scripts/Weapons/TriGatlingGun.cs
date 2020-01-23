using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TriGatlingGun : MechGunWeapon
{
    [Header("Tri-Gatling Gun Configuration")]
    [SerializeField]
    protected Animator m_gatlingAnimator;
    [SerializeField]
    protected ParticleSystem m_barrelWhirlWindEffect;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    protected float m_windUpTime = 0.35f;

    protected float windUpElapsed = 0;

    //protected override void Start()
    //{
    //    base.Start();
    //    ammoModule.onFinishReload += _AmmoModule_onFinishReload_Gatling;
    //}

    //private void _AmmoModule_onFinishReload_Gatling()
    //{
    //    print("hi" + m_gatlingAnimator.GetBool("Shooting"));
    //    if (m_gatlingAnimator.GetBool("Shooting"))
    //    {
    //        m_barrelWhirlWindEffect.Play();
    //        print("bruhmoment");
    //    }
    //}

    //private void OnDestroy()
    //{
    //    ammoModule.onFinishReload -= _AmmoModule_onFinishReload_Gatling;
    //}

    public override bool Activate(OVRInput.Controller _controller)
    {
        follower.m_followSpeed = m_followerSpeed;
        windUpElapsed = 0;
        m_gatlingAnimator.SetBool("Shooting", true);
        m_barrelWhirlWindEffect.Play();
        //foreach (var asd in m_muzzleFlash.GetComponentsInChildren<ParticleSystem>())
        //{
        //    asd.Play();
        //}
        //m_laserPointer.gameObject.SetActive(true);
        return true;
    }

    public override bool Hold(OVRInput.Controller _controller)
    {
        windUpElapsed += Time.deltaTime;
        if(windUpElapsed > m_windUpTime)
        {
            base.Hold(_controller);
        }
        m_gatlingAnimator.SetBool("Shooting", true);
        return true;
    }

    public override bool Stop(OVRInput.Controller _controller)
    {
        m_gatlingAnimator.SetBool("Shooting", false);
        //foreach (var asd in m_muzzleFlash.GetComponentsInChildren<ParticleSystem>())
        //{
        //    asd.Stop();
        //}
        // m_laserPointer.gameObject.SetActive(false);
        m_barrelWhirlWindEffect.Stop();

        return true;
    }
}
