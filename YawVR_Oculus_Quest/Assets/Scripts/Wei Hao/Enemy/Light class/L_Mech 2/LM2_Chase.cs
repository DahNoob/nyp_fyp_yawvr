using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LM2_Chase : StateMachineBehaviour
{
    private GameObject Player;
    private Transform PlayerTransform;
    private Vector3 relativePos;
    [SerializeField]
    private float rotationSpeed;
    [SerializeField]
    private float moveSpeed;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        relativePos = PlayerTransform.position - animator.transform.position;

        Quaternion toRotation = Quaternion.LookRotation(new Vector3(relativePos.x, 0, relativePos.z));
        animator.transform.rotation = Quaternion.Lerp(animator.transform.rotation, toRotation, rotationSpeed * Time.deltaTime);

        animator.transform.position += animator.transform.forward * moveSpeed * Time.deltaTime;
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
}
