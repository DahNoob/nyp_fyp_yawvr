using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FM2_Cooldown : SMB_BaseEnemyState
{
    Vector3 currPos;
    private float timer = 1.0f;

    public override void Check(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        
    }

    public override void Enter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        animator.SetBool("Attack", false);
        currPos = new Vector3(animator.transform.position.x, animator.transform.position.y, animator.transform.position.z);
        timer = 1.0f;       
    }


    public override void Update(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        timer -= 1.0f * Time.deltaTime;
        if (timer > 0.0f)
        {
            //currPos -= new Vector3(0, 0, 0.5f);
            //animator.transform.position = currPos;
        }
        else
        {
            animator.SetBool("Chase", true);
            //animator.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, -1);
        }
    }

    public override void Exit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        //animator.SetBool("Chase", true);
        //animator.GetComponent<Rigidbody>().velocity = Vector3.zero;
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
