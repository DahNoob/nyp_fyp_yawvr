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

abstract public class EnemyBase : MonoBehaviour
{
    [Header("Enemy Stats")]
    // Enemy Current Health
    [SerializeField]
    protected float health;
    // Enemy Max Health
    [SerializeField]
    protected float maxHealth;
    // The amount of Damage the enemy deals
    [SerializeField]
    protected float damage;
    // The speed the enemy moves
    [SerializeField]
    protected float moveSpeed;
    [SerializeField]
    protected GameObject m_dieEffect;
    [SerializeField]
    protected Transform m_bodyTransform;

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
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void takeDamage(int damage)
    {
        health = Mathf.Max(0, health - damage);
        if (health == 0)
            Die();
    }

    public void Die()
    {
        gameObject.SetActive(false);
        gameObject.GetComponent<ParticleSystem>().Play();
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
        return moveSpeed;
    }

    public float GetDamage()
    {
        return damage;
    }

    public float GetHealth ()
    {
        return health;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public void SetHealth(float new_HP)
    {
        health = new_HP;
    }

    public void SetDamage(float new_Damage)
    {
        damage = new_Damage;
    }

    public void SetMoveSpeed(float new_MoveSpeed)
    {
        moveSpeed = new_MoveSpeed;
    }

    public void SetMaxHealth(float new_MaxHealth)
    {
        maxHealth = new_MaxHealth;
    }

    void OnDisable()
    {
        Instantiate(m_dieEffect, m_bodyTransform.position, Quaternion.identity, Persistent.instance.GO_DYNAMIC.transform);
    }
}
