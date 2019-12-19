using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/******************************  
** Name: idk enemy
** Desc: derderereredeprpepdpepr
** Author: DahNoob
** Date: 18/12/2019, 11:10 AM
**************************
** Change History
**************************
** PR   Date                    Author    Description 
** --   --------                -------   ------------------------------------
** 1    18/12/2019, 11:10 AM    DahNoob   Created
*******************************/
[RequireComponent(typeof(Rigidbody))]
public class HeavyMech2 : EnemyBase
{
    // Banelings states
    protected enum _GameStates
    {
        WALK,
        CHASE,
        SPAWN
    }

    [Header("Heavy Mech 2 Configuration")]
    [SerializeField]
    protected int m_chaseRange = 10;
    [SerializeField]
    protected int m_attackRange = 4;
    [SerializeField]
    protected int m_rotationSpeed = 5;
    [SerializeField]
    protected float m_attackWindUp = 2.0f;
    [SerializeField]
    protected Rigidbody m_rigidBody;
    [SerializeField]
    protected Transform m_coreTransform;
    [SerializeField]
    protected Vector3 m_coreRightPosition;
    [SerializeField]
    protected Vector3 m_coreLeftPosition;
    [SerializeField]
    protected GameObject m_lesserEnemy;
    [SerializeField]
    protected GameObject m_dieEffect;

    protected _GameStates m_currentState = _GameStates.WALK;
    protected bool coreIsRightSide = false;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        //currentState = _GameStates.CHASE;
        //Player = GameObject.Find("Player");
        //rb = gameObject.GetComponent<Rigidbody>();
        //m_Animator = gameObject.GetComponentInChildren<Animator>();
        //poof = gameObject.GetComponent<ParticleSystem>();

        //transformX = transform.position;

        //Debug.Log("Current Health = " + health);
        //Debug.Log("Current Max Health = " + maxHealth);
        //Debug.Log("Current dmg = " + damage);
    }

    // Update is called once per frame
    void Update()
    {
        //Vector3 relativePos = Player.transform.position - transform.position;

        //// Debugging distance
        //float distance = Vector3.Distance(transform.position, Player.transform.position);
        ////if (Vector3.Distance(transform.position, Player.transform.position) >= attackRange)
        ////{
        ////currentState = _GameStates.CHASE;
        ////m_Animator.SetBool("Chase", true);

        //Debug.Log("Current State: " + currentState);
        //Debug.Log("Distance: " + distance);
        ////if (Vector3.Distance(transform.position, Player.transform.position) <= attackRange)
        ////{
        ////    transformX = transform.position;
        ////    m_Animator.SetBool("Explode", true);
        ////}
        //// }

        //switch (currentState)
        //{
        //    case _GameStates.CHASE:
        //        Quaternion toRotation = Quaternion.LookRotation(new Vector3(relativePos.x, 0, relativePos.z));
        //        transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);

        //        transform.position += transform.forward * moveSpeed * Time.deltaTime;
        //        break;
        //    case _GameStates.EXPLODE:
        //        attackWindUp -= 1.0f * Time.deltaTime;

        //        transformX.x = Mathf.Sin(Time.time * speed) * amount;

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

    void FixedUpdate()
    {
        if (m_currentState == _GameStates.SPAWN)
        {
            m_coreTransform.localPosition = Vector3.Lerp(m_coreTransform.localPosition, coreIsRightSide ? m_coreRightPosition : m_coreLeftPosition, 0.05f);
        }
        else
        {
            //m_rightSide.localPosition = Vector3.Lerp(m_rightSide.localPosition, Vector3.zero, 0.05f);
            //m_leftSide.localPosition = Vector3.Lerp(m_leftSide.localPosition, Vector3.zero, 0.05f);
            m_coreTransform.localPosition = Vector3.Lerp(m_coreTransform.localPosition, Vector3.zero, 0.075f);
        }
    }

    void PlayDeathParticle()
    {
        ////Instantiate and store in a temporary variable
        //ParticleSystem explode = Instantiate(poof);
        ////Destroy the Instantiated ParticleSystem                    
        //Destroy(explode, explodeDuration);
    }

    void OnCollisionEnter(Collision collision)
    {
        //if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Mech")
        //{
        //    Debug.Log("Hit");
        //    transformX = transform.position;
        //    currentState = _GameStates.EXPLODE;
        //    m_Animator.SetBool("Chase", false);
        //    m_Animator.SetBool("Explode", true);
        //}

        //if (collision.gameObject.tag == "Bullet")
        //{
        //    takeDamage(1);
        //    collision.gameObject.SetActive(false);
        //}
    }

    //shite implementation but wutever
    public void EnterSpawn()
    {
        m_currentState = _GameStates.SPAWN;
    }
    public void ExitSpawn()
    {
        m_currentState = _GameStates.CHASE;
    }
    public void SpawnEnemy()
    {
        coreIsRightSide = !coreIsRightSide;
        Instantiate(m_lesserEnemy, m_coreTransform.position, m_coreTransform.rotation, Persistent.instance.GO_DYNAMIC.transform);
    }

    void OnDestroy()
    {
        Instantiate(m_dieEffect, transform.position, Quaternion.identity, Persistent.instance.GO_DYNAMIC.transform);
    }
}
