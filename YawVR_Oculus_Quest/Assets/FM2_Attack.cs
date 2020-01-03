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
    private float m_trackTime = 4.0f;
    [SerializeField]
    private float m_attackWindUp = 1.0f;
    [SerializeField]
    private float rotationSpeed = 6.0f;

    public override void Check(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {

    }

    public override void Enter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        
    }

    public override void Update(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
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
            animator.gameObject.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, 0, 100));
        }
    }

    public override void Exit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        
    }
}
