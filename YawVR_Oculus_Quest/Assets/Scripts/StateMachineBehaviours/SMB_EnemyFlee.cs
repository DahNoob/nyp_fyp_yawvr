﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/******************************  
** Name: StateMachineBehaviour Enemy Flee
** Desc: The state of the SMB enemy of fleeing from player
** Author: DahNoob
** Date: 27/12/2019, 11:24 AM
**************************
** Change History
**************************
** PR   Date                    Author    Description 
** --   --------                -------   ------------------------------------
** 1    27/12/2019, 11:24 AM    DahNoob   Created
*******************************/
public class SMB_EnemyFlee : SMB_BaseEnemyState
{
    [Header("Enemy Flee State Configuration")]
    [SerializeField]
    protected float m_fleeRange = 75.0f;

    protected float fleeRangeSqr;

    public override void Check(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        float distanceSqr = CustomUtility.HitCheckRadius(PlayerHandler.instance.transform.position, enemy.transform.position);
        if (distanceSqr > fleeRangeSqr)
            animator.SetBool("Flee_Done", true);
        else
            CalculateFlee();
    }

    public override void Enter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        enemy.GetComponent<NavMeshAgent>().isStopped = false;
        fleeRangeSqr = m_fleeRange * m_fleeRange;
        animator.SetBool("Flee_Done", false);
        CalculateFlee();
    }

    public override void Update(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {

    }

    public override void Exit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        enemy.GetComponent<NavMeshAgent>().isStopped = true;
    }

    protected void CalculateFlee()
    {
        Vector3 playerToEnemyPos = PlayerHandler.instance.transform.position + (enemy.transform.position - PlayerHandler.instance.transform.position).normalized * m_fleeRange;
        enemy.GetComponent<NavMeshAgent>().SetDestination(MapPointsHandler.instance.GetClosestPoint(playerToEnemyPos));
    }
}
