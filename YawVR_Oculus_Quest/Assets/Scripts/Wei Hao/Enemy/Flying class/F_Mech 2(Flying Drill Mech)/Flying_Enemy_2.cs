using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************  
** Name: Flying Enemy 2 (Homing drill thingy ?)
** Desc: Flying Enemy 2 class
** Author: Wei Hao
** Date: 10/12/2019, 1:06 AM
**************************
** Change History
**************************
** PR   Date                    Author    Description 
** --   --------                -------   ------------------------------------
** 1    31/12/2019, 10:49 AM     Wei Hao
*******************************/
public class Flying_Enemy_2 : EnemyBase
{
    public enum _EnemyState
    {
        CHASE,
        SHOOT,
        AVOID,
        DIE,
    }

    List<string> buffs = new List<string> { "HP", "DMG", "MS" };

    private Transform target;
    private GameObject Player;

    // Baneling rotation speed
    private int rotationSpeed = 5;
    // Time taken before attack is activated
    private float attackWindUp = 2.0f;

    private float maximumRange = 15.0f;
    private float minimumRange = 5.0f;

    //Fetch the Animator
    Animator m_Animator;

    private Rigidbody rb;

    [SerializeField]
    private _EnemyState currentState;

    [Header("Rarity")]
    [SerializeField]
    private _Rarity rarity;
    private GameObject weightedRandom;

    [Header("Buff Prefabs")]
    [SerializeField]
    public GameObject HP_Buff_Prefab;
    public GameObject DMG_Buff_Prefab;
    public GameObject MS_Buff_Prefab;

    [Header("Active Buff")]
    [SerializeField]
    private bool HP;
    [SerializeField]
    private bool DMG;
    [SerializeField]
    private bool MS;


    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.Find("Player");
        target = Player.GetComponent<Transform>();
        rb = gameObject.GetComponent<Rigidbody>();
        m_Animator = gameObject.GetComponentInChildren<Animator>();
        weightedRandom = GameObject.Find("WeightedRNG");

        // Get rarity
        rarity = (_Rarity)weightedRandom.GetComponent<WeightedRandom>().random();

        string currBuff = StartBuff();
        string secondBuff = StartBuff();
        string thirdBuff = StartBuff();

        switch (rarity)
        {
            case _Rarity.DELTA:
                if (currBuff == "HP")
                {
                    GameObject hpBuff = Instantiate(HP_Buff_Prefab, transform.position, Quaternion.identity, transform);
                    HP = true;
                }
                if (currBuff == "DMG")
                {
                    GameObject dmgBuff = Instantiate(DMG_Buff_Prefab, transform.position, Quaternion.identity, transform);
                    DMG = true;
                }
                if (currBuff == "MS")
                {
                    GameObject msBuff = Instantiate(MS_Buff_Prefab, transform.position, Quaternion.identity, transform);
                    MS = true;
                }
                break;
            case _Rarity.BETA:                
                if (currBuff == "HP" || secondBuff == "HP")
                {
                    GameObject hpBuff = Instantiate(HP_Buff_Prefab, transform.position, Quaternion.identity, transform);
                    HP = true;
                }
                if (currBuff == "DMG" || secondBuff == "DMG")
                {
                    GameObject dmgBuff = Instantiate(DMG_Buff_Prefab, transform.position, Quaternion.identity, transform);
                    DMG = true;
                }
                if (currBuff == "MS" || secondBuff == "MS")
                {
                    GameObject msBuff = Instantiate(MS_Buff_Prefab, transform.position, Quaternion.identity, transform);
                    MS = true;
                }
                break;
            case _Rarity.ALPHA:              
                if (currBuff == "HP" || secondBuff == "HP" || thirdBuff == "HP")
                {
                    GameObject hpBuff = Instantiate(HP_Buff_Prefab, transform.position, Quaternion.identity, transform);
                    HP = true;
                }
                if (currBuff == "DMG" || secondBuff == "DMG" || thirdBuff == "DMG")
                {
                    GameObject dmgBuff = Instantiate(DMG_Buff_Prefab, transform.position, Quaternion.identity, transform);
                    DMG = true;
                }
                if (currBuff == "MS" || secondBuff == "MS" || thirdBuff == "MS")
                {
                    GameObject msBuff = Instantiate(MS_Buff_Prefab, transform.position, Quaternion.identity, transform);
                    MS = true;
                }
                break;
            case _Rarity.OMEGA:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
 
    }   

    void PlayDeathParticle()
    {
        //Instantiate and store in a temporary variable
        //ParticleSystem explode = Instantiate(poof);
        //Destroy the Instantiated ParticleSystem                    
        //Destroy(explode, explodeDuration);
    }

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
            currentState = _EnemyState.DIE;
            //m_Animator.SetBool("Explode", true);
        }

        if (collision.gameObject.tag == "Bullet")
        {
            takeDamage(1);
            collision.gameObject.SetActive(false);
        }
    }

    public float GetMoveSpeed()
    {
        return moveSpeed;
    }
    public float GetRotationSpeed()
    {
        return rotationSpeed;
    }
}
