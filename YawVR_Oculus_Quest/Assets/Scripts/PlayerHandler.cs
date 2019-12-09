using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVR;

/******************************  
** Name: Player Handler behaviour
** Desc: A sort of singleton behaviour that manages the player's data/logic (like energy)
** Author: DahNoob
** Date: forgot
**************************
** Change History
**************************
** PR   Date                    Author    Description 
** --   --------                -------   ------------------------------------
** 1    forgot                  DahNoob   Created
** 2    09/12/2019, 4:43PM      DahNoob   Added energy
*******************************/
[System.Serializable]
public class PlayerHandler : MonoBehaviour
{
    public static PlayerHandler instance { get; private set; }

    [Header("Hands")]
    public GameObject rightHand;
    public GameObject leftHand;

    [Header("Pilot Controllers")]
    [SerializeField]
    private PilotController m_rightController;
    [SerializeField]
    private PilotController m_leftController;

    [Header("UIs")]
    [SerializeField]
    private UnityEngine.UI.Slider m_energySlider;

    [Header("Configuration")]
    [SerializeField]
    private Color m_armInnerColor;
    [SerializeField]
    private Color m_armRimColor;
    [SerializeField]
    private float m_maxEnergy = 100.0f;
    [SerializeField]
    private float m_energyRegenRate = 8.0f;

    //Local variables
    private bool isEnergyDecreasing = false;

    //Hidden variables
    private float _energy;

    //Getters/Setters
    public float currEnergy {
        get { return _energy; }
        private set
        {
            _energy = Mathf.Clamp(value, 0.0f, m_maxEnergy);
        }
    }

    void Awake()
    {
        if (instance == null)
            instance = this;
        print("PlayerHandler awake!");
    }

    void Start()
    {
        currEnergy = m_maxEnergy;
        m_energySlider.maxValue = m_maxEnergy;
        print("PlayerHandler started!");
    }
    
    void Update()
    {
        if (isEnergyDecreasing)
            isEnergyDecreasing = false;
        else
            currEnergy += m_energyRegenRate * Time.deltaTime;
        m_energySlider.value = currEnergy;
    }

    public bool DecreaseEnergy(float _decrement)
    {
        if (currEnergy - _decrement > 0)
        {
            currEnergy -= _decrement;
            isEnergyDecreasing = true;
            return true;
        }
        return false;
    }

    public PilotController GetRightPilotController()
    {
        return m_rightController;
    }
    public PilotController GetLeftPilotController()
    {
        return m_leftController;
    }
    public Color GetArmInnerColor()
    {
        return m_armInnerColor;
    }
    public Color GetArmRimColor()
    {
        return m_armRimColor;
    }
}
