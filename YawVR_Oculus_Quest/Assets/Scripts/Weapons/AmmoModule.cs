using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AmmoModule
{
    [Header("Ammo Setup")]
    [SerializeField]
    [Tooltip("The current ammo in the clip")]
    private int m_currentAmmo;
    [SerializeField]
    [Tooltip("How much ammo is in each clip?")]
    private int m_maxAmmo = 1;
    [SerializeField]
    [Tooltip("How long does it take to reload?")]
    private float m_reloadTime = 0.5f;
    //Not sure if this is necessary
    [SerializeField]
    [Tooltip("Max amount of clips before the gun can't reload no more")]
    private int m_maxClips;
    [SerializeField]
    [Tooltip("Current amount of clips that is in the range 0 - maxClips")]
    private int m_currentClips;
    //Reload timer that determines whether the reload has finished
    private float m_reloadTick;
    //Is this gun already reloading
    [HideInInspector]
    public bool m_isReloading = false;

    [Header("Behaviour changes")]
    [SerializeField]
    private bool usesClips = true;
    [SerializeField]
    private bool usesAmmo = true;

    //Getter for reloadTime
    public float reloadTime
    {
        get { return m_reloadTime; }
        private set { m_reloadTime = value; }
    }

    public int currentAmmo
    {
        get { return m_currentAmmo; }
        private set { m_currentAmmo = Mathf.Clamp(value, 0, m_maxAmmo); }
    }

    public int maxAmmo
    {
        get { return m_maxAmmo; }
        private set { m_maxAmmo = value; }
    }

    public int maxClips
    {
        get { return m_maxClips; }
        private set { m_maxClips = value; }
    }

    public int currentClips
    {
        get { return m_currentClips; }
        private set { m_currentClips = Mathf.Clamp(value, 0, m_maxClips); }
    }

    //Inits values, for now can just not call if you want?
    public void Init()
    {
        //Sets current ammo to max ammo?
        currentAmmo = maxAmmo;
        m_reloadTick = 0;
    }

    //A function to decrease ammo or shoot
    public bool DecreaseAmmo(int ammoDeduction)
    {
        //Just ignore and always return true to the ammo decrease.
        if (!usesAmmo)
            return true;

        if (m_currentAmmo - ammoDeduction >= 0)
        {
            m_currentAmmo -= ammoDeduction;
            return true;
        }
        //Not enough ammo
        return false;
    }

    //Function to decrease clips
    public bool DecreaseClips(int clipDeduction)
    {
        //Just ignore and always return true to the clips decrease.
        if (!usesClips)
            return true;

        if (m_currentClips - clipDeduction >= 0)
        {
            m_currentClips -= clipDeduction;
            return true;
        }

        //Not enough clips
        return false;
    }


    //Reload function
    public IEnumerator Reload()
    {
        if (DecreaseClips(1))
        {
            if (!m_isReloading)
            {
                m_isReloading = true;
                while (true)
                {
                    yield return new WaitForSeconds(m_reloadTime);
                    m_isReloading = false;
                    m_currentAmmo = m_maxAmmo;
                    break;
                }
            }
        }
    }

    public void ChangeBehaviour(bool usesClips, bool usesAmmo)
    {
        this.usesClips = usesClips;
        this.usesAmmo = usesAmmo;
    }

    public float ReturnNormalized()
    {
        return (float)m_currentAmmo / (float)m_maxAmmo;
    }

}
