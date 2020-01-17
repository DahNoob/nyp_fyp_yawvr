using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FM2_Chase : SMB_BaseEnemyState
{
    public Transform projectile;
    private Transform PlayerTransform;
    private Vector3 relativePos;

    [SerializeField]
    protected float m_detectRange = 15.0f;

    [Header("Enemy Chase State Configuration")]
    [SerializeField]
    protected float m_inRange = 50.0f;
    [SerializeField]
    protected float m_outRange = 150.0f;
    [SerializeField]
    protected float m_rotationSpeed = 6.0f;

    protected float inRangeSqr, outRangeSqr;

    public NavMeshAgent navMeshAgent;

    public override void Check(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        float distanceSqr = CustomUtility.HitCheckRadius(PlayerHandler.instance.transform.position, enemy.transform.position);
        //enemy.GetComponent<NavMeshAgent>().SetDestination(PlayerHandler.instance.transform.position);
        //if (distanceSqr < inRangeSqr)
            animator.SetBool("Attack", true);
        //else if (distanceSqr > outRangeSqr)
        //    animator.SetBool("Chase_OutRange", true);
    }

    public override void Enter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        //PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        navMeshAgent = animator.GetComponent<NavMeshAgent>();
        navMeshAgent.isStopped = false;
        //navMeshAgent.SetDestination(PlayerHandler.instance.transform.position);
        animator.SetBool("Cooldown", false);
    }

    public override void Update(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        //navMeshAgent.SetDestination(PlayerHandler.instance.transform.position);
    }

    public override void Exit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        
    }
}
