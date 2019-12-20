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

    public enum STATE
    {
        IDLE,
        WALK
    }

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
    [ColorUsage(true,true)]
    private Color m_armInnerColor;
    [SerializeField]
    [ColorUsage(true, true)]
    private Color m_armRimColor;
    [SerializeField]
    private float m_maxEnergy = 100.0f;
    [SerializeField]
    private float m_energyRegenRate = 8.0f;
    [SerializeField]
    [Tooltip("The cut-off point to automatically activate the recenter pose (in world position y-coord)")]
    private int m_fallThreshold = -20;
    [SerializeField]
    private Transform m_camPivot;
    [SerializeField]
    private OVRScreenFade m_camScreenFade;
    [SerializeField]
    [Tooltip("Unable to be set, but serialized for debugging viewing purposes.")]
    private Vector3 m_cameraOffset;
    [SerializeField]
    private PlayerHandler.STATE state = STATE.IDLE;

    //Local variables
    private Vector3 origPos;
    private Quaternion origRot;
    private bool isResettingPose = false;

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
        m_camScreenFade.FadeIn();
        origPos = transform.position;
        origRot = transform.rotation;
        currEnergy = m_maxEnergy;
        m_energySlider.maxValue = m_maxEnergy;
        print("PlayerHandler started!");
    }
    
    void Update()
    {
        if (transform.position.y < m_fallThreshold)
            ResetPose();
        if (!(m_leftController.IsModuleActivated() || m_rightController.IsModuleActivated()))
            currEnergy += m_energyRegenRate * Time.deltaTime;
        m_energySlider.value = currEnergy;
        if(state == STATE.IDLE)
        {
            m_cameraOffset = Vector3.zero;
        }
        else if(state == STATE.WALK)
        {
            float time_mult = Time.time * 8;
            m_cameraOffset.Set(Mathf.Cos(time_mult) * 0.2f, Mathf.Sin(time_mult * 2) * 0.2f, 0);
        }
    }

    private void FixedUpdate()
    {
        m_camPivot.localPosition = Vector3.SlerpUnclamped(m_camPivot.localPosition, m_cameraOffset, 0.12f);
    }

    public bool DecreaseEnergy(float _decrement)
    {
        if (currEnergy - _decrement > 0)
        {
            currEnergy -= _decrement;
            return true;
        }
        return false;
    }

    public void ResetPose()
    {
        if(!isResettingPose)
        {
            StartCoroutine(ResetPoseThread());
        }
        //GetComponent<OVRScreenFade>().FadeIn();
        //GetComponent<CharacterController>().enabled = false;
        //transform.SetPositionAndRotation(origPos, origRot);
        //GetComponent<CharacterController>().enabled = true;
    }

    public IEnumerator ResetPoseThread()
    {
        isResettingPose = true;
        m_camScreenFade.FadeOut();
        yield return new WaitForSeconds(m_camScreenFade.fadeTime + 0.1f);
        GetComponent<CharacterController>().enabled = false;
        transform.SetPositionAndRotation(origPos, origRot);
        GetComponent<CharacterController>().enabled = true;
        m_camScreenFade.FadeIn();
        isResettingPose = false;
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
    public void SetState(PlayerHandler.STATE _newState)
    {
        state = _newState;
    }
}
