﻿using System;
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

/// <summary>
/// This class provides the core variables and functions for an enemy.
/// </summary>
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
    [SerializeField]
    protected SpriteRenderer m_minimapIcon;
    [SerializeField]
    protected UnityEngine.AI.NavMeshAgent m_navMeshAgent;
    [SerializeField]
    public AudioSource m_walkSound;

    //Base variables
    protected int flashTick = 0;
    protected MeshRenderer[] meshRenderers;

    //Getters/setters
    /// <summary>
    /// Get the enemy's target 
    /// </summary>
    public Transform m_target
    {
        get
        {
            if (_m_target == null)
                _m_target = PlayerHandler.instance.transform;

            return _m_target;
        }
        set { _m_target = value; }
    }

    /// <summary>
    /// Get the enemie's navMeshAgent
    /// </summary>
    public UnityEngine.AI.NavMeshAgent navMeshAgent
    {
        get { return m_navMeshAgent; }
        private set { m_navMeshAgent = value; }
    }

    /// <summary>
    /// Get the enemy's data.
    /// </summary>
    public EnemyInfo enemyInfo
    {
        get { return m_enemyInfo; }
        private set { m_enemyInfo = value; }
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

    private enum _Buffs
    {
        HP,
        DMG,
        MS
    }

    // Start is called before the first frame update
    virtual protected void Start()
    {
        if (m_target == null)
            m_target = PlayerHandler.instance.transform;
        health = GetMaxHealth();
        GetComponent<UnityEngine.AI.NavMeshAgent>().speed = GetSpeed();
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        SetFlash(false);
    }

    // Update is called once per frame
    virtual protected void Update()
    {
        //Update the bounds position to the transform.position
        queryBounds.position = transform.position;

        if (flashTick > 0)
        {
            --flashTick;
            if (flashTick <= 0)
                SetFlash(false);
        }
    }

    public void SetFlash(bool _flashing)
    {
        //MeshRenderer[] meshes = GetComponentsInChildren<MeshRenderer>();
        if (_flashing)
        {
            for (int i = 0; i < meshRenderers.Length; ++i)
            {
                //meshes[i].material.EnableKeyword("_EMISSION");
                //meshes[i].material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
                //meshes[i].material.SetColor("_EmissionColor", Color.white);
                if (meshRenderers[i])
                    meshRenderers[i].material = Persistent.instance.MAT_WHITE;
            }
        }
        else
        {
            for (int i = 0; i < meshRenderers.Length; ++i)
            {
                //meshes[i].material.DisableKeyword("_EMISSION");
                //meshes[i].material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;
                //meshes[i].material.SetColor("_EmissionColor", Color.black);
                if (meshRenderers[i])
                    meshRenderers[i].material = Persistent.instance.MAT_ENEMYMECH;
            }
        }
    }

    /// <summary>
    /// Enemy's override function for taking damage.
    /// </summary>
    /// <param name="damage"></param>
    public override void takeDamage(int damage)
    {
        if (health > 0)
        {
            if (flashTick <= 0)
                SetFlash(true);
            flashTick = 2;
            health -= damage;
            if (health <= 0)
                Die();
        }
    }

    /// <summary>
    /// Override function for enemy's die
    /// </summary>
    public override void Die()
    {
        //gameObject.SetActive(false);
        PlayerHandler.instance.AddCurrency(m_enemyInfo.currencyValue);
        flashTick = 0;
        SetFlash(false);
        InvokeDie();
        GetComponent<IPooledObject>().OnObjectDestroy();
        PlayerUISoundManager.instance.PlaySoundAt(PlayerUISoundManager.UI_SOUNDTYPE.DEATH_SOUND, transform.position);
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

        /// <summary>
        /// Get the enemy's speed value.
        /// </summary>
        /// <returns>Enemy's speed value in float.</returns>
    public float GetSpeed()
    {
        return m_enemyInfo.moveSpeed * moveSpeedMultiplier;
    }

    /// <summary>
    /// Gets the enemy's damage
    /// </summary>
    /// <returns>Enemy's damage value in int.</returns>
    public int GetDamage()
    {
        return (int)(m_enemyInfo.damage * damageMultiplier);
    }

    /// <summary>
    /// Get's the enemy's max health
    /// </summary>
    /// <returns>Enemy's max health in int.</returns>
    public int GetMaxHealth()
    {
        return (int)(m_enemyInfo.maxHealth * maxHealthMultiplier);
    }

    /// <summary>
    /// Sets the enemy's minimap icon sprite [no longer used]
    /// </summary>
    /// <param name="_newSprite">The sprite to be used as reference.</param>
    public void SetIconSprite(Sprite _newSprite = null)
    {
        m_minimapIcon.sprite = _newSprite ?? m_enemyInfo.defaultIcon;
    }

    /// <summary>
    /// Sets the enemy's icon color [no longer used]
    /// </summary>
    /// <param name="_color">Color for sprite</param>
    public void SetIconColor(Color _color)
    {
        m_minimapIcon.color = _color;
    }

    /// <summary>
    /// Sets the enemy's health
    /// </summary>
    /// <param name="new_HP">New HP value</param>
    public void SetHealth(int new_HP)
    {
        health = new_HP;
    }

    /// <summary>
    /// Sets the enemy's damage multiplier
    /// </summary>
    /// <param name="_newMult">New Multiplier Value</param>
    public void SetDamageMultiplier(float _newMult)
    {
        damageMultiplier = _newMult;
    }

    /// <summary>
    /// Sets the enemy's move speed multiplier
    /// </summary>
    /// <param name="_newMult">New Multiplier Value</param>
    public void SetMoveSpeedMultiplier(float _newMult)
    {
        moveSpeedMultiplier = _newMult;
        GetComponent<UnityEngine.AI.NavMeshAgent>().speed = m_enemyInfo.moveSpeed * moveSpeedMultiplier;
    }

    /// <summary>
    /// Sets the enemy's max health multiplier.
    /// </summary>
    /// <param name="_newMult">New Multiplier Value</param>
    public void SetMaxHealthMultiplier(float _newMult)
    {
        maxHealthMultiplier = _newMult;
    }

    void OnDisable()
    {
        // Instantiate(m_dieEffect, m_bodyTransform.position, Quaternion.identity, Persistent.instance.GO_DYNAMIC.transform);
        //ObjectPooler.instance.SpawnFromPool("EnemyDeathEffect", m_bodyTransform.position, Quaternion.identity);
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos) return;

        Gizmos.color = Color.gray;
        Gizmos.DrawWireCube(queryBounds.position,
            queryBounds.GetWidth() * 2);
    }
}
