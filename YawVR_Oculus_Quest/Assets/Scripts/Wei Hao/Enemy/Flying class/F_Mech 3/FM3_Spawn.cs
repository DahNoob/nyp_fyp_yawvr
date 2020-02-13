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
    GameObject Cargo;

    protected float animationTime;

    public override void CheckState(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
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

    public override void EnterState(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        //m_currentIteration = 0;
        animator.SetBool("Chase_InRange", false);

        //Transform pTransform = GameObject.Find(animator.name).GetComponent<Transform>();
        //Cargo = pTransform.Find("Flying_Mech_Master/Container").gameObject;
        Cargo = enemy.GetComponent<FlyingMech3>().m_cargoObject;
        //Cargo.AddComponent<Rigidbody>();
        //rb = Cargo.gameObject.GetComponent<Rigidbody>();
        rb = Cargo.AddComponent<Rigidbody>();
        //rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        //foreach (Transform trs in pTransform)
        //{
        //    if (trs.gameObject.name == "Container")
        //    {
        //        trs.gameObject.AddComponent<Rigidbody>();
        //        rb = trs.gameObject.GetComponent<Rigidbody>();
        //        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        //        break;
        //    }
        //}

        //DropCargo(animator);
        //enemy.GetComponent<HeavyMech2>().EnterSpawn();
    }

    public override void UpdateState(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
    }

    public override void ExitState(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {

    }
    
    //private void DropCargo(Animator animator)
    //{
    //    animationTime -= 1.0f * Time.deltaTime;
    //    if(animationTime <= 0.0f)
    //    {
    //        Destroy(Cargo.gameObject);
    //    }
    //}
}
