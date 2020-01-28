using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FM2_Attack : SMB_BaseEnemyState
{
    private bool StopTrack = false;
    private Vector3 playerLastPosition;
    private Vector3 relativePos;

    [Header("Configurations")]
    [SerializeField]
    private float m_trackTime = 3.0f;
    [SerializeField]
    private float m_attackWindUp = 1.0f;
    [SerializeField]
    private float rotationSpeed = 6.0f;
    [SerializeField]
    private float thrust = 200.0f;

    public override void CheckState(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {

    }

    public override void EnterState(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        animator.SetBool("Chase_InRange", false);
        m_trackTime = 3.0f;      
    }

    public override void UpdateState(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        m_trackTime -= 1.0f * Time.deltaTime;
        if (m_trackTime <= 0.0f)
        {
            StopTrack = true;
        }

        if (!StopTrack)
        {
            relativePos = PlayerHandler.instance.transform.position - animator.transform.position;

            Quaternion toRotation = Quaternion.LookRotation(new Vector3(relativePos.x, relativePos.y, relativePos.z));
            animator.transform.rotation = Quaternion.Lerp(animator.transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            animator.gameObject.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * thrust);
        }
    }

    public override void ExitState(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        StopTrack = false;
    }
}
