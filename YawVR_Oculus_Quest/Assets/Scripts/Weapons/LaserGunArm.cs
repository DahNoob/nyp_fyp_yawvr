using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LaserGunArm : MechArmModule
{
    [Header("Laser Gun Configuration")]
    [SerializeField]
    protected GameObject m_projectilePrefab;
    [SerializeField]
    protected Transform m_projectileOrigin;
    [SerializeField]
    protected float m_shootInterval = 0.1f;
    [SerializeField]
    protected GameObject m_muzzleFlash;
    [SerializeField]
    protected AudioSource m_shootAudioSource;
    [SerializeField]
    protected AudioClip[] m_shootAudioClips;
    [SerializeField]
    protected Animator m_gatlingAnimator;
    [SerializeField]
    protected MechLaserPointer m_laserPointer;
    [SerializeField]
    protected Vector3 m_recoilPosition = new Vector3(0, 0.02f, 0.05f);
    [SerializeField]
    protected Vector3 m_recoilRotation = new Vector3(-2, 0, 0);
    [SerializeField]
    protected Transform m_dissolveTransform;

    [Header("Ammo Configuration")]
    [SerializeField]
    protected AmmoModule ammoModule;

    [Header("UI Configuration")]
    //Current weapon ammo
    [SerializeField]
    private Text weaponAmmoText;

    //Local variables
    private float shootTick;
    private bool isFullyVisible = false;
    private Vector3 fadeInPos = new Vector3(0, 0.841f, 0);
    private Vector3 fadeOutPos = new Vector3(0, -1.512f, 0);

    void Start()
    {
        if (!CustomUtility.IsObjectPrefab(m_projectilePrefab))
            throw new System.Exception("Error! Member <m_projectilePrefab> is not a prefab!");
        //Set the fill amount to be the normalized value of the ammo left
        weaponAmmoText.text = ammoModule.currentAmmo.ToString();
        m_laserPointer.gameObject.SetActive(true);
        MeshRenderer[] ms = m_armObject.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < ms.Length; i++)
        {
            ms[i].material.SetFloat("_Amount", -1000.0f);
        }
    }

    void OnEnable()
    {
        mechHand.SetPose("HoldGun", true);
        StartCoroutine(fadeIn());
        
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

    void OnDisable()
    {
        mechHand.SetPose("HoldGun", false);
        MeshRenderer[] ms = m_armObject.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < ms.Length; i++)
        {
            ms[i].material.SetFloat("_Amount", -1000.0f);
        }
        StartCoroutine(fadeOut());
    }

    private void Update()
    {
        ////If it's reloading, then don't show?
        //weaponAmmoText.enabled = !ammoModule.m_isReloading;
    }

    private void FixedUpdate()
    {
        m_dissolveTransform.localPosition = Vector3.LerpUnclamped(m_dissolveTransform.localPosition, new Vector3(0, 0.841f, 0), 0.01f);
        MeshRenderer[] ms = m_armObject.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < ms.Length; i++)
        {
            ms[i].material.SetFloat("_Amount", m_dissolveTransform.position.y);
        }

    }

    IEnumerator fadeIn()
    {
        isFullyVisible = false;
        m_dissolveTransform.localPosition = fadeOutPos;
        while (!isFullyVisible)
        {
            yield return new WaitForFixedUpdate();
            m_dissolveTransform.localPosition = Vector3.LerpUnclamped(m_dissolveTransform.localPosition, fadeInPos, 0.03f);
            MeshRenderer[] ms = m_armObject.GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < ms.Length; i++)
            {
                ms[i].material.SetFloat("_Amount", m_dissolveTransform.position.y);
            }
            if (m_dissolveTransform.localPosition.y == fadeInPos.y)
                isFullyVisible = true;
        }
    }
    IEnumerator fadeOut()
    {
        isFullyVisible = true;
        m_dissolveTransform.localPosition = fadeInPos;
        while (isFullyVisible)
        {
            yield return new WaitForFixedUpdate();
            m_dissolveTransform.localPosition = Vector3.LerpUnclamped(m_dissolveTransform.localPosition, fadeOutPos, 0.03f);
            MeshRenderer[] ms = m_armObject.GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < ms.Length; i++)
            {
                ms[i].material.SetFloat("_Amount", m_dissolveTransform.position.y);
            }
            if (m_dissolveTransform.localPosition.y == fadeOutPos.y)
                isFullyVisible = false;
        }
    }
}
