using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FM2_Idle : SMB_BaseEnemyState
{
    [Header("Enemy Walk State Configuration")]
    [SerializeField]
    protected float m_detectRange = 50.0f;
    [SerializeField]
    protected float m_fieldOfViewAngle = 60.0f;

    public override void CheckState(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        PlayerHandler player = PlayerHandler.instance;
        animator.SetBool("Chase_InRange", CustomUtility.IsHitRadius(player.transform.position, enemy.transform.position, m_detectRange));
    }

    public override void EnterState(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        animator.SetBool("Chase_InRange", false);
    }

    public override void UpdateState(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {

    }

    public override void ExitState(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        
    }
}
