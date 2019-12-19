using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LM1_Walk : SMB_BaseEnemyState
{
    [Header("Enemy Walk State Configuration")]
    [SerializeField]
    protected float m_detectRange = 70.0f;
    [SerializeField]
    protected float m_fieldOfViewAngle = 60.0f;
    

    public override void Check(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        PlayerHandler player = PlayerHandler.instance;
        animator.SetBool("Chase_InRange", CustomUtility.IsHitRadius(player.transform.position, enemy.transform.position, m_detectRange));
        Debug.Log("Is there a player nearby? " + animator.GetBool("Chase_InRange"));
    }

    public override void Enter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        animator.SetBool("Walk_HasDetected", false);
    }

    public override void Update(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        //checkTimer += Time.deltaTime;
        //Update(animator, animatorStateInfo, layerIndex);
        //if (checkTimer > m_checkIntervals)
        //{
        //    checkTimer -= m_checkIntervals;
        //    Check(animator, animatorStateInfo, layerIndex);
        //}
    }

    public override void Exit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        
    }
}
