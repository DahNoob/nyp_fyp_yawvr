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
    [SerializeField]
    protected AudioSource m_revUpAudioSource;
    [SerializeField]
    protected AudioSource m_loopedShootAudioSource;
    [SerializeField]
    protected AudioSource m_revDownAudioSource;

    protected float windUpElapsed = 0;
    protected bool isActivated = false;
    protected bool isWindedUp = false;

    protected override void Start()
    {
        base.Start();
        onFadedIn += _TriGatlingGun_onFadedIn;
        ammoModule.onStartReload += _AmmoModule_onStartReload_Gatling;
    }

    private void _TriGatlingGun_onFadedIn()
    {
        if (isActivated)
        {
            Activate(m_controller);
        }
    }

    private void _AmmoModule_onStartReload_Gatling()
    {
        VibrationManager.StopHapticPulse(m_controller);
        m_gatlingAnimator.SetBool("Shooting", false);
        m_barrelWhirlWindEffect.Stop();
        m_revUpAudioSource.Stop();
        m_revDownAudioSource.Play();
        m_loopedShootAudioSource.Stop();
        windUpElapsed = shootTick = 0;
        isWindedUp = false;
    }

    private void OnDestroy()
    {
        ammoModule.onStartReload -= _AmmoModule_onStartReload_Gatling;
        onFadedIn -= _TriGatlingGun_onFadedIn;
    }

    public override bool Activate(OVRInput.Controller _controller)
    {
        //Not really calling the base's Activate function since TriGatling is an auto gun
        isActivated = true;
        follower.m_followSpeed = m_followerSpeed;
        windUpElapsed = 0;
        m_gatlingAnimator.SetBool("Shooting", true);
        m_barrelWhirlWindEffect.Play();
        m_revUpAudioSource.Play();
        m_revDownAudioSource.Stop();
        return true;
    }

    public override bool Hold(OVRInput.Controller _controller)
    {
        if(!ammoModule.m_isReloading && isFullyVisible)
        {
            windUpElapsed += Time.deltaTime;
            if (windUpElapsed > m_windUpTime)
            {
                base.Hold(_controller);
                if (!isWindedUp)
                {
                    isWindedUp = true;
                    m_loopedShootAudioSource.Play();
                    VibrationManager.SetControllerVibration(m_controller, 0.1f, 0.7f, true, 0.02f, 0.04f);
                }
            }
        }
        return true;
    }

    public override bool Stop(OVRInput.Controller _controller)
    {
        if(isWindedUp)
            m_revDownAudioSource.Play();
        isActivated = isWindedUp = false;
        m_gatlingAnimator.SetBool("Shooting", false);
        m_barrelWhirlWindEffect.Stop();
        m_revUpAudioSource.Stop();
        m_loopedShootAudioSource.Stop();
        VibrationManager.StopHapticPulse(m_controller);
        return true;
    }

    protected override void Vibe()
    {
        //none becuz im putting an auto vibration ratehr than vibrating every bullet shot
    }
}
