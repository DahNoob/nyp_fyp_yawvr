using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LM1_Idle : SMB_BaseEnemyState
{
    [Header("Enemy Walk State Configuration")]
    [SerializeField]
    protected float m_detectRange = 70.0f;
    //[SerializeField]
    //protected float m_fieldOfViewAngle = 60.0f;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void Check(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //PlayerHandler player = PlayerHandler.instance;
        animator.SetBool("Alert", CustomUtility.IsHitRadius(enemy.m_target.position, enemy.transform.position, m_detectRange));
        //Debug.Log("Is the LM1's target nearby? " + animator.GetBool("Alert"));
    }

    public override void Enter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    public override void Update(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //animator.SetBool("Walk", true);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    public override void Exit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

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
