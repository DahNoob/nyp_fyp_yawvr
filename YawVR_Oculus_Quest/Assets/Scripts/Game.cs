﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Game : MonoBehaviour
{
    public static Game instance { get; private set; }
    
    [Header("Default Mech Loadouts")]
    [SerializeField]
    private GameObject[] m_rightArmModules;
    [SerializeField]
    private GameObject[] m_leftArmModules;
    [Header("Prefabs")]
    [SerializeField]
    private GameObject[] m_enemies;

    void Awake()
    {
        if (instance == null)
            instance = this;
        print("Game awake!");
    }

    void Start()
    {
        ApplyMechLoadouts();
        ApplyObjectives();
        print("Game started!");
    }

    public void ApplyMechLoadouts()
    {
        PlayerHandler.instance.GetLeftPilotController().AttachArmModules(m_leftArmModules);
        PlayerHandler.instance.GetRightPilotController().AttachArmModules(m_rightArmModules);
    }

    public void ApplyObjectives()
    {
        MapPointsHandler mph = MapPointsHandler.instance;
        foreach (var obj in mph.m_variedObjectives.objectives)
        {
            if(obj.type == VariedObjectives.TYPE.BOUNTYHUNT)
            {
                Instantiate(m_enemies[2], mph.m_mapPoints[obj.mapPointIndex], Quaternion.identity, Persistent.instance.GO_DYNAMIC.transform);
            }
        }
    }
}
