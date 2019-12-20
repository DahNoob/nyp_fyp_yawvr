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
    protected Transform m_rightSide;
    [SerializeField]
    protected Transform m_leftSide;
    [SerializeField]
    protected GameObject m_lesserEnemy;

    protected _GameStates m_currentState = _GameStates.WALK;
    protected bool activeSideIsRight = false;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
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
        GetComponent<NavMeshAgent>().isStopped = true;
        FlipActiveSide();
    }
    public void ExitSpawn()
    {
        m_currentState = _GameStates.CHASE;
    }
    public void SpawnEnemy()
    {
        Transform spawnTransform = activeSideIsRight ? m_rightSide : m_leftSide;
        Instantiate(m_lesserEnemy, spawnTransform.position, spawnTransform.rotation, Persistent.instance.GO_DYNAMIC.transform);
    }
    public void FlipActiveSide()
    {
        RaycastHit hitLeft, hitRight;
        bool hittedRight = Physics.Raycast(new Ray(transform.position, transform.right), out hitRight, 7.0f);
        bool hittedLeft = Physics.Raycast(new Ray(transform.position, -transform.right), out hitLeft, 7.0f);
        if (hittedLeft && hittedRight)
        {
            activeSideIsRight = hitRight.distance > hitLeft.distance ? true : false;
        }
        else
        {
            activeSideIsRight = hittedRight;
        }
        GetComponent<Animator>().SetFloat("Blend", activeSideIsRight ? 1.0f : 0.0f);
        //activeSideIsRight = !activeSideIsRight;
    }
}
