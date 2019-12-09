using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Roam : EnemyBase
{
    private GameObject Player;
    private int maxDistance = 10;
    private int minDistance = 5;
    private int attackRange = 3;
    private int rotationSpeed = 5;

    private float attackCooldown = 1.0f;
    private int leapSpeed;

    [SerializeField]
    private States currentState;

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

                Vector3 StoredLocation = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                //Debug.Log("Attack Timer: " + pauseTimer);
                if (attackCooldown <= 0.0f)
                {
                    transform.position += transform.up * moveSpeed * Time.deltaTime;
                    //transform.position += transform.forward * leapSpeed * Time.deltaTime;     
                }
                break;
            default:
                break;
        }

        Debug.Log("Current State is: " + currentState);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Mech")
        {
            attackCooldown = 1.0f;
        }
    }
}
