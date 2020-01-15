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
public class PlayerHandler : BaseEntity
{
    public static PlayerHandler instance { get; private set; }

    public enum STATE
    {
        IDLE,
        WALK
    }

    [Tooltip("The player's hands itself.")]
    [Header("Hands")]
    public GameObject rightHand;
    public GameObject leftHand;

    [Tooltip("The MonoBehaviour of the PilotControllers.")]
    [Header("Pilot Controllers")]
    [SerializeField]
    private PilotController m_rightController;
    [SerializeField]
    private PilotController m_leftController;

    [Tooltip("The ControllerFollowers.")]
    [Header("Controller Followers")]
    [SerializeField]
    private ControllerFollower m_rightFollower;
    [SerializeField]
    private ControllerFollower m_leftFollower;

    [Tooltip("The Mech hands.")]
    [Header("Mech Hands")]
    [SerializeField]
    private MechHandHandler m_rightMechHand;
    [SerializeField]
    private MechHandHandler m_leftMechHand;

    [Tooltip("legs bruh")]
    [Header("Mech Legs")]
    [SerializeField]
    private Animator m_mechLegs;

    [Header("UIs")]
    [SerializeField]
    private UnityEngine.UI.Slider m_energySlider;
    [SerializeField]
    private UnityEngine.UI.Image m_vignette;

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
    [SerializeField]
    [Range(0.0f,0.2f)]
    private float m_camSwayIntensity = 0.1f;

    [Header("Armor Configuration")]
    [SerializeField]
    private float m_maxArmor = 25.0f;
    [SerializeField]
    private float m_armorRegenRate = 10.0f;
    [SerializeField]
    private float m_armorRegenDelay = 3.0f;

    //Local variables
    private float armor;
    private Vector3 origPos;
    private Quaternion origRot;
    private bool isResettingPose = false;
    private Vector3 finalCamOffset;
    private Vector3 cameraShake = new Vector3();
    private float shakeElapsed = 0;
    private int shakeInterval = 0;
    private bool walkHapticReady = true;
    private bool isShaking = false;
    private float armorRegenElapsed = 0;

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
        m_vignette.color = Persistent.instance.COLOR_TRANSPARENT;
        m_camScreenFade.FadeIn();
        armor = m_maxArmor;
        origPos = transform.position;
        origRot = transform.rotation;
        currEnergy = m_maxEnergy;
        m_energySlider.maxValue = m_maxEnergy;
        rightHand.GetComponent<OVRGrabber>().QueryOffset += OnGrabberQueryOffset;
        leftHand.GetComponent<OVRGrabber>().QueryOffset += OnGrabberQueryOffset;
        print("PlayerHandler started!");
    }
    
    void Update()
    {
        if (transform.position.y < m_fallThreshold)
            ResetPose();
        if (!(m_leftController.IsModuleActivated() || m_rightController.IsModuleActivated()))
            currEnergy += m_energyRegenRate * Time.deltaTime;
        shakeElapsed -= Time.deltaTime;
        armorRegenElapsed += Time.deltaTime;
        if (armorRegenElapsed > m_armorRegenDelay)
        {
            armor = Mathf.Min(m_maxArmor, armor + m_armorRegenRate * Time.deltaTime);
        }
        m_energySlider.value = currEnergy;
        if (state == STATE.IDLE)
        {
            m_cameraOffset = Vector3.zero;
            m_mechLegs.SetFloat("Blend", 0);
            walkHapticReady = true;
        }
        else if(state == STATE.WALK)
        {
            float walkMultiplier = GetComponent<MechMovement>().movementAlpha;
            float time_mult = Time.time * 8;
            float sin = Mathf.Sin(time_mult * 2);
            if(walkHapticReady && sin < -0.8f)
            {
                walkHapticReady = false;
                float strength = 0.35f * walkMultiplier;
                VibrationManager.SetControllerVibration(OVRInput.Controller.RTouch, 0.04f, strength, false, 0.005f);
                VibrationManager.SetControllerVibration(OVRInput.Controller.LTouch, 0.04f, strength, false, 0.005f);
            }
            else if(!walkHapticReady && sin > 0.5f)
            {
                walkHapticReady = true;
            }
            m_cameraOffset = Vector3.LerpUnclamped(Vector3.zero, new Vector3(Mathf.Cos(time_mult) * m_camSwayIntensity, sin * m_camSwayIntensity, 0), walkMultiplier);
            m_mechLegs.SetFloat("Blend", walkMultiplier);
        }
        if (Input.GetKeyDown(KeyCode.G))
            Shake(0.2f);
    }

    private void FixedUpdate()
    {
        if (isShaking && ++shakeInterval % 2 == 0 && shakeElapsed > 0)
        {
            cameraShake = Vector3.LerpUnclamped(Vector3.zero, new Vector3(Random.Range(-0.15f, 0.15f), Random.Range(-0.15f, 0.15f)), shakeElapsed);
        }
        else if (isShaking && shakeElapsed < 0)
        {
            isShaking = false;
            cameraShake = Vector3.zero;
        }
        m_camPivot.localPosition = finalCamOffset = Vector3.LerpUnclamped(m_camPivot.localPosition, m_cameraOffset, 0.12f) + cameraShake;
        m_vignette.color = Color.LerpUnclamped(m_vignette.color, Persistent.instance.COLOR_TRANSPARENT, 0.05f);
        
        //rightHand.GetComponent<OVRGrabber>().SetAnchorOffsetPosition(-m_camPivot.localPosition);
        //leftHand.GetComponent<OVRGrabber>().SetAnchorOffsetPosition(-m_camPivot.localPosition);
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

    public PilotController GetRightPilotController() { return m_rightController; }
    public PilotController GetLeftPilotController() { return m_leftController; }
    public ControllerFollower GetRightFollower() { return m_rightFollower; }
    public ControllerFollower GetLeftFollower() { return m_leftFollower; }
    public MechHandHandler GetRightMechHand() { return m_rightMechHand; }
    public MechHandHandler GetLeftMechHand() { return m_leftMechHand; }

    public Color GetArmInnerColor()
    {
        return m_armInnerColor;
    }
    public Color GetArmRimColor()
    {
        return m_armRimColor;
    }
    public Vector3 GetCameraOffset()
    {
        return m_camPivot.localPosition;
    }
    public void SetState(PlayerHandler.STATE _newState)
    {
        state = _newState;
    }
    public void SetLegsAngle(float _x, float _y)
    {
        m_mechLegs.transform.localEulerAngles = new Vector3(0, Mathf.Atan2(-_y, _x) * Mathf.Rad2Deg + 90, 0);
    }
    public void Shake(float _duration)
    {
        isShaking = true;
        shakeElapsed = _duration;
    }
    public void OnGrabberQueryOffset(OVRGrabber _obj)
    {
        _obj.SetAnchorOffsetPosition(-finalCamOffset);
    }
    void OnDestroy()
    {
        rightHand.GetComponent<OVRGrabber>().QueryOffset -= OnGrabberQueryOffset;
        leftHand.GetComponent<OVRGrabber>().QueryOffset -= OnGrabberQueryOffset;

    }

    public override void takeDamage(int damage)
    {
        if (armor > 0)
        {
            armor = Mathf.Max(0, armor - damage);
            m_vignette.color = Color.cyan;
        }
        else
        {
            health -= damage;
            m_vignette.color = Color.red;
        }
        armorRegenElapsed = 0;
        float intensity = Mathf.Min(damage * 0.05f, 0.5f);
        Shake(intensity);
        VibrationManager.SetControllerVibration(OVRInput.Controller.RTouch, 0.08f, intensity);
        VibrationManager.SetControllerVibration(OVRInput.Controller.LTouch, 0.08f, intensity);
        if (health <= 0)
            Die();
    }

    public override void Die()
    {
        InvokeDie();
    }
}
