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

    public readonly Color COLOR_TRANSPARENT = new Color(0, 0, 0, 0);
    public GameObject GO_STATIC { get; private set; }
    public GameObject GO_DYNAMIC { get; private set; }

    public GameObject PREFAB_MODULE_ICON;
    public GameObject PREFAB_BEACON;
    public GameObject PREFAB_CROWN;
    public Sprite MINIMAP_ICON_ENEMY;
    public Sprite MINIMAP_ICON_OBJECTIVE;
    public Material MAT_WHITE;
    public Material MAT_ENEMYMECH;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            instance.GO_STATIC = GameObject.Find("Static");
            instance.GO_DYNAMIC = GameObject.Find("Dynamic");
            return;
        }
        instance = this;
        DontDestroyOnLoad(this);
        GO_STATIC = GameObject.Find("Static");
        GO_DYNAMIC = GameObject.Find("Dynamic");
        print("Persistent awake!");
    }
    void Start()
    {
        
        print("Persistent started!");
    }
}
