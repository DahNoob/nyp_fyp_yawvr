using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class MechGunWeapon : MechBaseWeapon
{
    [Header("Base Gun Configuration")]
    [SerializeField]
    protected GameObject m_projectilePrefab;
    [SerializeField]
    protected Transform m_projectileOrigin;
    [SerializeField]
    protected float m_shootInterval = 0.1f;
    [Range(0.0f, 5.0f)]
    [SerializeField]
    protected float m_shootDeviation = 1.0f;
    [SerializeField]
    protected GameObject m_muzzleFlash;
    [SerializeField]
    protected AudioSource m_shootAudioSource;
    [SerializeField]
    protected AudioClip[] m_shootAudioClips;
    [SerializeField]
    protected MechLaserPointer m_laserPointer;
    [SerializeField]
    protected Vector3 m_recoilPosition = new Vector3(0, 0.02f, 0.05f);
    [SerializeField]
    protected Vector3 m_recoilRotation = new Vector3(-2, 0, 0);

    [Header("Ammo Configuration")]
    [SerializeField]
    protected AmmoModule ammoModule;

    //Local variables
    protected float shootTick;

    virtual protected void Start()
    {
        if (!CustomUtility.IsObjectPrefab(m_projectilePrefab))
            throw new System.Exception("Error! Member <m_projectilePrefab> is not a prefab!");
        //Set the fill amount to be the normalized value of the ammo left
        ammoModule.Init();
        ammoModule.onFinishReload += _AmmoModule_onFinishReload;
        shootTick = m_shootInterval;

        GUIManager.instance.SetWeaponInfo(m_controller, m_moduleIcon, m_moduleName, ammoModule.currentAmmo, ammoModule.maxAmmo);

        //Update ammo in GUI first
        //GUIManager.instance.SetWeaponInfoAmmo(m_controller, ammoModule.currentAmmo, ammoModule.maxAmmo);
    }


    private void _AmmoModule_onFinishReload()
    {
        forceFade = false;
        if (isSelected)
            FadeIn();

        //Update the UI again with the new ammo
        GUIManager.instance.SetWeaponInfoAmmo(m_controller, ammoModule.currentAmmo, ammoModule.maxAmmo);
    }

    override public bool Selected()
    {
        base.Selected();
        m_laserPointer.gameObject.SetActive(true);
        mechHand.SetPose("HoldGun", true);
        return true;
    }

    override public bool Grip()
    {
        m_laserPointer.gameObject.SetActive(true);
        return true;
    }

    //Update UI
    public override bool UpdateUI()
    {
        GUIManager.instance.SetWeaponInfo(m_controller, m_moduleIcon, m_moduleName, ammoModule.currentAmmo, ammoModule.maxAmmo);
        return true;
    }

    public override bool Hold(OVRInput.Controller _controller)
    {
        //If after shot and needs to reload
        if (ammoModule.NeedReload())
        {
            //Call reloading function...
            Reload();
            GUIManager.instance.SetWeaponInfoReloading(m_controller);
        }
        //If the ammo module is not reloading?
        if (!ammoModule.m_isReloading && isFullyVisible)
        {
            if (shootTick > m_shootInterval)
            {
                shootTick = 0;

                //If can decrease ammo, then shoot
                if (ammoModule.DecreaseAmmo(1))
                {
                    SpawnProjectile();
                    Vibe();
                    follower.Bump(m_recoilPosition, m_recoilRotation);
                    //VibrationManager.SetControllerVibration(m_controller, vibeClip);
                    m_shootAudioSource.clip = m_shootAudioClips[Random.Range(0, m_shootAudioClips.Length - 1)];
                    m_shootAudioSource.Play();
                    //Triggered
                    GUIManager.instance.Triggered(_controller);
                    //Set weapon info ammo
                    GUIManager.instance.SetWeaponInfoAmmo(m_controller, ammoModule.currentAmmo, ammoModule.maxAmmo);
                    return true;
                }
            }
        }

        return false;

    }

    override public bool Ungrip()
    {
        m_laserPointer.gameObject.SetActive(false);
        return true;
    }

    override public bool Unselected()
    {
        base.Unselected();
        m_laserPointer.gameObject.SetActive(false);
        mechHand.SetPose("HoldGun", false);
        return true;
    }

    virtual protected void SpawnProjectile()
    {
        //BaseProjectile derp = Instantiate(m_projectilePrefab, m_projectileOrigin.position, m_projectileOrigin.rotation, Persistent.instance.GO_DYNAMIC.transform).GetComponent<BaseProjectile>();
        BaseProjectile derp = ObjectPooler.instance.SpawnFromPool(PoolObject.OBJECTTYPES.PLAYER_PROJECTILE, m_projectileOrigin.position, m_projectileOrigin.rotation * Quaternion.Euler(Random.Range(-m_shootDeviation, m_shootDeviation), Random.Range(-m_shootDeviation, m_shootDeviation), 0)).GetComponent<BaseProjectile>();
        derp.Init();
    }

    //vibecheck dawg
    virtual protected void Vibe()
    {
        VibrationManager.SetControllerVibration(m_controller, 0.01f, 0.4f);
    }

    virtual protected void Reload()
    {
        if (ammoModule.m_isReloading)
            return;
        forceFade = true;
        FadeOut();
        StartCoroutine(ammoModule.Reload());
    }

    void OnDestroy()
    {
        ammoModule.onFinishReload -= _AmmoModule_onFinishReload;
    }

    void Update()
    {
        shootTick += Time.deltaTime;
        //If after shot and needs to reload
        if (ammoModule.NeedReload())
        {
            //Call reloading function...
            Reload();
            GUIManager.instance.SetWeaponInfoReloading(m_controller);
        }

    }
}
