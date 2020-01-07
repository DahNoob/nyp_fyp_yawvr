using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************  
** Name: StateMachineBehaviour Enemy Walk
** Desc: The state of the SMB enemy of walking
** Author: DahNoob
** Date: 18/12/2019, 2:17 PM
**************************
** Change History
**************************
** PR   Date                    Author    Description 
** --   --------                -------   ------------------------------------
** 1    18/12/2019, 2:17 PM     DahNoob   Created
** 2    18/12/2019, 3:16 PM     DahNoob   Implemented SMB enemybase as the base for SMB_EnemyWalk
*******************************/
public class SMB_EnemyWalk : SMB_BaseEnemyState
{
    [Header("Enemy Walk State Configuration")]
    [SerializeField]
    protected float m_detectRange = 50.0f;
    [SerializeField]
    protected float m_fieldOfViewAngle = 60.0f;

    public override void Check(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        animator.SetBool("Walk_HasDetected", CustomUtility.IsHitRadius(enemy.m_target.transform.position, enemy.transform.position, m_detectRange));
    }

    public override void Enter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        animator.SetBool("Walk_HasDetected", false);
    }

    public override void Update(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {

    }

    public override void Exit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        
    }
}
