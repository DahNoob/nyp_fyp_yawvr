using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FM1_Heal : SMB_BaseEnemyState
{
    public override void CheckState(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        
    }

    public override void EnterState(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        Transform parent = animator.transform.parent;
        parent.GetChild(1).gameObject.SetActive(true);
        //animator.transform.GetChild(0).gameObject.SetActive(true);
    }

    public override void UpdateState(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {

    }

    public override void ExitState(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        
    }
}
