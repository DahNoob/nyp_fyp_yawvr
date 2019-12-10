using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    // Enemy Current Health
    protected int health;
    // Enemy Max Health
    protected int maxHealth;
    // The amount of Damage the enemy deals
    protected int damage;
    // The speed the enemy moves
    protected int moveSpeed;

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
}
