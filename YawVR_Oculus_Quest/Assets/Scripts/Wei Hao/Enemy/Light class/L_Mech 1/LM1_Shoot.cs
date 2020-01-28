﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LM1_Shoot : SMB_BaseEnemyState
{
    // Dodge Check
    [SerializeField]
    private float dodgeCheck = 2.0f;
    [SerializeField]
    protected float m_DodgeDetectRange = 10.0f;
    [SerializeField]
    protected float m_detectRange = 15.0f;


    
    [Header("LM1 Shoot State Configuration")]
    [SerializeField]
    protected NavMeshAgent navMeshAgent;

    // Time taken before attack is activated
    private float attackWindUp = 0.0f;

    private bool shoot = false;

    public override void CheckState(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        PlayerHandler player = PlayerHandler.instance;
        //animator.SetBool("Dodge", CustomUtility.IsHitRadius(player.transform.position, enemy.transform.position, m_DodgeDetectRange));
        //Debug.Log("Is it time to dodge? " + animator.GetBool("Dodge"));
        animator.SetBool("Shoot", CustomUtility.IsHitRadius(player.transform.position, enemy.transform.position, m_detectRange));
        //Debug.Log("Hit Check Radius: " + HitCheckRadius(player.transform.position, enemy.transform.position));
        Debug.Log("Is Player still near you? " + animator.GetBool("Shoot"));
    }

    public override void EnterState(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        //animator.SetBool("Shoot", false);
        //navMeshAgent.isStopped = true;
    }

    public override void UpdateState(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        Vector3 relativePos = enemy.m_target.position - animator.transform.position;

        Quaternion toRotation = Quaternion.LookRotation(new Vector3(relativePos.x, 0, relativePos.z));
        animator.transform.rotation = Quaternion.Lerp(animator.transform.rotation, toRotation, animator.GetComponent<Light_Enemy_1>().GetRotationSpeed() * Time.deltaTime);

        attackWindUp -= 1.0f * Time.deltaTime;
        if (attackWindUp <= 0.0f)
        {
            shoot = true;
            if (shoot)
            {
                animator.GetComponent<Light_Enemy_1>().Shoot_Async();//StartCoroutine(EnemyShoot(animator));
                attackWindUp = 2.0f;
                shoot = false;
            }
        }

        //dodgeCheck -= 1.0f * Time.deltaTime;
        //if (dodgeCheck <= 0.0f)
        //{
            //Check(animator, animatorStateInfo, layerIndex);
            //dodgeCheck = 2.0f;
            //Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, 5.0f);
            //for (int i = 0; i < hitColliders.Length; i++)
            //{
            //    if (hitColliders[i].gameObject.tag == "Bullet")
            //    {
            //        Debug.Log("Projectile detected");
            //        currentState = _EnemyState.AVOID;
            //    }
            //}
        //}

    }

    public override void ExitState(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        //navMeshAgent.isStopped = true;
    }

    //IEnumerator EnemyShoot(Animator animator)
    //{
    //    Light_Enemy_1 lm1 = enemy.GetComponent<Light_Enemy_1>();
    //    //Debug.Log("Fire start");
    //    BaseProjectile _projectileL = ObjectPooler.instance.SpawnFromPool(PoolObject.OBJECTTYPES.ENEMY_PROJECTILE, lm1.leftProjectileOrigin.position, Quaternion.identity).GetComponent<BaseProjectile>();
    //    lm1.leftProjectileOrigin.LookAt(enemy.m_target);
    //    _projectileL.Init(lm1.leftProjectileOrigin);

    //    yield return new WaitForSeconds(0.2f);

    //    //Debug.Log("2nd Fire start");
    //    BaseProjectile _projectileR = ObjectPooler.instance.SpawnFromPool(PoolObject.OBJECTTYPES.ENEMY_PROJECTILE, lm1.rightProjectileOrigin.position, Quaternion.identity).GetComponent<BaseProjectile>();
    //    lm1.rightProjectileOrigin.LookAt(enemy.m_target);
    //    _projectileR.Init(lm1.rightProjectileOrigin);
    //}

    //public static bool IsOutsideHitRadius(Vector3 _pos1, Vector3 _pos2, float _radius)
    //{
    //    return HitCheckRadius(_pos1, _pos2) > (_radius * _radius);
    //}

    //public static float HitCheckRadius(Vector3 _pos1, Vector3 _pos2)
    //{
    //    return (_pos1 - _pos2).sqrMagnitude;
    //}
}
