using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************  
** Name: StateMachineBehaviour Enemy Start
** Desc: The state of the SMB enemy where it has just been spawned
** Author: DahNoob
** Date: 02/01/2020, 3:20 PM
**************************
** Change History
**************************
** PR   Date                    Author    Description 
** --   --------                -------   ------------------------------------
** 1    02/01/2020, 3:20 PM     DahNoob   Created
*******************************/
public class SMB_EnemyStart : SMB_BaseEnemyState
{
    public override void Check(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        animator.SetBool("Start_Ready", true);
    }

    public override void Enter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        enemy.GetComponent<UnityEngine.AI.NavMeshAgent>().updatePosition = false;
        animator.SetBool("Start_Ready", false);
    }

    public override void Update(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {

    }

    public override void Exit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        enemy.GetComponent<UnityEngine.AI.NavMeshAgent>().updatePosition = true;
    }
}
