using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************  
** Name: StateMachineBehaviour Base Enemy State
** Desc: An abstract class of StateMachineBehaviour for enemies
** Author: DahNoob
** Date: 18/12/2019, 3:03 PM
**************************
** Change History
**************************
** PR   Date                    Author    Description 
** --   --------                -------   ------------------------------------
** 1    18/12/2019, 3:03 PM     DahNoob   Created
*******************************/
[System.Serializable]
abstract public class SMB_BaseEnemyState : StateMachineBehaviour
{
    [Header("Base Enemy State Configuration")]
    [SerializeField]
    protected float m_checkIntervals = 1.5f;

    //Base variables
    protected EnemyBase enemy = null;
    protected float checkTimer = 0.0f;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        if (enemy == null)
            enemy = animator.GetComponent<EnemyBase>();
        checkTimer = 0;
        Enter(animator, animatorStateInfo, layerIndex);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        checkTimer += Time.deltaTime;
        Update(animator, animatorStateInfo, layerIndex);
        if(checkTimer > m_checkIntervals)
        {
            checkTimer -= m_checkIntervals;
            Check(animator, animatorStateInfo, layerIndex);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        Exit(animator, animatorStateInfo, layerIndex);
    }

    abstract public void Check(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex);//Logic check for setting it to the next state
    abstract public void Enter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex);//Function that is automatically called when the state is entered
    abstract public void Update(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex);//Function that is automatically called every update
    abstract public void Exit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex);//Function that is automatically called when the state is exiting
}
