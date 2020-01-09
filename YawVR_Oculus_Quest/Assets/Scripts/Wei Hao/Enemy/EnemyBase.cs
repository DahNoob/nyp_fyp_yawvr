using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************  
** Name: Enemy Stats
** Desc: To manage the stats for all enemies
** Author: Wei Hao
** Date: 6/12/2019, 9:15 AM
**************************
** Change History
**************************
** PR   Date                    Author    Description 
** --   --------                -------   ------------------------------------
** 1    27/11/2019, 5:05 PM     Wei Hao   Created and implemented
** 2    18/12/2019, 10:31 PM    Wei Hao   Added rarity enum
*******************************/

abstract public class EnemyBase : BaseEntity
{
    [Header("Enemy Stats")]
    [SerializeField]
    protected EnemyInfo m_enemyInfo;
    //// Enemy Current Health
    //[SerializeField]
    //protected float health;
    // Enemy Max Health
    [SerializeField]
    protected float maxHealthMultiplier = 1;
    // The amount of Damage the enemy deals
    [SerializeField]
    protected float damageMultiplier = 1;
    // The speed the enemy moves
    [SerializeField]
    protected float moveSpeedMultiplier = 1;
    [SerializeField]
    protected GameObject m_dieEffect;
    [SerializeField]
    protected Transform m_bodyTransform;

    public Transform m_target {
        get {
            if (_m_target == null)
                _m_target = PlayerHandler.instance.transform;

            return _m_target;
        }
        set { _m_target = value; }
    }

    //Hidden variables
    private Transform _m_target;

    [Header("Objects of Interest Area")]
    [SerializeField]
    private bool showGizmos;
    [SerializeField]
    protected QuadRect queryBounds;

    public enum States
    {
        IDLE,
        CHASE,
        WAIT,
        ATTACK
    }

    public enum _Rarity
    {
        NORMAL,
        DELTA,
        BETA,
        OMEGA,
        ALPHA
    }

    // Start is called before the first frame update
    virtual protected void Start()
    {
        if (m_target == null)
            m_target = PlayerHandler.instance.transform;
        health = GetMaxHealth();
        GetComponent<UnityEngine.AI.NavMeshAgent>().speed = GetSpeed();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void takeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
            Die();
    }

    public override void Die()
    {
        //gameObject.SetActive(false);
        InvokeDie();
        Destroy(gameObject);
        //gameObject.GetComponent<ParticleSystem>().Play();
    }

    //public override void takeDamage(int damage)
    //{
    //    health = Mathf.Max(0, health - damage);
    //    if (health == 0)
    //        Die();
    //}

    //public override void Die()
    //{
    //}

    public float GetSpeed()
    {
        return m_enemyInfo.moveSpeed * moveSpeedMultiplier;
    }

    public int GetDamage()
    {
        return (int)(m_enemyInfo.damage * damageMultiplier);
    }

    public int GetHealth ()
    {
        return health;
    }

    public int GetMaxHealth()
    {
        return (int)(m_enemyInfo.maxHealth * maxHealthMultiplier);
    }

    public void SetHealth(int new_HP)
    {
        health = new_HP;
    }

    public void SetDamageMultiplier(float _newMult)
    {
        damageMultiplier = _newMult;
    }

    public void SetMoveSpeedMultiplier(float _newMult)
    {
        moveSpeedMultiplier = _newMult;
        GetComponent<UnityEngine.AI.NavMeshAgent>().speed = m_enemyInfo.moveSpeed * moveSpeedMultiplier;
    }

    public void SetMaxHealthMultiplier(float _newMult)
    {
        maxHealthMultiplier = _newMult;
    }

    void OnDisable()
    {
        Instantiate(m_dieEffect, m_bodyTransform.position, Quaternion.identity, Persistent.instance.GO_DYNAMIC.transform);
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos) return;

        Gizmos.color = Color.gray;
        Gizmos.DrawWireCube(queryBounds.position,
            queryBounds.GetWidth() * 2);
    }
}
