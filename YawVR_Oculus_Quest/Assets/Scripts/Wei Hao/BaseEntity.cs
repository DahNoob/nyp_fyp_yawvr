using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************  
** Name: Entity Base
** Desc: 
** Author: Wei Hao
** Date: 6/12/2019, 9:15 AM
**************************
** Change History
**************************
** PR   Date                    Author    Description 
** --   --------                -------   ------------------------------------
** 1    27/11/2019, 5:05 PM     Wei Hao   
*******************************/

/// <summary>
/// Base entity class that objects inherit from.
/// </summary>
public abstract class BaseEntity : DynamicQuadTreeObject
{
    /// <summary>
    /// Delegate function for entity death.
    /// </summary>
    /// <param name="_entity">The base entity that has died.</param>
    public delegate void OnEntityDie(BaseEntity _entity);

    /// <summary>
    /// Called when an entity has died.
    /// </summary>
    public event OnEntityDie onEntityDie;

    protected int health;

    /// <summary>
    /// Take damage function
    /// </summary>
    /// <param name="damage">Amount of damage this entity would take</param>
    public abstract void takeDamage(int damage);

    /// <summary>
    /// Die function that is called when entity death [deprecated?]
    /// </summary>
    public abstract void Die();

    /// <summary>
    /// Invoke function for death.
    /// </summary>
    protected void InvokeDie()
    {
        onEntityDie?.Invoke(this);
    }

    /// <summary>
    /// Getter for health.
    /// </summary>
    /// <returns>Entity Health Value</returns>
    public int GetHealth()
    {
        return health;
    }
}
