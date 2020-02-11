using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LM1_Death : SMB_BaseEnemyState
{
    public override void CheckState(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
    }

    public override void EnterState(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
    }

    public override void ExitState(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        enemy.GetComponent<Light_Enemy_1>().ActualDie();
    }

    public override void UpdateState(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
    }
}
