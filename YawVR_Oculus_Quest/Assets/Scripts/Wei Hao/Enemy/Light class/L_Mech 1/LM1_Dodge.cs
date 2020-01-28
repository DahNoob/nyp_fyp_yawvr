using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LM1_Dodge : SMB_BaseEnemyState
{
    [SerializeField]
    // Dodge Check
    private float dodgeCheck = 2.0f;
    [SerializeField]
    private float rotationSpeed;
    [SerializeField]
    private float moveSpeed;

    public override void CheckState(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {

    }

    public override void EnterState(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        animator.SetBool("Chase", false);
        animator.SetBool("Shoot", false);
        animator.GetComponent<Light_Enemy_1>().StartCoroutine(EnemyDodge(animator));
    }

    public override void UpdateState(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        
    }

    public override void ExitState(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {

    }

    IEnumerator EnemyDodge(Animator animator)
    {
        float random = Random.Range(1, 10);

        if (random > 5)
        {
            animator.transform.position += animator.transform.forward * /*moveSpeed*/ animator.GetComponent<Light_Enemy_1>().GetSpeed() * Time.deltaTime;
            //animator.transform.position += animator.transform.right * (moveSpeed * 1.5f) * Time.deltaTime;
        }
        else
        {
            animator.transform.position += animator.transform.forward * /*moveSpeed*/ animator.GetComponent<Light_Enemy_1>().GetSpeed() * Time.deltaTime;
            //animator.transform.position += animator.transform.right * (-moveSpeed * 1.5f) * Time.deltaTime;
        }

        yield return new WaitForSeconds(1.5f);
        animator.SetBool("DodgeEnd", true);
    }
}
