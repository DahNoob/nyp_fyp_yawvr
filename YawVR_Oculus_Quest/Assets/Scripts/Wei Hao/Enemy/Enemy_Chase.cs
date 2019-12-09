using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************  
** Name: EnemyTest Script
** Desc: A test enemy that follows you?
** Author: Wei Hao
** Date: 6/12/2019, 12:30 AM
**************************
** Change History
**************************
** PR   Date                    Author    Description 
** --   --------                -------   ------------------------------------
** 1    6/12/2019, 12:50:00 PM  Wei Hao   Created and implemented
*******************************/
public class Enemy_Chase : EnemyBase
{
    private GameObject Player;
    private int maxDistance = 10;
    private int minDistance = 5;
    private int attackRange = 5;//3;
    private int rotationSpeed = 5;

    private float attackCooldown = 1.0f;
    private int leapSpeed;
    private bool readyToAttack = false;

    [SerializeField]
    private States currentState;

    public ParticleSystem poof;

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = 10;
        health = maxHealth;
        damage = 1;
        moveSpeed = 4;
        leapSpeed = 8;
        currentState = States.CHASE;
        Player = GameObject.Find("Player");
        
        Debug.Log("Current Health = " + health);
        Debug.Log("Current Max Health = " + maxHealth);
        Debug.Log("Current dmg = " + damage);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 relativePos = Player.transform.position - transform.position;       

        // Debugging distance
        float distance = Vector3.Distance(transform.position, Player.transform.position);

        if (Vector3.Distance(transform.position, Player.transform.position) >= attackRange)
        {
            currentState = States.CHASE;
            Debug.Log(distance);
            
            if (Vector3.Distance(transform.position, Player.transform.position) <= attackRange + 1)
            {
                currentState = States.ATTACK;
            }
        }

        switch (currentState)
        {
            case States.CHASE:
                Quaternion toRotation = Quaternion.LookRotation(new Vector3(relativePos.x, 0, relativePos.z));
                transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);

                transform.position += transform.forward * moveSpeed * Time.deltaTime;
                break;
            case States.ATTACK:
                //transform.LookAt(new Vector3(Player.transform.position.x, 0, Player.transform.position.z));
                attackCooldown -= 1.0f * Time.deltaTime;

                //Vector3 StoredLocation = new Vector3(transform.position.x,transform.position.y,transform.position.z);
                Debug.Log("Attack Timer: " + attackCooldown);
                if (attackCooldown <= 0.0f)
                {
                    readyToAttack = true;
                    transform.LookAt(new Vector3(Player.transform.position.x, 0, Player.transform.position.z));

                    transform.position += transform.forward * leapSpeed * Time.deltaTime;
                    //transform.position += transform.forward * leapSpeed * Time.deltaTime;     
                }
                break;
            case States.WAIT:
                break;
            default:
                break;
        }

        Debug.Log("Current State is: " + currentState);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Mech" && readyToAttack == true)
        {
            currentState = States.WAIT;
            attackCooldown = 1.0f;
            readyToAttack = false;
        }

        if(collision.gameObject.tag == "Bullet")
        {
            takeDamage(1);
        }
    }
}
