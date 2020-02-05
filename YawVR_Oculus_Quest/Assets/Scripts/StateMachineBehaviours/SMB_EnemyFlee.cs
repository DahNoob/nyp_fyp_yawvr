using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/******************************  
** Name: StateMachineBehaviour Enemy Flee
** Desc: The state of the SMB enemy of fleeing from player
** Author: DahNoob
** Date: 27/12/2019, 11:24 AM
**************************
** Change History
**************************
** PR   Date                    Author    Description 
** --   --------                -------   ------------------------------------
** 1    27/12/2019, 11:24 AM    DahNoob   Created
*******************************/
public class SMB_EnemyFlee : SMB_BaseEnemyState
{
    [Header("Enemy Flee State Configuration")]
    [SerializeField]
    protected float m_fleeDetectRange = 150.0f;
    [SerializeField]
    protected float m_fleeMovementRange = 75.0f;

    protected float fleeDetectRSqr;

    public override void CheckState(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        float distanceSqr = CustomUtility.HitCheckRadius(PlayerHandler.instance.transform.position, enemy.transform.position);
        if (distanceSqr > fleeDetectRSqr)
            animator.SetBool("Flee_Done", true);
        else
            CalculateFlee();
    }

    public override void EnterState(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        enemy.GetComponent<NavMeshAgent>().isStopped = false;
        fleeDetectRSqr = m_fleeDetectRange * m_fleeDetectRange;
        animator.SetBool("Flee_Done", false);
        CalculateFlee();
        enemy.m_walkSound.Play();
    }

    public override void UpdateState(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {

    }

    public override void ExitState(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        enemy.GetComponent<NavMeshAgent>().isStopped = true;
        enemy.m_walkSound.Stop();
    }

    protected void CalculateFlee()
    {
        Vector3 playerToEnemyPos = PlayerHandler.instance.transform.position + (enemy.transform.position - PlayerHandler.instance.transform.position).normalized * m_fleeMovementRange;

        //Use Quadtree stuff with a box radius of 75
        QuadRect newQuadRect = new QuadRect(playerToEnemyPos, 75, 75);

        enemy.GetComponent<NavMeshAgent>().SetDestination(MapPointsHandler.instance.GetClosestPoint(newQuadRect));

        //Debug.Log("Result 1: "  + MapPointsHandler.instance.GetClosestPoint(playerToEnemyPos)
        //                + "| Result 2: " + MapPointsHandler.instance.GetClosestPoint(newQuadRect));
    }
}
