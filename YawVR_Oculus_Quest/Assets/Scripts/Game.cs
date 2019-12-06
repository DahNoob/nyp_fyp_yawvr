using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Game : MonoBehaviour
{
    public static Game instance { get; private set; }

    [Header("Default Mech Loadouts")]
    [SerializeField]
    private MechArmModule[] m_rightArmModules;
    [SerializeField]
    private MechArmModule[] m_leftArmModules;

    void Start()
    {
        if (instance == null)
            instance = this;
        ApplyMechLoadouts();
    }

    public void ApplyMechLoadouts()
    {
        foreach (MechArmModule module in m_leftArmModules)
        {
            PlayerHandler.instance.GetLeftPilotController().AttachArmModule(module);//so crappy but wutever
        }
        foreach (MechArmModule module in m_rightArmModules)
        {
            PlayerHandler.instance.GetRightPilotController().AttachArmModule(module);//bruh moment
        }
    }
}
