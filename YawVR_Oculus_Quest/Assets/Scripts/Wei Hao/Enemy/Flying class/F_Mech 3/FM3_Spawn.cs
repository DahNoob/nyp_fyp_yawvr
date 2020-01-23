using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FM3_Spawn : SMB_BaseEnemyState
{
    //[Header("FM3 Spawn State Configuration")]
    //[SerializeField]
    //protected int m_spawnIterations = 3;

    ////Local variables
    //protected int m_currentIteration = 0;

    private Rigidbody rb;

    protected float animationTime;

    public override void Check(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        //if (m_currentIteration > m_spawnIterations)
        //{
        //    animator.SetBool("Spawn_Finished", true);
        //}
        //else
        //{
        //    m_currentIteration += 1;
        //    enemy.GetComponent<HeavyMech2>().SpawnEnemy();
        //}
        
    }

    public override void Enter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        //m_currentIteration = 0;
        animator.SetBool("Chase_InRange", false);

        Transform pTransform = GameObject.Find(animator.name).GetComponent<Transform>();
        foreach (Transform trs in pTransform)
        {
            if (trs.gameObject.name == "Cargo")
            {
                trs.gameObject.AddComponent<Rigidbody>();
                rb = trs.gameObject.GetComponent<Rigidbody>();
                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                break;
            }
        }

        //StartCoroutine(DropCargo(animator));
        //enemy.GetComponent<HeavyMech2>().EnterSpawn();
        
    }

    public override void Update(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
    }

    public override void Exit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {

    }

    private IEnumerator DropCargo(Animator animator)
    {
        yield return new WaitForSeconds(animationTime);       
    }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
