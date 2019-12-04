using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************  
** Name: Persistent Behaviour
** Desc: Makes it so objects with this behaviour will persist after scene changes
** Author: DahNoob
** Date: 27/11/2019, 5:05 PM
**************************
** Change History
**************************
** PR   Date                    Author    Description 
** --   --------                -------   ------------------------------------
** 1    27/11/2019, 5:05 PM     DahNoob   Created and implemented
*******************************/
public class Persistent : MonoBehaviour
{
    private void Start()
    {
        DontDestroyOnLoad(this);
    }
}
