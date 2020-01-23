using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LM1_Chase : SMB_BaseEnemyState
{
    [SerializeField]
    // Dodge Check
    protected float dodgeCheck = 2.0f;
    [SerializeField]
    protected float m_DodgeDetectRange = 10.0f;
    [SerializeField]
    protected float m_detectRange = 15.0f;
    [SerializeField]
    protected float m_inRange = 50.0f;
    [SerializeField]
    protected float m_outRange = 150.0f;
    [SerializeField]
    protected float m_rotationSpeed = 6.0f;

    [Header("LM1 Chase State Configuration")]
    [SerializeField]
    protected Transform projectile;

    protected float inRangeSqr, outRangeSqr;
    // Time taken before attack is activated
    private float attackWindUp = 0.0f;

    private bool shoot = false;

    public override void Check(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        //PlayerHandler player = PlayerHandler.instance;

        float distanceSqr = CustomUtility.HitCheckRadius(enemy.m_target.position, enemy.transform.position);
        //enemy.GetComponent<NavMeshAgent>().SetDestination(PlayerHandler.instance.transform.position);
        if (distanceSqr < inRangeSqr)
            animator.SetBool("Shoot", true);
        //animator.SetBool("Dodge", CustomUtility.IsHitRadius(player.transform.position, enemy.transform.position, m_DodgeDetectRange));
        //Debug.Log("Is it time to dodge? " + animator.GetBool("Dodge"));
        //animator.SetBool("Shoot", CustomUtility.IsHitRadius(player.transform.position, enemy.transform.position, m_detectRange));
        //Debug.Log("Stand and shoot? " + animator.GetBool("Shoot"));
        //Debug.Log("Hit Check Radius: " + CustomUtility.HitCheckRadius(enemy.m_target.position, enemy.transform.position));
        enemy.navMeshAgent.SetDestination(enemy.m_target.position);
        //if (CustomUtility.HitCheckRadius(player.transform.position,enemy.transform.position) < m_detectRange * m_detectRange)
        //{
        //    navMeshAgent.isStopped = true;
        //    animator.SetBool("Shoot", true);
        //    //navMeshAgent.isStopped = false;
        //}

        //float distanceSqr = CustomUtility.HitCheckRadius(PlayerHandler.instance.transform.position, enemy.transform.position);
        ////animator.SetBool("Shoot", CustomUtility.IsHitRadius(player.transform.position, enemy.transform.position, m_detectRange));
        ////Debug.Log("Stand and shoot? " + animator.GetBool("Shoot"));
        ////enemy.GetComponent<NavMeshAgent>().SetDestination(PlayerHandler.instance.transform.position);
        //if (distanceSqr < inRangeSqr)
        //    animator.SetBool("Shoot", true);
        //else if (distanceSqr > outRangeSqr)
        //    animator.SetBool("Shoot", false);

        //Debug.Log("Stand and shoot? " + animator.GetBool("Shoot"));
    }

    public override void Enter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        //animator.SetBool("Chase_InRange", false);
        //animator.SetBool("Chase_OutRange", false);
        //inRangeSqr = m_inRange * m_inRange;
        //outRangeSqr = m_outRange * m_outRange;
       
        enemy.navMeshAgent.isStopped = false;
        enemy.navMeshAgent.SetDestination(enemy.m_target.position);
        animator.SetBool("DodgeEnd", false);
        
    }

    public override void Update(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        //navMeshAgent.SetDestination(enemy.m_target.position);
        //relativePos = PlayerTransform.position - animator.transform.position;

        //Quaternion toRotation = Quaternion.LookRotation(new Vector3(relativePos.x, 0, relativePos.z));
        //animator.transform.rotation = Quaternion.Lerp(animator.transform.rotation, toRotation, /*rotationSpeed */ animator.GetComponent<Light_Enemy_1>().GetRotationSpeed() * Time.deltaTime);

        //animator.transform.position += animator.transform.forward * /*moveSpeed*/ animator.GetComponent<Light_Enemy_1>().GetMoveSpeed() * Time.deltaTime;

        //relativePos = PlayerHandler.instance.transform.position - animator.transform.position;

        //Quaternion toRotation = Quaternion.LookRotation(new Vector3(relativePos.x, 0, relativePos.z));

        attackWindUp -= 1.0f * Time.deltaTime;
        if (attackWindUp <= 0.0f)
        {
            shoot = true;
            if (shoot)
            {
                animator.GetComponent<Light_Enemy_1>().Shoot_Async();//StartCoroutine(EnemyShoot(animator));
                attackWindUp = 2.0f;
                shoot = false;
            }
        }

        //dodgeCheck -= 1.0f * Time.deltaTime;
        //if (dodgeCheck <= 0.0f)
        //{
           // Check(animator, animatorStateInfo, layerIndex);
            //dodgeCheck = 2.0f;
            //Collider[] hitColliders = Physics.OverlapSphere(animator.transform.position, 5.0f);
            //for (int i = 0; i < hitColliders.Length; i++)
            //{
            //    if (hitColliders[i].gameObject.tag == "Bullet")
            //    {
            //        Debug.Log("Projectile detected");
            //        animator.SetBool("Dodge", true);
            //    }
            //}
        //}
    }

    public override void Exit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        
    }

    //IEnumerator EnemyShoot(Animator animator)
    //{
    //    Light_Enemy_1 lm1 = enemy.GetComponent<Light_Enemy_1>();
    //    //Debug.Log("Fire start");
    //    BaseProjectile _projectileL = ObjectPooler.instance.SpawnFromPool(PoolObject.OBJECTTYPES.ENEMY_PROJECTILE, lm1.leftProjectileOrigin.position, Quaternion.identity).GetComponent<BaseProjectile>();
    //    lm1.leftProjectileOrigin.LookAt(enemy.m_target);
    //    _projectileL.Init(lm1.leftProjectileOrigin);

    //    yield return new WaitForSeconds(0.2f);

    //    //Debug.Log("2nd Fire start");
    //    BaseProjectile _projectileR = ObjectPooler.instance.SpawnFromPool(PoolObject.OBJECTTYPES.ENEMY_PROJECTILE, lm1.rightProjectileOrigin.position, Quaternion.identity).GetComponent<BaseProjectile>();
    //    lm1.rightProjectileOrigin.LookAt(enemy.m_target);
    //    _projectileR.Init(lm1.rightProjectileOrigin);
    //}


    //// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    //PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    //    //m_projectileOriginL = animator.transform.Find("LBlaster Projectile Origin");
    //    //m_projectileOriginR = animator.transform.Find("RBlaster Projectile Origin");
    //    //m_projectileOriginR
    //}

    //// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{

    //    //dodgeCheck -= 1.0f * Time.deltaTime;
    //    //if (dodgeCheck <= 0.0f)
    //    //{
    //    //    Collider[] hitColliders = Physics.OverlapSphere(animator.transform.position, 5.0f);
    //    //    for (int i = 0; i < hitColliders.Length; i++)
    //    //    {
    //    //        if (hitColliders[i].gameObject.tag == "Player" || hitColliders[i].gameObject.tag == "Mech")
    //    //        {
    //    //            Debug.Log("Projectile detected");
    //    //            animator.SetBool("Dodge", true);                    
    //    //        }
    //    //    }
    //    //    dodgeCheck = 2.0f;
    //    //}
    //}

    //// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{

    //}
}
