using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class initializes player values
/// Inherits from PlayerBase
/// </summary>
public class Player : PlayerBase
{
    // Start is called before the first frame update
    void Start()
    {
        maxHealth = 100;
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetHealth()
    {
        return health;
    }
}
