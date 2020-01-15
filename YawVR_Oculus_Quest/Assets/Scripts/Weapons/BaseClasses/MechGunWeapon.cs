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
    [SerializeField]
    protected GameObject m_muzzleFlash;
    [SerializeField]
    protected AudioSource m_shootAudioSource;
    [SerializeField]
    protected AudioClip[] m_shootAudioClips;
    [SerializeField]
    protected MechLaserPointer m_laserPointer;

    [Header("UI Configuration")]
    //Current weapon ammo
    [SerializeField]
    protected UnityEngine.UI.Text weaponAmmoText;

    [Header("Ammo Configuration")]
    [SerializeField]
    protected AmmoModule ammoModule;

    virtual protected void Start()
    {
        if (!CustomUtility.IsObjectPrefab(m_projectilePrefab))
            throw new System.Exception("Error! Member <m_projectilePrefab> is not a prefab!");
        //Set the fill amount to be the normalized value of the ammo left
        weaponAmmoText.text = ammoModule.currentAmmo.ToString();
    }

    override public bool Selected()
    {
        base.Selected();
        mechHand.SetPose("HoldGun", true);
        return true;
    }

    override public bool Unselected()
    {
        base.Unselected();
        mechHand.SetPose("HoldGun", false);
        return true;
    }
}
