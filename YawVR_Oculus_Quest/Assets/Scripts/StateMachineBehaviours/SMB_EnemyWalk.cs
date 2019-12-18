using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMB_EnemyWalk : StateMachineBehaviour
{
    [SerializeField]
    protected float m_detectRange = 50.0f;
    [SerializeField]
    protected float m_fieldOfViewAngle = 60.0f;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        
    }
}
