using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LM1_Alert : SMB_BaseEnemyState
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void Check(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }

    public override void Enter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy.GetComponent<Light_Enemy_1>().m_alertParticleSystem.Emit(1);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    public override void Update(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    public override void Exit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }
}
