using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************  
** Name: Persistent Behaviour
** Desc: Makes it so objects with this behaviour will persist after scene changes, also acts as a singleton if needed
** Author: DahNoob
** Date: 27/11/2019, 5:05 PM
**************************
** Change History
**************************
** PR   Date                    Author    Description 
** --   --------                -------   ------------------------------------
** 1    27/11/2019, 5:05 PM     DahNoob   Created and implemented
*******************************/
[System.Serializable]
public class Persistent : MonoBehaviour
{
    public static Persistent instance { get; private set; }

    private void Awake()
    {
        DontDestroyOnLoad(this);
        if (instance == null)
            instance = this;
        print("Persistent awake!");
    }
    void Start()
    {
        print("Persistent started!");
    }
}
