using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LaserGunWeapon : MechGunWeapon
{
    [Header("Laser Gun Configuration")]
    [SerializeField]
    protected Animator m_gatlingAnimator;
    [SerializeField]
    protected Vector3 m_recoilPosition = new Vector3(0, 0.02f, 0.05f);
    [SerializeField]
    protected Vector3 m_recoilRotation = new Vector3(-2, 0, 0);

    //Local variables
    private float shootTick;

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

    public override bool Hold(OVRInput.Controller _controller)
    {
        //If the ammo module is not reloading?
        if (ammoModule.m_isReloading == false)
        {
            shootTick += Time.deltaTime;
            if (shootTick >= m_shootInterval)
            {
                shootTick -= m_shootInterval;
                //if (PlayerHandler.instance.DecreaseEnergy(m_energyReduction))
                //{
                if (ammoModule.DecreaseAmmo(1))
                {
                    //Set the current ammo count
                    weaponAmmoText.text = ammoModule.currentAmmo.ToString();

                    BaseProjectile derp = Instantiate(m_projectilePrefab, m_projectileOrigin.position, m_projectileOrigin.rotation, Persistent.instance.GO_DYNAMIC.transform).GetComponent<BaseProjectile>();
                    derp.Init(m_projectileOrigin);
                    follower.Bump(m_recoilPosition, m_recoilRotation);
                    //VibrationManager.SetControllerVibration(m_controller, vibeClip);
                    VibrationManager.SetControllerVibration(m_controller, 0.01f, 0.4f);
                    m_shootAudioSource.clip = m_shootAudioClips[Random.Range(0, m_shootAudioClips.Length - 1)];
                    m_shootAudioSource.Play();

                    //Triggered
                    GUIManager.instance.Triggered(_controller);
                    return true;
                }
                else
                {
                    //Try to reload? I mean it's a test.
                    StartCoroutine(ammoModule.Reload());
                }
            }
        }
        return false;
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
