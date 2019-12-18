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
public class Light_Enemy_1 : EnemyBase
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


    // Start is called before the first frame update
    void Start()
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

        //Debug.Log("Current Health = " + health);
        //Debug.Log("Current Max Health = " + maxHealth);
        //Debug.Log("Current dmg = " + damage);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 relativePos = target.position - transform.position;

        // Debugging distance
        float distance = Vector3.Distance(transform.position, target.position);
        if (Vector3.Distance(transform.position, target.position) >= minimumRange)
        {
            currentState = _EnemyState.CHASE;
            //m_Animator.SetBool("Chase", true);

            Debug.Log("Current State: " + currentState);
            //Debug.Log("Distance: " + distance);
            if (Vector3.Distance(transform.position, target.position) <= maximumRange)
            {
                Debug.Log("Within Range");
                currentState = _EnemyState.SHOOT;
                //transformX = transform.position;
                //m_Animator.SetBool("Explode", true);
            }
        }

        dodgeCheck -= 1.0f * Time.deltaTime;
        if (dodgeCheck <= 0.0f)
        {
            Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, 5.0f);
            for (int i = 0; i < hitColliders.Length; i++)
            {
                if (hitColliders[i].gameObject.tag == "Bullet")
                {
                    Debug.Log("Projectile detected");
                    currentState = _EnemyState.AVOID;
                }
            }
        }

        switch (currentState)
        {
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
        }
    }

    //IEnumerator EnemyShoot()
    //{
    //    BaseProjectile _projectileL = Instantiate(projectile, transform.position + (target.position - transform.position).normalized, Quaternion.LookRotation(target.position - transform.position), Persistent.instance.GO_DYNAMIC.transform).GetComponent<BaseProjectile>();
    //    _projectileL.Init(m_projectileOriginL);

    //    yield return new WaitForSeconds(0.2f);

    //    BaseProjectile _projectileR = Instantiate(projectile, transform.position + (target.position - transform.position).normalized, Quaternion.LookRotation(target.position - transform.position), Persistent.instance.GO_DYNAMIC.transform).GetComponent<BaseProjectile>();
    //    _projectileR.Init(m_projectileOriginR);
    //}

    IEnumerator EnemyDodge()
    {
        float random = Random.Range(1, 10);

        if (random > 5)
        {
            transform.position += transform.right * (moveSpeed * 1.5f) * Time.deltaTime;
        }
        else
        {
            transform.position += transform.right * (-moveSpeed * 1.5f) * Time.deltaTime;
        }

        yield return new WaitForSeconds(1.5f);

        //currentState = _EnemyState.CHASE;
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
            Debug.Log("Hit");
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
