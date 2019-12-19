using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LM2_Idle : SMB_EnemyWalk
{

    public override void Check(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        //PlayerHandler player = PlayerHandler.instance;
        //animator.SetBool("Walk_HasDetected", CustomUtility.IsHitRadius(player.transform.position, enemy.transform.position, m_detectRange));
    }

    public override void Enter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("Chase", true);
    }

    public override void Update(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {

    }

    public override void Exit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {

    }
}
