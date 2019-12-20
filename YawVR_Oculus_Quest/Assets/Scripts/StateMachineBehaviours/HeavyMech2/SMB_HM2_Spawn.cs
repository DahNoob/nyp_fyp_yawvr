using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/******************************  
** Name: StateMachineBehaviour HeavyMech2 Spawn
** Desc: The state of the heavy mech spawning light mech 2s'
** Author: DahNoob
** Date: 18/12/2019, 5:02 PM
**************************
** Change History
**************************
** PR   Date                    Author    Description 
** --   --------                -------   ------------------------------------
** 1    18/12/2019, 5:02 PM     DahNoob   Created
*******************************/
public class SMB_HM2_Spawn : SMB_BaseEnemyState
{
    [Header("HM2 Spawn State Configuration")]
    [SerializeField]
    protected int m_spawnIterations = 3;

    //Local variables
    protected int m_currentIteration = 0;

    public override void Check(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        if(m_currentIteration > m_spawnIterations)
        {
            animator.SetBool("Spawn_Done", true);
        }
        else
        {
            m_currentIteration += 1;
            enemy.GetComponent<HeavyMech2>().SpawnEnemy();
        }
        
    }

    public override void Enter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        m_currentIteration = 0;
        animator.SetBool("Spawn_Done", false);
        enemy.GetComponent<HeavyMech2>().EnterSpawn();
    }

    public override void Exit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        enemy.GetComponent<HeavyMech2>().ExitSpawn();
    }

    public override void Update(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
    }
}
