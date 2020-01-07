using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/******************************  
** Name: StateMachineBehaviour Enemy Chase
** Desc: The state of the SMB enemy of chasing
** Author: DahNoob
** Date: 18/12/2019, 3:41 PM
**************************
** Change History
**************************
** PR   Date                    Author    Description 
** --   --------                -------   ------------------------------------
** 1    18/12/2019, 3:41 PM     DahNoob   Created
*******************************/
public class SMB_EnemyChase : SMB_BaseEnemyState
{
    [Header("Enemy Chase State Configuration")]
    [SerializeField]
    protected float m_inRange = 50.0f;
    [SerializeField]
    protected float m_outRange = 150.0f;

    protected float inRangeSqr, outRangeSqr;

    public override void Check(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        float distanceSqr = CustomUtility.HitCheckRadius(enemy.m_target.transform.position, enemy.transform.position);
        enemy.GetComponent<NavMeshAgent>().SetDestination(enemy.m_target.transform.position);
        if (distanceSqr < inRangeSqr)
            animator.SetBool("Chase_InRange", true);
        else if (distanceSqr > outRangeSqr)
            animator.SetBool("Chase_OutRange", true);
    }

    public override void Enter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        enemy.GetComponent<NavMeshAgent>().isStopped = false;
        animator.SetBool("Chase_InRange", false);
        animator.SetBool("Chase_OutRange", false);
        inRangeSqr = m_inRange * m_inRange;
        outRangeSqr = m_outRange * m_outRange;
        enemy.GetComponent<NavMeshAgent>().SetDestination(enemy.m_target.transform.position);
    }

    public override void Update(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        //relativePos = PlayerHandler.instance.transform.position - animator.transform.position;

        //Quaternion toRotation = Quaternion.LookRotation(new Vector3(relativePos.x, 0, relativePos.z));
        ////animator.transform.rotation = Quaternion.Slerp(animator.transform.rotation, toRotation, m_rotationSpeed * Time.deltaTime);

        ////animator.transform.position += animator.transform.forward * enemy.GetSpeed() * Time.deltaTime;
        ////enemy.GetComponent<NavMeshAgent>().Move(enemy.transform.forward * enemy.GetSpeed() * Time.deltaTime);

    }

    public override void Exit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        enemy.GetComponent<NavMeshAgent>().isStopped = true;
    }
}
