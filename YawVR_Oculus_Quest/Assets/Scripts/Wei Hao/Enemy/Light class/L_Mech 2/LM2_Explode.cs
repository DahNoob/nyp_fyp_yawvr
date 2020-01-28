using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LM2_Explode : SMB_BaseEnemyState
{
    private Transform Player;

    [SerializeField]
    private float m_explodeRange = 15.0f;
    // Time taken before attack is activated
    private float attackWindUp = 1.0f;

    public override void CheckState(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {

    }

    public override void EnterState(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        Player = PlayerHandler.instance.transform;
        animator.GetComponent<Rigidbody>().velocity = Vector3.zero;
        animator.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }

    public override void UpdateState(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        attackWindUp -= 1.0f * Time.deltaTime;

        if (attackWindUp <= 0.0f)
        {
            Collider[] hitColliders = Physics.OverlapSphere(animator.transform.position, m_explodeRange);
            for (int i = 0; i < hitColliders.Length; i++)
            {
                //if (hitColliders[i].gameObject.tag == "Player")
                //{
                //    Debug.Log("B00M !!");
                //}
                var asd = hitColliders[i].GetComponent<BaseEntity>();
                if (hitColliders[i].transform == PlayerHandler.instance.transform)
                {
                    //Debug.Log((int)enemy.GetDamage());
                    PlayerHandler.instance.takeDamage((int)enemy.GetDamage());
                }
            }
            animator.GetComponent<IPooledObject>().OnObjectDestroy();
        }
    }

    public override void ExitState(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
    }

}
