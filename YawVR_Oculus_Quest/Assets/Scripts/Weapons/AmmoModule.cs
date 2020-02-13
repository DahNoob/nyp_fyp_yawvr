using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class that can be added to any gun to have ammo capabilities.
/// </summary>
[System.Serializable]
public class AmmoModule
{
    public delegate void StartReload();
    public delegate void FinishReload();

    /// <summary>
    /// Called when a reload is called.
    /// </summary>
    public event StartReload onStartReload;
    /// <summary>
    /// called when a reload has finished.
    /// </summary>
    public event FinishReload onFinishReload;

    [Header("Ammo Setup")]
    [SerializeField]
    private AmmoModuleInfo m_ammoInfo;
    //[SerializeField]
    [Tooltip("The current ammo in the clip")]
    private int m_currentAmmo;
    //[SerializeField]
    //[Tooltip("How much ammo is in each clip?")]
    //private int m_maxAmmo = 1;
    //[SerializeField]
    [Tooltip("How long does it take to reload?")]
    private float m_reloadTime = 0.5f;
    //Not sure if this is necessary
    //[SerializeField]
    //[Tooltip("Max amount of clips before the gun can't reload no more")]
    //private int m_maxClips;
    //[SerializeField]
    [Tooltip("Current amount of clips that is in the range 0 - maxClips")]
    private int m_currentClips;
    //Reload timer that determines whether the reload has finished
    private float m_reloadTick;
    //Is this gun already reloading
    [HideInInspector]
    public bool m_isReloading = false;

    [Header("Behaviour changes")]
    //[SerializeField]
    private bool usesClips = true;
   // [SerializeField]
    private bool usesAmmo = true;

    /// <summary>
    /// Gets the reloadTime for this module.
    /// </summary>
    public float reloadTime
    {
        get { return m_reloadTime; }
        private set { m_reloadTime = value; }
    }

    /// <summary>
    /// Gets current ammo for this module.
    /// </summary>
    public int currentAmmo
    {
        get { return m_currentAmmo; }
        private set { m_currentAmmo = Mathf.Clamp(value, 0, m_ammoInfo.maxAmmo); }
    }

    /// <summary>
    /// Gets the max ammo of this module.
    /// </summary>
    public int maxAmmo
    {
        get { return m_ammoInfo.maxAmmo; }
        //private set { m_maxAmmo = value; }
    }

    /// <summary>
    /// Gets max clips of this module.
    /// </summary>
    public int maxClips
    {
        get { return m_ammoInfo.maxClip; }
        //private set { m_maxClips = value; }
    }

    /// <summary>
    /// Gets the current clips
    /// </summary>
    public int currentClips
    {
        get { return m_currentClips; }
        private set { m_currentClips = Mathf.Clamp(value, 0, m_ammoInfo.maxClip); }
    }

    /// <summary>
    /// Inits values, for now can just not call if you want?
    /// </summary>
    public void Init()
    {
        //Sets current ammo to max ammo?
        currentAmmo = m_ammoInfo.maxAmmo;
        currentClips = m_ammoInfo.maxClip;
        m_reloadTick = 0;
        m_reloadTime = m_ammoInfo.reloadTime;
        usesAmmo = m_ammoInfo.usesAmmo;
        usesClips = m_ammoInfo.usesClips;
    }

    /// <summary>
    ///  A function to decrease ammo or for shooting
    /// </summary>
    /// <param name="ammoDeduction">Amount of ammo deducted for this function.</param>
    /// <returns>True if ammo was successfully deducted, false if not.</returns>
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

 /// <summary>
 /// Function to decrease amount of clips
 /// </summary>
 /// <param name="clipDeduction">Amount of clips to deduct by</param>
 /// <returns>True if clips were deducted, false if not.</returns>
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


    /// <summary>
    /// Coroutine for reloading this ammoModule
    /// </summary>
    /// <returns></returns>
    public IEnumerator Reload()
    {
        if (DecreaseClips(1))
        {
            if (!m_isReloading)
            {
                m_isReloading = true;
                onStartReload?.Invoke();
                while (true)
                {
                    yield return new WaitForSeconds(m_reloadTime);
                    m_isReloading = false;
                    m_currentAmmo = m_ammoInfo.maxAmmo;
                    onFinishReload?.Invoke();
                    Persistent.instance.SOUND_RELOAD_FADEIN.PlaySound();
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Changes behaviour on whether this ammo module uses clips or ammo.
    /// </summary>
    /// <param name="usesClips">Does this module want to use clips?</param>
    /// <param name="usesAmmo">Does this module want to use ammo?</param>
    public void ChangeBehaviour(bool usesClips, bool usesAmmo)
    {
        this.usesClips = usesClips;
        this.usesAmmo = usesAmmo;
    }

    /// <summary>
    /// Returns a normalized value between 0 and 1
    /// </summary>
    /// <returns>A normalized value between 0 and 1 (currentAmmo/maxAmmo)</returns>
    public float ReturnNormalized()
    {
        return (float)m_currentAmmo / (float)m_ammoInfo.maxAmmo;
    }

    /// <summary>
    /// Does this ammoModule need to reload?
    /// </summary>
    /// <returns>Returns true if this ammoModule needs to reload</returns>
    public bool NeedReload()
    {
        //if already reloading then just dont do the reload again
        if (m_isReloading)
            return false;

        if (!usesAmmo)
            return false;

        //Else if bullet less or equals 0 
        if (currentAmmo <= 0)
            return true;

        return false;
    }

}
