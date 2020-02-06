using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************  
** Name: StateMachineBehaviour Enemy Walk
** Desc: The state of the SMB enemy of walking
** Author: DahNoob
** Date: 18/12/2019, 2:17 PM
**************************
** Change History
**************************
** PR   Date                    Author    Description 
** --   --------                -------   ------------------------------------
** 1    18/12/2019, 2:17 PM     DahNoob   Created
** 2    18/12/2019, 3:16 PM     DahNoob   Implemented SMB enemybase as the base for SMB_EnemyWalk
*******************************/
public class SMB_EnemyWalk : SMB_BaseEnemyState
{
    [Header("Enemy Walk State Configuration")]
    [SerializeField]
    protected float m_detectRange = 50.0f;
    [SerializeField]
    protected float m_fieldOfViewAngle = 60.0f;
    [SerializeField]
    [Range(0, 100)]
    protected int m_walkChance = 20;

    public override void CheckState(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        if (Random.Range(0, 100) < m_walkChance)
        {
            QuadRect newQuadRect = new QuadRect(enemy.transform.position + Vector3.RotateTowards(Vector3.forward * 30, new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f)), 0.0f, 0.0f), 75, 150);
            enemy.navMeshAgent.SetDestination(MapPointsHandler.instance.GetClosestPoint(newQuadRect));
            Debug.Log(MapPointsHandler.instance.GetClosestPoint(newQuadRect));
        }
        if(CustomUtility.IsHitRadius(enemy.m_target.transform.position, enemy.transform.position, m_detectRange))
        {
            string[] layers = { "Terrain", "Player" };
            RaycastHit hit;
            Physics.Raycast(enemy.transform.position, (enemy.m_target.position - enemy.transform.position).normalized, out hit, 999999, LayerMask.GetMask(layers));
            if (hit.collider.transform == enemy.m_target)
                animator.SetBool("Walk_HasDetected", true);
        }
    }

    public override void EnterState(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        animator.SetBool("Walk_HasDetected", false);
    }

    public override void UpdateState(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {

    }

    public override void ExitState(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        
    }
}
