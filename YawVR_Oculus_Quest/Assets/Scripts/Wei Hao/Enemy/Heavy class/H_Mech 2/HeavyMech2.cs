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
** 2    27/12/2019, 11:47 AM    DahNoob   Implemented spawning recharge time
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
    protected float m_spawnRechargeTime = 10.0f;
    [SerializeField]
    protected Rigidbody m_rigidBody;
    [SerializeField]
    protected Transform m_coreTransform;
    [SerializeField]
    protected Transform m_spawnPivotRight;
    [SerializeField]
    protected Transform m_spawnPivotLeft;
    [SerializeField]
    protected GameObject m_lesserEnemy;

    //Local variables
    protected _GameStates m_currentState = _GameStates.WALK;
    protected bool activeSideIsRight = false;
    protected float spawnRechargeTimer;
    protected List<Collider> ignoredColliders = new List<Collider>();

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        spawnRechargeTimer = Time.time;
        AddToQuadTree(this.gameObject);
    }

    void Update()
    {
        bool spawnRecharged = Time.time > spawnRechargeTimer;
        Animator anim = GetComponent<Animator>();
        NavMeshAgent nav = GetComponent<NavMeshAgent>();
        anim.SetBool("Flee_Done", spawnRecharged);
        anim.SetBool("Walk_DoFlee", !spawnRecharged);
        if (nav.velocity == Vector3.zero)
        {
            anim.SetFloat("AnimSpeed", 0);
            return;
        }
        anim.SetFloat("AnimSpeed", GetComponent<NavMeshAgent>().velocity.magnitude / GetComponent<NavMeshAgent>().speed);
        
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
        GetComponent<NavMeshAgent>().obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;

        FlipActiveSide();

    }
    public void ExitSpawn()
    {
        m_currentState = _GameStates.CHASE;
        spawnRechargeTimer = Time.time + m_spawnRechargeTime;
        GetComponent<NavMeshAgent>().isStopped = false;
        GetComponent<NavMeshAgent>().obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        Collider col = GetComponent<Collider>();
        for (int i = 0; i < ignoredColliders.Count; ++i)
        {
            Physics.IgnoreCollision(col, ignoredColliders[i], false);
        }
        ignoredColliders.Clear();
    }
    public void SpawnEnemy()
    {
        Transform spawnTransform = activeSideIsRight ? m_spawnPivotRight : m_spawnPivotLeft;
        Collider newEnemy = Instantiate(m_lesserEnemy, spawnTransform.position, transform.rotation, Persistent.instance.GO_DYNAMIC.transform).GetComponent<Collider>();
        Physics.IgnoreCollision(GetComponent<Collider>(), newEnemy, true);
        ignoredColliders.Add(newEnemy);
        newEnemy.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, 5000.0f, 12000.0f));
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
