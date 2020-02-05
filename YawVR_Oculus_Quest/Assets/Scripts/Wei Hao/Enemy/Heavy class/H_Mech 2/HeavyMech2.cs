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
public class HeavyMech2 : EnemyBase ,IPooledObject
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
    protected ParticleSystem m_effectRight_Wind;
    [SerializeField]
    protected ParticleSystem m_effectLeft_Wind;
    [SerializeField]
    protected ParticleSystem m_effectRight_Spawn;
    [SerializeField]
    protected ParticleSystem m_effectLeft_Spawn;
    [SerializeField]
    protected GameObject m_lesserEnemy;
    [SerializeField]
    protected AudioSource m_spawnSpinSound;
    [SerializeField]
    protected AudioSource m_spawnSound;

    //Local variables
    protected _GameStates m_currentState = _GameStates.WALK;
    protected bool activeSideIsRight = false;
    protected float spawnRechargeTimer;
    protected List<Collider> ignoredColliders = new List<Collider>();

    //New variables
    private Animator anim;
    private NavMeshAgent navMeshAgent;

    public void OnObjectSpawn()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = new Vector3(0f, 0f, 0f);
        rb.angularVelocity = new Vector3(0f, 0f, 0f);

        Start();
        SetIconColor(Color.white);
        SetIconSprite();
    }

    public void OnObjectDestroy()
    {
        ObjectPooler.instance.SpawnFromPool(PoolObject.OBJECTTYPES.ENEMY_DEATH_EFFECT, transform.position + GetComponent<SphereCollider>().center, Quaternion.identity);
        anim.SetBool("ResetAnim", true);
        RemoveFromQuadTree(this.gameObject);
        this.gameObject.SetActive(false);
        ObjectPooler.instance.DisableInPool(PoolObject.OBJECTTYPES.HEAVY_MECH2);
    }

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        health = GetMaxHealth();
        spawnRechargeTimer = Time.time;

        anim = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>(); 

        AddToQuadTree(this.gameObject, QuadTreeManager.DYNAMIC_TYPES.ENEMIES); 
    }

    override protected void Update()
    {
        base.Update();
        bool spawnRecharged = Time.time > spawnRechargeTimer;
        anim.SetBool("Flee_Done", spawnRecharged);
        anim.SetBool("Walk_DoFlee", !spawnRecharged);
        if (navMeshAgent.velocity == Vector3.zero)
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
        //    takeDamage(10);
        //    //collision.gameObject.SetActive(false);
        //}
    }

    //shite implementation but wutever
    public void EnterSpawn()
    {
        m_currentState = _GameStates.SPAWN;
        GetComponent<NavMeshAgent>().isStopped = true;
        GetComponent<NavMeshAgent>().obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        m_spawnSpinSound.Play();
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
        m_effectLeft_Wind.Stop();
        m_effectRight_Wind.Stop();
        m_spawnSpinSound.Stop();
    }

    public void SpawnEnemy()
    {
        Transform spawnTransform = activeSideIsRight ? m_spawnPivotRight : m_spawnPivotLeft;
        //Collider newEnemy = Instantiate(m_lesserEnemy, spawnTransform.position, transform.rotation, Persistent.instance.GO_DYNAMIC.transform).GetComponent<Collider>();
        Collider newEnemy = ObjectPooler.instance.SpawnFromPool(PoolObject.OBJECTTYPES.LIGHT_MECH2, spawnTransform.position, transform.rotation).GetComponent<Collider>();
        newEnemy.GetComponent<EnemyBase>().m_target = m_target;
        Physics.IgnoreCollision(GetComponent<Collider>(), newEnemy, true);
        ignoredColliders.Add(newEnemy);
        newEnemy.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, 500000.0f, 1200000.0f));
        m_spawnSound.Play();
        if (activeSideIsRight)
            m_effectRight_Spawn.Emit(1);
        else
            m_effectLeft_Spawn.Emit(1);
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
        anim.SetFloat("Blend", activeSideIsRight ? 1.0f : 0.0f);
        if (activeSideIsRight)
            m_effectRight_Wind.Play();
        else
            m_effectLeft_Wind.Play();
        //activeSideIsRight = !activeSideIsRight;
    }
}
