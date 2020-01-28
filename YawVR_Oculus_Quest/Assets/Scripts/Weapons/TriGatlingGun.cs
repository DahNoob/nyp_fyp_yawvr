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
    protected bool isActivated = false;

    protected override void Start()
    {
        base.Start();
        ammoModule.onStartReload += _AmmoModule_onStartReload_Gatling;
        ammoModule.onFinishReload += _AmmoModule_onFinishReload_Gatling;
    }

    private void _AmmoModule_onStartReload_Gatling()
    {
        m_gatlingAnimator.SetBool("Shooting", false);
        m_barrelWhirlWindEffect.Stop();
    }

    private void _AmmoModule_onFinishReload_Gatling()
    {
        if(isActivated)
        {
            m_gatlingAnimator.SetBool("Shooting", true);
            m_barrelWhirlWindEffect.Play();
        }
    }

    private void OnDestroy()
    {
        ammoModule.onStartReload -= _AmmoModule_onStartReload_Gatling;
        ammoModule.onFinishReload -= _AmmoModule_onFinishReload_Gatling;
    }

    public override bool Activate(OVRInput.Controller _controller)
    {
        isActivated = true;
        follower.m_followSpeed = m_followerSpeed;
        windUpElapsed = 0;
        m_gatlingAnimator.SetBool("Shooting", true);
        m_barrelWhirlWindEffect.Play();
        return true;
    }

    public override bool Hold(OVRInput.Controller _controller)
    {
        windUpElapsed += Time.deltaTime;
        if(windUpElapsed > m_windUpTime)
        {
            base.Hold(_controller);
        }
        return true;
    }

    public override bool Stop(OVRInput.Controller _controller)
    {
        isActivated = false;
        m_gatlingAnimator.SetBool("Shooting", false);
        m_barrelWhirlWindEffect.Stop();
        return true;
    }
}
