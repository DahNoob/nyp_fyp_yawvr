using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LM2_Explode : StateMachineBehaviour
{
    private Transform Player;

    // Time taken before attack is activated
    private float attackWindUp = 2.0f;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        animator.GetComponent<Rigidbody>().velocity = Vector3.zero;
        animator.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        attackWindUp -= 1.0f * Time.deltaTime;

        if (attackWindUp <= 0.0f)
        {
            Collider[] hitColliders = Physics.OverlapSphere(animator.transform.position, 5.0f);
            for (int i = 0; i < hitColliders.Length; i++)
            {
                if (hitColliders[i].gameObject.tag == "Player")
                {
                    Debug.Log("B00M !!");
                }
            }
            //PlayDeathParticle();
            animator.gameObject.SetActive(false);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
