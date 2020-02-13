﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingMech3 : EnemyBase, IPooledObject
{
    private enum _Buffs
    {
        HP,
        DMG,
        MS
    }
    List<string> buffs = new List<string> { "HP", "DMG", "MS" };

    private GameObject Player;
    //[Header("Flying Mech 3 Colliders")]
    //[SerializeField]
    //protected Collider Body;
    //[SerializeField]
    //protected Collider WeakPoint_1;
    //[SerializeField]
    //protected Collider WeakPoint_2;

    private Rigidbody rb;

    //Fetch the Animator
    Animator m_Animator;

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

    //[Header("Flying Mech 3 Configuration")]
    //[SerializeField]
    //protected Rigidbody m_rigidBody;
    //[SerializeField]
    //protected Transform m_coreTransform;
    //[SerializeField]
    //protected GameObject m_lesserEnemy;

    protected List<Collider> ignoredColliders = new List<Collider>();

    //[SerializeField]
    //protected GameObject m_WeakPoint_1;
    //[SerializeField]
    //protected GameObject m_WeakPoint_2;

    //[Header("Flying Mech 3 Configuration")]
    [SerializeField]
    protected GameObject LM_Spawn1;
    [SerializeField]
    protected GameObject LM_Spawn2;

    public void OnObjectSpawn()
    {
        //Rigidbody rb = GetComponent<Rigidbody>();
        //rb.velocity = new Vector3(0f, 0f, 0f);
        //rb.angularVelocity = new Vector3(0f, 0f, 0f);

        Start();
        SetIconColor(Color.white);
        SetIconSprite();
    }

    public void OnObjectDestroy()
    {
        ObjectPooler.instance.SpawnFromPool(PoolObject.OBJECTTYPES.ENEMY_DEATH_EFFECT, transform.position + GetComponent<SphereCollider>().center, Quaternion.identity);
        m_Animator.SetBool("ResetAnim", true);
        RemoveFromQuadTree(this.gameObject);
        this.gameObject.SetActive(false);
        ObjectPooler.instance.DisableInPool(PoolObject.OBJECTTYPES.FLYING_MECH3);
    }

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        GetComponent<UnityEngine.AI.NavMeshAgent>().updatePosition = false;
        Player = PlayerHandler.instance.gameObject;
        rb = gameObject.GetComponent<Rigidbody>();
        m_Animator = gameObject.GetComponentInChildren<Animator>();
        weightedRandom = GameObject.Find("WeightedRNG");

        // Get rarity
        rarity = (_Rarity)weightedRandom.GetComponent<WeightedRandom>().random();

        string currBuff = StartBuff();
        if (rarity == _Rarity.DELTA)
        {
            if (currBuff == "HP")
            {
                GameObject hpBuff = Instantiate(HP_Buff_Prefab, transform.position, Quaternion.identity, transform);
                HP = true;
            }
            else if (currBuff == "DMG")
            {
                GameObject dmgBuff = Instantiate(DMG_Buff_Prefab, transform.position, Quaternion.identity, transform);
                DMG = true;
            }
            else if (currBuff == "MS")
            {
                GameObject msBuff = Instantiate(MS_Buff_Prefab, transform.position, Quaternion.identity, transform);
                MS = true;
            }
        }
        else if (rarity == _Rarity.BETA)
        {
            string secondBuff = StartBuff();

            if (currBuff == "HP" || secondBuff == "HP")
            {
                GameObject hpBuff = Instantiate(HP_Buff_Prefab, transform.position, Quaternion.identity, transform);
                HP = true;
            }
            else if (currBuff == "DMG" || secondBuff == "DMG")
            {
                GameObject dmgBuff = Instantiate(DMG_Buff_Prefab, transform.position, Quaternion.identity, transform);
                DMG = true;
            }
            else if (currBuff == "MS" || secondBuff == "MS")
            {
                GameObject msBuff = Instantiate(MS_Buff_Prefab, transform.position, Quaternion.identity, transform);
                MS = true;
            }
        }
        else if (rarity == _Rarity.ALPHA)
        {
            string secondBuff = StartBuff();
            string thirdBuff = StartBuff();

            if (currBuff == "HP" || secondBuff == "HP" || thirdBuff == "HP")
            {
                GameObject hpBuff = Instantiate(HP_Buff_Prefab, transform.position, Quaternion.identity, transform);
                HP = true;
            }
            else if (currBuff == "DMG" || secondBuff == "DMG" || thirdBuff == "DMG")
            {
                GameObject dmgBuff = Instantiate(DMG_Buff_Prefab, transform.position, Quaternion.identity, transform);
                DMG = true;
            }
            else if (currBuff == "MS" || secondBuff == "MS" || thirdBuff == "MS")
            {
                GameObject msBuff = Instantiate(MS_Buff_Prefab, transform.position, Quaternion.identity, transform);
                MS = true;
            }
        }
        else if (rarity == _Rarity.OMEGA)
        {
            //string secondBuff = StartBuff();
            //string thirdBuff = StartBuff();
            //string fourthBuff = StartBuff();

            //if (currBuff == "HP" || secondBuff == "HP" || thirdBuff == "HP")
            //{
            //    GameObject hpBuff = Instantiate(HP_Buff_Prefab, transform.position, Quaternion.identity, transform);
            //    HP = true;
            //}
            //if (currBuff == "DMG" || secondBuff == "DMG" || thirdBuff == "DMG")
            //{
            //    GameObject dmgBuff = Instantiate(DMG_Buff_Prefab, transform.position, Quaternion.identity, transform);
            //    DMG = true;
            //}
            //if (currBuff == "MS" || secondBuff == "MS" || thirdBuff == "MS")
            //{
            //    GameObject msBuff = Instantiate(MS_Buff_Prefab, transform.position, Quaternion.identity, transform);
            //    MS = true;
            //}
        }

        //Add to Quadtree
        AddToQuadTree(this.gameObject, QuadTreeManager.DYNAMIC_TYPES.ENEMIES);        
    }

    // Update is called once per frame
    override protected void Update()
    {
        //Vector3 relativePos = Player.transform.position - transform.position;

        // Debugging distance
        //float distance = Vector3.Distance(transform.position, Player.transform.position);
        //if (Vector3.Distance(transform.position, Player.transform.position) >= attackRange)
        //{
        //currentState = _GameStates.CHASE;
        //m_Animator.SetBool("Chase", true);

        //Debug.Log("Current State: " + currentState);
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

        //Update the bounds
        queryBounds.position = transform.position;
    }

    public string StartBuff()
    {
        System.Random random = new System.Random();
        int index = random.Next(buffs.Count);
        var selectedBuff = buffs[index];
        buffs.RemoveAt(index);
        return selectedBuff;
    }

    public void SpawnEnemy(/*Transform LM_Spawn1, Transform LM_Spawn2*/)
    {
        EnemyBase newEnemy = ObjectPooler.instance.SpawnFromPool(PoolObject.OBJECTTYPES.LIGHT_MECH1, LM_Spawn1.transform.position, transform.rotation).GetComponent<EnemyBase>();
        //newEnemy.GetComponent<EnemyBase>().m_target = m_target;

        EnemyBase newEnemy2 = ObjectPooler.instance.SpawnFromPool(PoolObject.OBJECTTYPES.LIGHT_MECH1, LM_Spawn2.transform.position, transform.rotation).GetComponent<EnemyBase>();
        //newEnemy2.GetComponent<EnemyBase>().m_target = m_target;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            Debug.Log("Health = " + GetHealth());
        }
        //if(collision.collider == Body)
        //{
        //    Debug.Log("Body Hit");
        //}
        //if (collision.collider == WeakPoint_1)
        //{
        //    Debug.Log("WP_1 Hit");
        //}
        //if (collision.collider == WeakPoint_2)
        //{
        //    Debug.Log("WP_2 Hit");
        //}
    }

    public void CollisionDetected(WeakPoint child)
    {
        //takeDamage(5);
        Debug.Log("Child object collided");
    }
}
