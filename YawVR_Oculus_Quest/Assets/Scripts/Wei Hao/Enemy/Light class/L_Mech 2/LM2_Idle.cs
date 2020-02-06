using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LM2_Idle : SMB_EnemyWalk
{

    public override void CheckState(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        base.CheckState(animator, animatorStateInfo, layerIndex);
        //PlayerHandler player = PlayerHandler.instance;
        //animator.SetBool("Walk_HasDetected", CustomUtility.IsHitRadius(player.transform.position, enemy.transform.position, m_detectRange));
    }

    public override void EnterState(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //animator.SetBool("Chase", true);
        base.EnterState(animator, stateInfo, layerIndex);
    }

    public override void UpdateState(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        base.UpdateState(animator, animatorStateInfo, layerIndex);
    }

    public override void ExitState(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        base.ExitState(animator, animatorStateInfo, layerIndex);
        enemy.navMeshAgent.speed = 0.25f; //starts charging/revving up
        enemy.navMeshAgent.SetDestination(enemy.m_target.position);
    }
}
