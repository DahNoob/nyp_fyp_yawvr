using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LM2_Charge : SMB_BaseEnemyState
{
    public override void CheckState(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
    }

    public override void EnterState(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        enemy.GetComponent<LightMech2>().m_swirlLinesEffect.Play();
    }

    public override void UpdateState(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
    }

    public override void ExitState(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
    }
}
