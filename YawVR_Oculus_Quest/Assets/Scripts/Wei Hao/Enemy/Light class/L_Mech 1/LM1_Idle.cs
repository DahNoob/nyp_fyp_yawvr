using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LM1_Idle : SMB_BaseEnemyState
{
    [Header("Enemy Walk State Configuration")]
    [SerializeField]
    protected float m_detectRange = 10.0f;
    [SerializeField]
    protected float m_detectLongRange = 80.0f;
    [SerializeField]
    protected float m_fieldOfViewAngle = 60.0f;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void CheckState(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(!animator.GetBool("Alert"))
        {
            bool isInViewRange = Vector2.Angle(CustomUtility.ToVector2(enemy.transform.forward), CustomUtility.ToVector2(enemy.m_target.position - enemy.transform.position)) < m_fieldOfViewAngle;
            animator.SetBool("Alert", CustomUtility.IsHitRadius(enemy.m_target.position, enemy.transform.position, isInViewRange ? m_detectLongRange : m_detectRange));
        }
        
        //Debug.Log("Is the LM1's target nearby? " + animator.GetBool("Alert"));
    }

    public override void EnterState(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("Alert", false);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    public override void UpdateState(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //animator.SetBool("Walk", true);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    public override void ExitState(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
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
