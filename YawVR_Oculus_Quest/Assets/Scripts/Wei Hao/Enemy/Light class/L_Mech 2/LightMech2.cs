using System;
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

    private enum _Buffs
    {
        HP,
        DMG,
        MS
    }
    List<string> buffs = new List<string> { "HP", "DMG", "MS" };

    private GameObject Player;
    [Header("Light Mech 2 Configuration")]
    [SerializeField]
    // Chase range
    protected int chaseRange = 10;
    [SerializeField]
    // Attack range
    protected int attackRange = 4;
    [SerializeField]
    // LM2 rotation speed
    protected int rotationSpeed = 5;
    [SerializeField]
    // Time taken before attack is activated
    protected float attackWindUp = 2.0f;

    [Header("Current State")]
    [SerializeField]
    private _GameStates currentState = _GameStates.CHASE;

    private Rigidbody rb;

    //Fetch the Animator
    Animator m_Animator;

    //// Particle effect when baneling explodes
    //public ParticleSystem poof;
    //float explodeDuration = 1.0f;



    //[Header("Explosion")]
    //private float speed = 1.0f; //how fast it shakes
    //private float amount = 1.0f; //how much it shakes
    //private Vector3 transformX;

    //private GameObject passiveBuffs;

    [Header("Rarity")]
    [SerializeField]
    private _Rarity rarity;
    private GameObject weightedRandom;

    [Header("Death Particle Effect")]
    [SerializeField]
    public GameObject explosionPrefab;
    private GameObject explosion;

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
        // Current State
        //currentState = _GameStates.CHASE;
        GetComponent<UnityEngine.AI.NavMeshAgent>().updatePosition = false;
        Player = GameObject.Find("Player");
        rb = gameObject.GetComponent<Rigidbody>();
        m_Animator = gameObject.GetComponentInChildren<Animator>();
        //poof = gameObject.GetComponent<ParticleSystem>();
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
        }
        else if (rarity == _Rarity.BETA)
        {
            string secondBuff = StartBuff();

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
    }

    public void PlayDeathParticle()
    {
        //explosion = Instantiate(explosionPrefab, transform.position, transform.rotation);
    }

    public string StartBuff()
    {
        System.Random random = new System.Random();
        int index = random.Next(buffs.Count);
        var selectedBuff = buffs[index];
        buffs.RemoveAt(index);
        return selectedBuff;
    }

    void OnTriggerEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Mech")
        {
            Debug.Log("Hit");
            //transformX = transform.position;
            //currentState = _GameStates.EXPLODE;
            m_Animator.SetBool("Chase", false);
            m_Animator.SetBool("Explode", true);
        }

        if (collision.gameObject.tag == "Bullet")
        {
            takeDamage(1);
            collision.gameObject.SetActive(false);
        }
    }
    //void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Mech")
    //    {
    //        Debug.Log("Hit");
    //        //transformX = transform.position;
    //        //currentState = _GameStates.EXPLODE;
    //        m_Animator.SetBool("Chase", false);
    //        m_Animator.SetBool("Explode", true);
    //    }

    //    if (collision.gameObject.tag == "Bullet")
    //    {
    //        takeDamage(1);
    //        collision.gameObject.SetActive(false);
    //    }
    //}
}
