using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LM2_Idle : SMB_EnemyWalk
{

    public override void Check(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        base.Check(animator, animatorStateInfo, layerIndex);
        //PlayerHandler player = PlayerHandler.instance;
        //animator.SetBool("Walk_HasDetected", CustomUtility.IsHitRadius(player.transform.position, enemy.transform.position, m_detectRange));
    }

    public override void Enter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //animator.SetBool("Chase", true);
        base.Enter(animator, stateInfo, layerIndex);
    }

    public override void Update(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        base.Update(animator, animatorStateInfo, layerIndex);
    }

    public override void Exit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        base.Exit(animator, animatorStateInfo, layerIndex);
        enemy.GetComponent<UnityEngine.AI.NavMeshAgent>().speed = 0.25f; //starts charging/revving up
        enemy.GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(PlayerHandler.instance.transform.position);
    }
}
