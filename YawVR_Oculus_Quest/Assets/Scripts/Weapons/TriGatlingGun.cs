using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OVR;

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
    //[SerializeField]
    //protected AudioSource m_revUpAudioSource;
    //[SerializeField]
    //protected AudioSource m_loopedShootAudioSource;
    //[SerializeField]
    //protected AudioSource m_revDownAudioSource;

    [SerializeField]
    SoundFXRef m_revUpSound;
    [SerializeField]
    SoundFXRef m_revDownSound;
    [SerializeField]
    SoundFXRef m_loopedShootSound;


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
        m_gatlingAnimator.SetBool("Shooting", false);
        m_barrelWhirlWindEffect.Stop();
        //m_revUpAudioSource.Stop();
        //m_revDownAudioSource.Play();
        //m_loopedShootAudioSource.Stop();
        m_revUpSound.StopSound();
        m_revDownSound.PlaySound();
        m_loopedShootSound.StopSound();
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
        //m_revUpAudioSource.Play();
        //m_revDownAudioSource.Stop();
        m_revUpSound.PlaySound();
        m_revDownSound.StopSound();
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
                    //m_loopedShootAudioSource.Play();
                    m_loopedShootSound.PlaySound();
                    m_loopedShootSound.AttachToParent(Camera.main.transform);
                }
            }
        }
        return true;
    }

    public override bool Stop(OVRInput.Controller _controller)
    {
        if (isWindedUp)
            m_revDownSound.PlaySound();
            //m_revDownAudioSource.Play();
        isActivated = isWindedUp = false;
        m_gatlingAnimator.SetBool("Shooting", false);
        m_barrelWhirlWindEffect.Stop();
        //m_revUpAudioSource.Stop();
        //m_loopedShootAudioSource.Stop();
        m_revUpSound.StopSound();
        m_loopedShootSound.DetachFromParent();
        m_loopedShootSound.StopSound();
        return true;
    }
}
