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
public abstract class BaseEntity : MonoBehaviour
{
    protected int health;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public abstract void takeDamage(int damage);

    public abstract void Die();
}
