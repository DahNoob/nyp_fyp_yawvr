using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LM2_Chase : SMB_BaseEnemyState
{
    private GameObject Player;
    //private Transform PlayerTransform;
    private Vector3 relativePos;
    [SerializeField]
    private float rotationSpeed;
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    protected float m_detectRange = 10.0f;

    [HideInInspector]
    public NavMeshAgent navMeshAgent;

    public override void Check(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        //animator.SetBool("Explode", CustomUtility.IsHitRadius(enemy.transform.position, enemy.m_target.transform.position, 5.0f));
        Collider[] hitColliders = Physics.OverlapSphere(animator.transform.position, m_detectRange);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            //if (hitColliders[i].gameObject.tag == "Player")
            //{
            //    animator.SetBool("Explode", true);
            //}
            if(hitColliders[i].transform == enemy.m_target)
            {
                animator.SetBool("Explode", true);
                break;
            }
        }
        navMeshAgent.SetDestination(enemy.m_target.transform.position);
    }

    public override void Enter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        //PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        navMeshAgent = animator.GetComponent<NavMeshAgent>();
        animator.SetBool("Chase", false);
        navMeshAgent.isStopped = false;
        navMeshAgent.speed = enemy.GetSpeed();
    }

    public override void Update(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {

        //relativePos = PlayerTransform.position - animator.transform.position;

        //Quaternion toRotation = Quaternion.LookRotation(new Vector3(relativePos.x, 0, relativePos.z));
        //animator.transform.rotation = Quaternion.Lerp(animator.transform.rotation, toRotation, rotationSpeed * Time.deltaTime);

        //animator.transform.position += animator.transform.forward * moveSpeed * Time.deltaTime;
    }

    public override void Exit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {

    }
}
