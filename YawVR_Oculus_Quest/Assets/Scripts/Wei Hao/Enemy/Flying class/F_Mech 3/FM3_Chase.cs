using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FM3_Chase : SMB_BaseEnemyState
{
    [Header("Enemy Chase State Configuration")]
    [SerializeField]
    protected float m_inRange = 50.0f;
    [SerializeField]
    protected float m_outRange = 150.0f;

    protected float inRangeSqr, outRangeSqr;

    public override void Check(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        float distanceSqr = CustomUtility.HitCheckRadius(enemy.m_target.transform.position, enemy.transform.position);
        //enemy.GetComponent<NavMeshAgent>().SetDestination(enemy.m_target.transform.position);
        if (distanceSqr < inRangeSqr)
            animator.SetBool("Chase_InRange", true);
        else if (distanceSqr > outRangeSqr)
            animator.SetBool("Chase_OutRange", true);
    }

    public override void Enter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        //enemy.GetComponent<NavMeshAgent>().isStopped = false;
        animator.SetBool("Chase_InRange", false);
        animator.SetBool("Chase_OutRange", false);
        inRangeSqr = m_inRange * m_inRange;
        outRangeSqr = m_outRange * m_outRange;
        //enemy.GetComponent<NavMeshAgent>().SetDestination(enemy.m_target.transform.position);
    }

    public override void Update(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        //relativePos = PlayerHandler.instance.transform.position - animator.transform.position;

        //Quaternion toRotation = Quaternion.LookRotation(new Vector3(relativePos.x, 0, relativePos.z));
        ////animator.transform.rotation = Quaternion.Slerp(animator.transform.rotation, toRotation, m_rotationSpeed * Time.deltaTime);

        ////animator.transform.position += animator.transform.forward * enemy.GetSpeed() * Time.deltaTime;
        ////enemy.GetComponent<NavMeshAgent>().Move(enemy.transform.forward * enemy.GetSpeed() * Time.deltaTime);

    }

    public override void Exit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        //enemy.GetComponent<NavMeshAgent>().isStopped = true;
    }
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
