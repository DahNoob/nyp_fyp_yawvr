using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************  
** Name: Banelings
** Desc: Baneling's beheviour
** Author: Wei Hao
** Date: 10/12/2019, 1:06 AM
**************************
** Change History
**************************
** PR   Date                    Author    Description 
** --   --------                -------   ------------------------------------
** 1    10/12/2019, 1:06 PM     Wei Hao   Created & Implemented
** 2    13/12/2019, 1:33 PM     Wei Hao   Updated basic animation
** 3    18/12/2019, 1:50 PM     Wei Hao   Added rarity for enemy
*******************************/
public class LightMech2 : EnemyBase
{
    // Banelings states
    private enum _GameStates
    {
        CHASE,
        EXPLODE,
    }

    private GameObject Player;
    // Chase range
    private int chaseRange = 10;
    // Attack range
    private int attackRange = 4;
    // Baneling rotation speed
    private int rotationSpeed = 5;
    // Time taken before attack is activated
    private float attackWindUp = 2.0f;

    private Rigidbody rb;

    //Fetch the Animator
    Animator m_Animator;

    // Particle effect when baneling explodes
    public ParticleSystem poof;
    float explodeDuration = 1.0f;

    [Header("Current State")]
    [SerializeField]
    private _GameStates currentState;

    [Header("Explosion")]
    private float speed = 1.0f; //how fast it shakes
    private float amount = 1.0f; //how much it shakes
    private Vector3 transformX;

    [Header("Rarity")]
    [SerializeField]
    private _Rarity rarity;
    private GameObject weightedRandom;

    // Start is called before the first frame update
    void Start()
    {
        // Current State
        //currentState = _GameStates.CHASE;

        Player = GameObject.Find("Player");
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
        Vector3 relativePos = Player.transform.position - transform.position;

        // Debugging distance
        float distance = Vector3.Distance(transform.position, Player.transform.position);
        //if (Vector3.Distance(transform.position, Player.transform.position) >= attackRange)
        //{
            //currentState = _GameStates.CHASE;
            //m_Animator.SetBool("Chase", true);

            Debug.Log("Current State: " + currentState);
            //Debug.Log("Distance: " + distance);
            //if (Vector3.Distance(transform.position, Player.transform.position) <= attackRange)
            //{
            //    transformX = transform.position;
            //    m_Animator.SetBool("Explode", true);
            //}
       // }

        //switch (currentState)
        //{
        //    case _GameStates.CHASE:
        //        //Quaternion toRotation = Quaternion.LookRotation(new Vector3(relativePos.x, 0, relativePos.z));
        //        //transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);

        //        //transform.position += transform.forward * moveSpeed * Time.deltaTime;

        //        //m_Animator.SetBool("Chase", true);
        //        break;
        //    case _GameStates.EXPLODE:
        //        attackWindUp -= 1.0f * Time.deltaTime;

        //        //transformX.x = Mathf.Sin(Time.time * speed) * amount;

        //        if (attackWindUp <= 0.0f)
        //        {
        //            Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, 5.0f);
        //            for (int i = 0; i < hitColliders.Length; i++)
        //            {
        //                if (hitColliders[i].gameObject.tag == "Player")
        //                {
        //                    //ImpactReceiver IR = hitColliders[i].transform.GetComponent<ImpactReceiver>();
        //                    //Vector3 dir = hitColliders[i].transform.position - transform.position;
        //                    //float force = Mathf.Clamp(/*explosion force*/ 50 / 3, 0, 15); 
        //                    //IR.AddImpact(dir, force);
        //                    //Player.gameObject.GetComponent<Player>().takeDamage(damage);  
        //                    //poof.Play();
        //                    Debug.Log("B00M !!");
        //                    //Debug.Log("Player health = " + Player.GetComponent<Player>().GetHealth());
        //                }
        //            }
        //            //PlayDeathParticle();
        //            gameObject.SetActive(false);
        //        }
        //        break;
        //}
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
            currentState = _GameStates.EXPLODE;
            m_Animator.SetBool("Chase", false);
            m_Animator.SetBool("Explode", true);
        }

        if (collision.gameObject.tag == "Bullet")
        {
            takeDamage(1);
            collision.gameObject.SetActive(false);
        }
    }
}
