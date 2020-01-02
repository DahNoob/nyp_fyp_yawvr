using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LM2_Explode : SMB_BaseEnemyState
{
    private Transform Player;

    // Time taken before attack is activated
    private float attackWindUp = 1.0f;

    public override void Check(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {

    }

    public override void Enter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        animator.GetComponent<Rigidbody>().velocity = Vector3.zero;
        animator.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }

    public override void Update(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
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
            animator.gameObject.GetComponent<LightMech2>().PlayDeathParticle();
            animator.gameObject.SetActive(false);
        }
    }

    public override void Exit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
    }

}
