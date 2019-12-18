using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LM1_Chase : StateMachineBehaviour
{
    public Transform projectile;
    private Transform PlayerTransform;
    private Vector3 relativePos;

    private Transform m_projectileOriginL;
    private Transform m_projectileOriginR;

    [SerializeField]
    private float rotationSpeed;
    [SerializeField]
    private float moveSpeed;

    // Time taken before attack is activated
    private float attackWindUp = 0.0f;

    private bool shoot = false;

    [SerializeField]
    // Dodge Check
    private float dodgeCheck = 2.0f;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        m_projectileOriginL = animator.transform.Find("LBlaster Projectile Origin");
        m_projectileOriginR = animator.transform.Find("RBlaster Projectile Origin");
        //m_projectileOriginR
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        relativePos = PlayerTransform.position - animator.transform.position;

        Quaternion toRotation = Quaternion.LookRotation(new Vector3(relativePos.x, 0, relativePos.z));
        animator.transform.rotation = Quaternion.Lerp(animator.transform.rotation, toRotation, /*rotationSpeed */ animator.GetComponent<Light_Enemy_1>().GetRotationSpeed() * Time.deltaTime);

        animator.transform.position += animator.transform.forward * /*moveSpeed*/ animator.GetComponent<Light_Enemy_1>().GetMoveSpeed() * Time.deltaTime;

        attackWindUp -= 1.0f * Time.deltaTime;
        if (attackWindUp <= 0.0f)
        {
            shoot = true;
            if (shoot)
            {
                animator.GetComponent<Light_Enemy_1>().StartCoroutine(EnemyShoot(animator));
                attackWindUp = 2.0f;
                shoot = false;
            }
        }

        dodgeCheck -= 1.0f * Time.deltaTime;
        if (dodgeCheck <= 0.0f)
        {
            Collider[] hitColliders = Physics.OverlapSphere(animator.transform.position, 5.0f);
            for (int i = 0; i < hitColliders.Length; i++)
            {
                if (hitColliders[i].gameObject.tag == "Bullet")
                {
                    Debug.Log("Projectile detected");
                    animator.SetBool("Dodge", true);
                    animator.SetBool("Chase", false);
                }
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    IEnumerator EnemyShoot(Animator animator)
    {
        //Debug.Log("Fire start");
        BaseProjectile _projectileL = Instantiate(projectile, animator.transform.position + (PlayerTransform.position - animator.transform.position).normalized, Quaternion.LookRotation(PlayerTransform.position - animator.transform.position), Persistent.instance.GO_DYNAMIC.transform).GetComponent<BaseProjectile>();
        _projectileL.Init(m_projectileOriginL);

        yield return new WaitForSeconds(0.2f);

        //Debug.Log("2nd Fire start");
        BaseProjectile _projectileR = Instantiate(projectile, animator.transform.position + (PlayerTransform.position - animator.transform.position).normalized, Quaternion.LookRotation(PlayerTransform.position - animator.transform.position), Persistent.instance.GO_DYNAMIC.transform).GetComponent<BaseProjectile>();
        _projectileR.Init(m_projectileOriginR);
    }

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
