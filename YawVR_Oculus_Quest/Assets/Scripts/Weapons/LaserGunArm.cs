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
    protected ParticleSystem m_shootParticle;
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

    [Header("Ammo Configuration")]
    [SerializeField]
    protected AmmoModule ammoModule;

    [Header("UI Configuration")]
    [SerializeField]
    //Weapon fill circle or something that looks horrible now?
    private Image weaponFill;
    //Current weapon ammo
    [SerializeField]
    private Text weaponAmmoText;
    //Reloading status
    [SerializeField]
    private Text weaponReloading;

    //Local variables
    private float shootTick;

    void Start()
    {
        if (!CustomUtility.IsObjectPrefab(m_projectilePrefab))
            throw new System.Exception("Error! Member <m_projectilePrefab> is not a prefab!");
        //Set the fill amount to be the normalized value of the ammo left
        weaponFill.fillAmount = ammoModule.ReturnNormalized();
        weaponAmmoText.text = ammoModule.currentAmmo.ToString();
        m_laserPointer.gameObject.SetActive(true);
    }

    void OnEnable()
    {
        mechHand.SetPose("HoldGun", true);
    }

    public override bool Activate(OVRInput.Controller _controller)
    {
        shootTick = 0;
        follower.m_followSpeed = m_followerSpeed;
        m_gatlingAnimator.SetFloat("Blend", 1);
        //m_laserPointer.gameObject.SetActive(true);

        //Enable the reticle when holding
        GUIManager.instance.EnableReticle(_controller, true);
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
                    //Set the fill amount to be the normalized value of the ammo left
                    weaponFill.fillAmount = ammoModule.ReturnNormalized();
                    //Set the current ammo count
                    weaponAmmoText.text = ammoModule.currentAmmo.ToString();

                    BaseProjectile derp = Instantiate(m_projectilePrefab, m_projectileOrigin.position, m_projectileOrigin.rotation, Persistent.instance.GO_DYNAMIC.transform).GetComponent<BaseProjectile>();
                    derp.Init(m_projectileOrigin);
                    follower.Bump(m_recoilPosition, m_recoilRotation);
                    //VibrationManager.SetControllerVibration(m_controller, vibeClip);
                    VibrationManager.SetControllerVibration(m_controller, 0.01f, 0.4f);
                    m_shootParticle.Emit(2);
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
        GUIManager.instance.EnableReticle(_controller, false);
        m_gatlingAnimator.SetFloat("Blend", 0);
        // m_laserPointer.gameObject.SetActive(false);
        return true;
    }

    void OnDisable()
    {
        mechHand.SetPose("HoldGun", false);
    }

    private void Update()
    {
        //Some test for now, will finish tommorow
        weaponReloading.enabled = ammoModule.m_isReloading;
        //If it's reloading, then don't show?
        weaponAmmoText.enabled = !ammoModule.m_isReloading;
    }
}
