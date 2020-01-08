using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Roam : EnemyBase
{
    private GameObject Player;
    private int maxDistance = 10;
    private int minDistance = 5;
    private int chaseRange = 25;
    private int attackRange = 5;//3;
    private int rotationSpeed = 5;

    private float attackCooldown = 2.0f;
    private float attackWindUp = 2.0f;
    private int leapSpeed;
    private bool readyToAttack = true;
    private Vector3 player_LastPosition;
    private Vector3 _targetDestination;
    private Quaternion desiredRotation;
    private Vector3 _direction;

    private Rigidbody rb;

    private States currentState;

    public ParticleSystem poof;

    // Start is called before the first frame update
    void Start()
    {
        //maxHealth = 10;
        //health = maxHealth;
        //damage = 1;
        //moveSpeed = 4;
        leapSpeed = 8;
        currentState = States.IDLE;
        Player = GameObject.Find("Player");
        rb = gameObject.GetComponent<Rigidbody>();

        //Debug.Log("Current Health = " + health);
        //Debug.Log("Current Max Health = " + maxHealth);
        //Debug.Log("Current dmg = " + damage);
    }

    public void PlayerLastPosition()
    {
        player_LastPosition = Player.transform.position;
    }

    public Vector3 LastPosition()
    {
        return player_LastPosition;
    }
    

    // Update is called once per frame
    void Update()
    {
        Vector3 relativePos = Player.transform.position - transform.position;

        // Debugging distance
        float distance = Vector3.Distance(transform.position, Player.transform.position);

        if (currentState != States.WAIT && Vector3.Distance(transform.position, Player.transform.position) <= chaseRange)
        {
            currentState = States.CHASE;
            //Debug.Log(distance);

            if (Vector3.Distance(transform.position, Player.transform.position) <= attackRange + 1)
            {
                PlayerLastPosition();
                //player_LastPosition = Player.transform.position;
                currentState = States.ATTACK;
            }
        }

        switch (currentState)
        {
            case States.IDLE:
                Vector3 newPosition = (transform.position + (transform.forward * GetSpeed())) + new Vector3(Random.Range(-4.5f, 4.5f), 0.0f, Random.Range(-4.5f, 4.5f));

                _targetDestination = new Vector3(newPosition.x, 0, newPosition.z);
                _direction = Vector3.Normalize(_targetDestination - transform.position);
                _direction = new Vector3(_direction.x, 0.0f, _direction.z);
                desiredRotation = Quaternion.LookRotation(_direction);

                break;
            case States.CHASE:
                Quaternion toRotation = Quaternion.LookRotation(new Vector3(relativePos.x, 0, relativePos.z));
                transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);

                transform.position += transform.forward * GetSpeed() * Time.deltaTime;
                break;
            case States.ATTACK:

                //// Store player last position when target current state changed to ATTACK
                //player_LastPosition = Player.transform.position;
                //new Vector3(player_LastPosition.x, 0, player_LastPosition.z)
                transform.LookAt(LastPosition());

                // Homing charge
                //transform.LookAt(new Vector3(Player.transform.position.x, 0, Player.transform.position.z));

                attackWindUp -= 1.0f * Time.deltaTime;

                //Debug.Log("Attack Timer: " + attackCooldown);
                if (attackWindUp <= 0.0f)
                {
                    transform.position += transform.forward * leapSpeed * Time.deltaTime;   
                }
                break;
            case States.WAIT:
                transform.LookAt(new Vector3(Player.transform.position.x, 0, Player.transform.position.z));

                attackCooldown -= 1.0f * Time.deltaTime;
                if (attackCooldown <= 0.0f)
                {
                    attackCooldown = 2.0f;
                    attackWindUp = 2.0f;
                    readyToAttack = true;
                    currentState = States.CHASE;
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
            currentState = States.WAIT;
        }

        if (collision.gameObject.tag == "Bullet")
        {
            takeDamage(1);
            collision.gameObject.SetActive(false);
        }
    }
}
