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
        poof = gameObject.GetComponent<ParticleSystem>();
        weightedRandom = GameObject.Find("WeightedRNG");

        // Get rarity
        rarity = (_Rarity)weightedRandom.GetComponent<WeightedRandom>().random();

        transformX = transform.position;
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

    public float GetMoveSpeed()
    {
        return moveSpeed;
    }
    public float GetRotationSpeed()
    {
        return rotationSpeed;
    }
}
