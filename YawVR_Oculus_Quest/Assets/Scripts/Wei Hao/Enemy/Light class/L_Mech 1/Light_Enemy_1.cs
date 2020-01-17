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
public class Light_Enemy_1 : EnemyBase, IPooledObject
{
    public enum _EnemyState
    {
        CHASE,
        SHOOT,
        AVOID,
        DIE,
    }

    private enum _Buffs
    {
        HP,
        DMG,
        MS
    }

    List<string> buffs;

    public Transform projectile;
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

    // Particle effect when baneling explodes
    public ParticleSystem poof;
    float explodeDuration = 1.0f;

    [SerializeField]
    private _EnemyState currentState;

    [Header("Rarity")]
    [SerializeField]
    private _Rarity rarity;
    private WeightedRandom weightedRandom;

    private float projectileSpeed;
    private float amount = 1.0f; //how much it shakes
    private Vector3 transformX;

    //[Header("Projectile Origin")]
    //// Light Mech Shooting
    //public Transform m_projectileOriginL;
    //public Transform m_projectileOriginR;

    // Dodge Check
    private float dodgeCheck = 2.0f;

    [Header("Death Particle Effect")]
    [SerializeField]
    public GameObject explosionPrefab;
    private GameObject explosion;

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

    public void OnObjectDestroy()
    {
        //Set bool i suppose if it actually dead
        m_Animator.SetBool("ResetAnim", true);
        RemoveFromQuadTree(this.gameObject);
        this.gameObject.SetActive(false);
        ObjectPooler.instance.SpawnFromPool(PoolObject.OBJECTTYPES.ENEMY_DEATH_EFFECT, m_bodyTransform.position, Quaternion.identity);
    }

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        // Current State
        //currentState = _EnemyState.AVOID;
        Player = PlayerHandler.instance.gameObject;
        target = Player.transform;
        rb = GetComponent<Rigidbody>();
        m_Animator = GetComponentInChildren<Animator>();
        poof = GetComponent<ParticleSystem>();

        // Get rarity
        rarity = (_Rarity)WeightedRandom.instance.random();
        buffs = new List<string> { "HP", "DMG", "MS" };

        transformX = transform.position;

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
    void Update()
    {
        //Vector3 relativePos = target.position - transform.position;

        //// Debugging distance
        //float distance = Vector3.Distance(transform.position, target.position);
        //if (Vector3.Distance(transform.position, target.position) >= minimumRange)
        //{
        //    currentState = _EnemyState.CHASE;
        //    //m_Animator.SetBool("Chase", true);

        //    Debug.Log("Current State: " + currentState);
        //    //Debug.Log("Distance: " + distance);
        //    if (Vector3.Distance(transform.position, target.position) <= maximumRange)
        //    {
        //        Debug.Log("Within Range");
        //        currentState = _EnemyState.SHOOT;
        //        //transformX = transform.position;
        //        //m_Animator.SetBool("Explode", true);
        //    }
        //}

        //dodgeCheck -= 1.0f * Time.deltaTime;
        //if (dodgeCheck <= 0.0f)
        //{
        //    Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, 5.0f);
        //    for (int i = 0; i < hitColliders.Length; i++)
        //    {
        //        if (hitColliders[i].gameObject.tag == "Bullet")
        //        {
        //            Debug.Log("Projectile detected");
        //            currentState = _EnemyState.AVOID;
        //        }
        //    }
        //}

        //switch (currentState)
        //{
        //case _EnemyState.CHASE:
        //    Quaternion toRotation = Quaternion.LookRotation(new Vector3(relativePos.x, 0, relativePos.z));
        //    transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);

        //    transform.position += transform.forward * moveSpeed * Time.deltaTime;

        //    attackWindUp -= 1.0f * Time.deltaTime;
        //    if (attackWindUp <= 0.0f)
        //    {
        //        //StartCoroutine(EnemyShoot());
        //        attackWindUp = 2.0f;
        //    }

        //    break;
        //case _EnemyState.SHOOT:
        //    toRotation = Quaternion.LookRotation(new Vector3(relativePos.x, 0, relativePos.z));
        //    transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);

        //    attackWindUp -= 1.0f * Time.deltaTime;
        //    if (attackWindUp <= 0.0f)
        //    {
        //        StartCoroutine(EnemyShoot());
        //        attackWindUp = 2.0f;
        //    }

        //    //transformX.x = Mathf.Sin(Time.time * speed) * amount;

        //    break;
        //case _EnemyState.AVOID:

        //    StartCoroutine(EnemyDodge());               
        //    //currentState = _EnemyState.CHASE;
        //    //transform.position += transform.right * -moveSpeed * Time.deltaTime;

        //    break;
        //case _EnemyState.DIE:
        //    break;
        //}

        //Update the bounds position to the transform.position
        queryBounds.position = transform.position;
    }

    //void PlayDeathParticle()
    //{
    //    explosion = Instantiate(explosionPrefab, transform.position, transform.rotation);
    //}

    public string StartBuff()
    {
        System.Random random = new System.Random();
        int index = random.Next(buffs.Count);
        var selectedBuff = buffs[index];
        buffs.RemoveAt(index);
        return selectedBuff;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Mech")
        {
            //Debug.Log("Hit");
            //transformX = transform.position;
            //currentState = _EnemyState.DIE;
            //m_Animator.SetBool("Explode", true);
        }
    }

    //public float GetMoveSpeed()
    //{
    //    return moveSpeed;
    //}

    public float GetRotationSpeed()
    {
        return rotationSpeed;
    }

}
