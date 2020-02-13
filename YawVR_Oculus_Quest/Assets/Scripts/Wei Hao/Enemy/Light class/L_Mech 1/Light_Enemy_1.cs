using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/******************************  
** Name: Light Enemy 1
** Desc: Light Enemy 1 behaviours and stats
** Author: Wei Hao
** Date: 10/12/2019, 1:06 AM
**************************
** Change History
**************************
** PR   Date                    Author    Description 
** --   --------                -------   ------------------------------------
** 1    10/12/2019, 1:06 PM     Wei Hao   Created & Implemented
** 2    18/12/2019, 1:50 PM     Wei Hao   Added rarity for enemy
** 3    2/1/2020, 4:31 PM       Wei Hao   Added passive buffs depending on rarity
*******************************/

/// <summary>
/// This class provides all functionalities and variables that is used by LightMech1
/// </summary>
public class Light_Enemy_1 : EnemyBase, IPooledObject
{
    [Header("Light Mech 1 Resources")]
    [SerializeField]
    private ParticleSystem m_alertParticleSystem;
    [SerializeField]
    private GameObject m_fireParticleSystem;
    [SerializeField]
    private Transform m_rightProjectileOrigin;
    [SerializeField]
    private Transform m_leftProjectileOrigin;
    [SerializeField]
    private OVR.SoundFXRef m_shootSound;
    [SerializeField]
    private OVR.SoundFXRef m_deathSound;

    //public enum _EnemyState
    //{
    //    CHASE,
    //    SHOOT,
    //    AVOID,
    //    DIE,
    //}

    private enum _Buffs
    {
        HP,
        DMG,
        MS
    }

    List<string> buffs;

    //public Transform projectile;
    private Transform target;
    private GameObject Player;

    //public GameObject leftBlaster;
    //public GameObject rightBlaster;

    // Attack range
    private int attackRange = 10;
    // Baneling rotation speed
    private int rotationSpeed = 5;
    // Time taken before attack is activated
    private float attackWindUp = 2.0f;

    private float maximumRange = 15.0f;
    private float minimumRange = 5.0f;

    //Fetch the Animator
    Animator m_Animator;

    private Rigidbody rb;

    float explodeDuration = 1.0f;

    //[SerializeField]
    // _EnemyState currentState;

    [Header("Rarity")]
    [SerializeField]
    private _Rarity rarity;
    private WeightedRandom weightedRandom;

    private float projectileSpeed;
    private bool inDyingAnim;
    private ParticleSystem[] deathParticles;

    [Header("Buff Prefabs")]
    [SerializeField]
    public GameObject HP_Buff_Prefab;
    public GameObject DMG_Buff_Prefab;
    public GameObject MS_Buff_Prefab;
    //private _Buffs buffs;

    [Header("Active Buff")]
    [SerializeField]
    private bool HP;
    [SerializeField]
    private bool DMG;
    [SerializeField]
    private bool MS;

    //Getters/setters
    public ParticleSystem alertParticleSystem
    {
        get { return m_alertParticleSystem; }
        private set { alertParticleSystem = value; }
    }
    public Transform rightProjectileOrigin
    {
        get { return m_rightProjectileOrigin; }
        private set { m_rightProjectileOrigin = value; }
    }
    public Transform leftProjectileOrigin
    {
        get { return m_leftProjectileOrigin; }
        private set { m_leftProjectileOrigin = value; }
    }

    /// <summary>
    /// Called when this enemy  is spawned from the object pool.
    /// </summary>
    public void OnObjectSpawn()
    {
        //Just gonna cheese it by calling start first
        Start();
        SetIconColor(Color.white);
        SetIconSprite();
        //Reset velocity
        rb.velocity = new Vector3(0f, 0f, 0f);
        rb.angularVelocity = new Vector3(0f, 0f, 0f);
    }


    /// <summary>
    /// Called when this enemy should be "destroyed"
    /// </summary>
    public void OnObjectDestroy()
    {
        //Set bool i suppose if it actually dead
        ObjectPooler.instance.SpawnFromPool(PoolObject.OBJECTTYPES.ENEMY_DEATH_EFFECT, transform.position, Quaternion.identity);
        m_Animator.SetBool("ResetAnim", true);
        RemoveFromQuadTree(this.gameObject);
        this.gameObject.SetActive(false);
    }

    void Awake()
    {
        deathParticles = m_fireParticleSystem.GetComponentsInChildren<ParticleSystem>();
    }

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        Player = PlayerHandler.instance.gameObject;
        target = Player.transform;
        rb = GetComponent<Rigidbody>();
        m_Animator = GetComponentInChildren<Animator>();

        // Get rarity
        rarity = (_Rarity)WeightedRandom.instance.random();
        buffs = new List<string> { "HP", "DMG", "MS" };

        inDyingAnim = false;

        string currBuff = StartBuff();
        if (rarity == _Rarity.DELTA)
        {
            if (currBuff == "HP")
            {
                GameObject hpBuff = Instantiate(m_enemyInfo.hpBuff, transform.position, Quaternion.identity, transform);
                HP = true;
            }
            else if (currBuff == "DMG")
            {
                GameObject dmgBuff = Instantiate(m_enemyInfo.dmgBuff, transform.position, Quaternion.identity, transform);
                DMG = true;
            }
            else if (currBuff == "MS")
            {
                GameObject msBuff = Instantiate(m_enemyInfo.msBuff, transform.position, Quaternion.identity, transform);
                MS = true;
            }
        }
        else if (rarity == _Rarity.BETA)
        {
            string secondBuff = StartBuff();

            if (currBuff == "HP" || secondBuff == "HP")
            {
                GameObject hpBuff = Instantiate(m_enemyInfo.hpBuff, transform.position, Quaternion.identity, transform);
                HP = true;
            }
            if (currBuff == "DMG" || secondBuff == "DMG")
            {
                GameObject dmgBuff = Instantiate(m_enemyInfo.dmgBuff, transform.position, Quaternion.identity, transform);
                DMG = true;
            }
            if (currBuff == "MS" || secondBuff == "MS")
            {
                GameObject msBuff = Instantiate(m_enemyInfo.msBuff, transform.position, Quaternion.identity, transform);
                MS = true;
            }
        }
        else if (rarity == _Rarity.ALPHA)
        {
            string secondBuff = StartBuff();
            string thirdBuff = StartBuff();

            if (currBuff == "HP" || secondBuff == "HP" || thirdBuff == "HP")
            {
                GameObject hpBuff = Instantiate(m_enemyInfo.hpBuff, transform.position, Quaternion.identity, transform);
                HP = true;
            }
            if (currBuff == "DMG" || secondBuff == "DMG" || thirdBuff == "DMG")
            {
                GameObject dmgBuff = Instantiate(m_enemyInfo.dmgBuff, transform.position, Quaternion.identity, transform);
                DMG = true;
            }
            if (currBuff == "MS" || secondBuff == "MS" || thirdBuff == "MS")
            {
                GameObject msBuff = Instantiate(m_enemyInfo.msBuff, transform.position, Quaternion.identity, transform);
                MS = true;
            }
        }
        else if (rarity == _Rarity.OMEGA)
        {
            //string secondBuff = StartBuff();
            //string thirdBuff = StartBuff();
            //string fourthBuff = StartBuff();

            //if (currBuff == "HP" || secondBuff == "HP" || thirdBuff == "HP")
            //{
            //    GameObject hpBuff = Instantiate(HP_Buff_Prefab, transform.position, Quaternion.identity, transform);
            //    HP = true;
            //}
            //if (currBuff == "DMG" || secondBuff == "DMG" || thirdBuff == "DMG")
            //{
            //    GameObject dmgBuff = Instantiate(DMG_Buff_Prefab, transform.position, Quaternion.identity, transform);
            //    DMG = true;
            //}
            //if (currBuff == "MS" || secondBuff == "MS" || thirdBuff == "MS")
            //{
            //    GameObject msBuff = Instantiate(MS_Buff_Prefab, transform.position, Quaternion.identity, transform);
            //    MS = true;
            //}
        }
        //Add this object to quad tree 
        AddToQuadTree(this.gameObject, QuadTreeManager.DYNAMIC_TYPES.ENEMIES);
    }

    // Update is called once per frame
    override protected void Update()
    {
        base.Update();      
    }

    /// <summary>
    /// Called when this enemy takes damage
    /// </summary>
    public override void takeDamage(int damage)
    {
        base.takeDamage(damage);
        m_Animator.SetBool("Alert", true);
    }

    public override void Die()
    {
        if (!inDyingAnim)
        {
            inDyingAnim = true;
            m_Animator.Play("Death");
            for (int i = 0; i < deathParticles.Length; ++i)
            {
                deathParticles[i].Play();
            }
            m_deathSound.PlaySoundAt(transform.position);
        }
    }

    /// <summary>
    /// Called when this enemy hp is 0
    /// </summary>
    public void ActualDie()
    {
        inDyingAnim = false;
        base.Die();
    }

    /// <summary>
    /// Called at Start to give this mech a random buff
    /// </summary>
    public string StartBuff()
    {
        System.Random random = new System.Random();
        int index = random.Next(buffs.Count);
        var selectedBuff = buffs[index];
        buffs.RemoveAt(index);
        return selectedBuff;
    }

    /// <summary>
    /// Called when this enemy is required to shoot at target
    /// </summary>
    public void Shoot_Async()
    {
        StartCoroutine(EnemyShoot());
    }

    public float GetRotationSpeed()
    {
        return rotationSpeed;
    }

    IEnumerator EnemyShoot()
    {
        //Debug.Log("Fire start");
        //BaseProjectile _projectileL = Instantiate(projectile, animator.transform.position + (PlayerTransform.position - animator.transform.position).normalized, Quaternion.LookRotation(PlayerTransform.position - animator.transform.position), Persistent.instance.GO_DYNAMIC.transform).GetComponent<BaseProjectile>();
        //BaseProjectile _projectileL = Instantiate(projectile, m_projectileOriginL.position, Quaternion.identity, Persistent.instance.GO_DYNAMIC.transform).GetComponent<BaseProjectile>();
        BaseProjectile _projectileL = ObjectPooler.instance.SpawnFromPool(PoolObject.OBJECTTYPES.ENEMY_PROJECTILE, m_leftProjectileOrigin.position, Quaternion.identity).GetComponent<BaseProjectile>();
        m_leftProjectileOrigin.LookAt(m_target);
        _projectileL.Init(m_leftProjectileOrigin);
        m_shootSound.PlaySoundAt(m_leftProjectileOrigin.position);

        yield return new WaitForSeconds(0.2f);

        //Debug.Log("2nd Fire start");
        //BaseProjectile _projectileR = Instantiate(projectile, animator.transform.position + (PlayerTransform.position - animator.transform.position).normalized, Quaternion.LookRotation(PlayerTransform.position - animator.transform.position), Persistent.instance.GO_DYNAMIC.transform).GetComponent<BaseProjectile>();
        BaseProjectile _projectileR = ObjectPooler.instance.SpawnFromPool(PoolObject.OBJECTTYPES.ENEMY_PROJECTILE, m_rightProjectileOrigin.position, Quaternion.identity).GetComponent<BaseProjectile>();
        m_rightProjectileOrigin.LookAt(m_target);
        _projectileR.Init(m_rightProjectileOrigin);
        m_shootSound.PlaySoundAt(m_rightProjectileOrigin.position);
    }
}
