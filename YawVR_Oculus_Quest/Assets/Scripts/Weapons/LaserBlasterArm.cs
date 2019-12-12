using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/******************************  
** Name: Laser Blaster Module
** Desc: Mech's Laser Blaster Module (like stormtrooper's pew pew guns)
** Author: DahNoob
** Date: 09/12/2019, 11:59AM
**************************
** Change History
**************************
** PR   Date                    Author    Description 
** --   --------                -------   ------------------------------------
** 1    09/12/2019, 11:59AM     DahNoob   Created
** 2    12/12/2019, asd
*******************************/
public class LaserBlasterArm : MechArmModule
{
    [Header("Laser Blaster Configuration")]
    [SerializeField]
    protected GameObject m_projectilePrefab;
    [SerializeField]
    protected Transform m_projectileOrigin;
    [SerializeField]
    protected float m_shootInterval = 1.0f;
    [SerializeField]
    protected int m_projectileAmount;
    [SerializeField]
    protected ParticleSystem m_shootParticle;
    [SerializeField]
    protected AudioSource m_shootAudioSource;
    [SerializeField]
    protected AudioClip[] m_shootAudioClips;

    //Local variables
    private float shootTick;

    void Start()
    {
        if (!CustomUtility.IsObjectPrefab(m_projectilePrefab))
            throw new System.Exception("Error! Member <m_projectilePrefab> is not a prefab!");
    }

    public override bool Activate(OVRInput.Controller _controller)
    {
        shootTick = 0;
        follower.m_followSpeed = m_followerSpeed;
        return true;
    }

    public override bool Hold(OVRInput.Controller _controller)
    {
        shootTick -= Time.deltaTime;
        if (shootTick < 0)
        {
            shootTick += m_shootInterval;
            if (PlayerHandler.instance.DecreaseEnergy(m_energyReduction))
            {
                for (int i = 0; i < m_projectileAmount; ++i)
                {
                    BaseProjectile derp = Instantiate(m_projectilePrefab, m_projectileOrigin.position, m_projectileOrigin.rotation * Quaternion.Euler(Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f)), Persistent.instance.GO_DYNAMIC.transform).GetComponent<BaseProjectile>();
                    //derp.transform.SetPositionAndRotation(m_projectileOrigin.position, m_projectileOrigin.rotation );
                    //derp.transform.Rotate(Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f));
                    derp.Init();
                }
                follower.Bump(new Vector3(0, 0.02f, 0.05f), new Vector3(-6, 0, 0));
                //VibrationManager.SetControllerVibration(m_controller, vibeClip);
                VibrationManager.SetControllerVibration(m_controller, 0.08f, 0.8f);
                m_shootParticle.Emit(8);
                m_shootAudioSource.clip = m_shootAudioClips[Random.Range(0, m_shootAudioClips.Length - 1)];
                m_shootAudioSource.Play();
                return true;
            }
        }
        return false;
    }

    public override bool Stop(OVRInput.Controller _controller)
    {
        return true;
    }
}
