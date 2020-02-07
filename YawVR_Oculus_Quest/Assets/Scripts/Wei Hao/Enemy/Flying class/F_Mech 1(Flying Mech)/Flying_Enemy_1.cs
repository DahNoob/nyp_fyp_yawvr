using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
*******************************/
public class Flying_Enemy_1 : EnemyBase
{
    public enum _EnemyState
    {
        CHASE,
        SHOOT,
        AVOID,
        DIE,
    }

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
    private GameObject weightedRandom;

    private float projectileSpeed;
    private float amount = 1.0f; //how much it shakes
    private Vector3 transformX;

    [Header("Projectile Origin")]
    // Light Mech Shooting
    public Transform m_projectileOriginL;
    public Transform m_projectileOriginR;

    // Dodge Check
    private float dodgeCheck = 2.0f;

    Transform Parent;


    // Start is called before the first frame update
    new void Start()
    {
        // Current State
        //currentState = _EnemyState.AVOID;
        Player = GameObject.Find("Player");
        target = Player.GetComponent<Transform>();
        rb = gameObject.GetComponent<Rigidbody>();
        m_Animator = gameObject.GetComponentInChildren<Animator>();
        poof = gameObject.GetComponent<ParticleSystem>();
        weightedRandom = GameObject.Find("WeightedRNG");

        // Get rarity
        rarity = (_Rarity)weightedRandom.GetComponent<WeightedRandom>().random();

        transformX = transform.position;
        Parent = transform.parent;

        //Debug.Log("Current Health = " + health);
        //Debug.Log("Current Max Health = " + maxHealth);
        //Debug.Log("Current dmg = " + damage);
    }

    // Update is called once per frame
    override protected void Update()
    {
 

        //switch (currentState)
        //{
            
        //}
    }

    void StartHeal()
    {    
        Parent.GetChild(1).gameObject.SetActive(true);
    }

    void StopHeal()
    {
        Parent.GetChild(1).gameObject.SetActive(false);
    }

    void PlayDeathParticle()
    {
        //Instantiate and store in a temporary variable
        ParticleSystem explode = Instantiate(poof);
        //Destroy the Instantiated ParticleSystem                    
        Destroy(explode, explodeDuration);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Mech")
        {
            //Debug.Log("Hit");
            transformX = transform.position;
            currentState = _EnemyState.DIE;
            //m_Animator.SetBool("Explode", true);
        }

        if (collision.gameObject.tag == "Bullet")
        {
            takeDamage(1);
            collision.gameObject.SetActive(false);
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
