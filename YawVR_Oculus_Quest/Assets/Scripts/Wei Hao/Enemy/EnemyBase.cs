using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************  
** Name: Enemy Stats
** Desc: To manage the stats for all enemies
** Author: Wei Hao
** Date: 6/12/2019, 9:15 AM
**************************
** Change History
**************************
** PR   Date                    Author    Description 
** --   --------                -------   ------------------------------------
** 1    27/11/2019, 5:05 PM     Wei Hao   Created and implemented
** 2    18/12/2019, 10:31 PM     Wei Hao   Added rarity enum
*******************************/

public class EnemyBase : MonoBehaviour
{
    // Enemy Current Health
    protected int health;
    // Enemy Max Health
    protected int maxHealth;
    // The amount of Damage the enemy deals
    protected int damage;
    // The speed the enemy moves
    protected int moveSpeed;

    public enum States
    {
        IDLE,
        CHASE,
        WAIT,
        ATTACK
    }

    public enum _Rarity
    {
        NORMAL,
        DELTA,
        BETA,
        OMEGA,
        ALPHA
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void takeDamage(int damage)
    {
        health = Mathf.Max(0, health - damage);
        if (health == 0)
            Die();
    }

    public void Die()
    {
        gameObject.SetActive(false);
        gameObject.GetComponent<ParticleSystem>().Play();
    }

    //public override void takeDamage(int damage)
    //{
    //    health = Mathf.Max(0, health - damage);
    //    if (health == 0)
    //        Die();
    //}

    //public override void Die()
    //{
    //}
}
