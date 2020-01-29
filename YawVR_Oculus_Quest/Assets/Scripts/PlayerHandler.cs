using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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
    private Slider m_healthBar;
    [SerializeField]
    private Slider m_armorBar;
    [SerializeField]
    private Image[] m_warningUI;
    [SerializeField]
    private UnityEngine.UI.Image m_vignette;
    [SerializeField]
    private float healthLerpSpeed = 5;
    [SerializeField]
    private float armorLerpSpeed = 5;
    [SerializeField]
    private float warningUIFadeSpeed = 5;

    [Header("Configuration")]
    [SerializeField]
    [ColorUsage(true,true)]
    private Color m_armInnerColor;
    [SerializeField]
    [ColorUsage(true, true)]
    private Color m_armRimColor;
    [SerializeField]
    private int m_maxHealth = 100;
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
    [SerializeField]
    [Range(0.0f, 0.2f)]
    private float m_damageShake = 0.08f;

    [Header("Armor Configuration")]
    [SerializeField]
    private float m_maxArmor = 25.0f;
    [SerializeField]
    private float m_armorRegenRate = 10.0f;
    [SerializeField]
    private float m_armorRegenDelay = 3.0f;

    [Header("Debugs")]
    public bool overrideControllers = false;

    //Local variables
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
    private bool startedRecharge = false;
    private float DEV_resetLevelTimer = 0;

    //Hidden variables
    private float _health, _armor;
    private float m_uiHealth, m_uiArmor;
    private float m_warningTimer;

    //Getters/Setters
    public new float health {
        get { return _health; }
        private set
        {
            _health = Mathf.Clamp(value, 0.0f, m_maxHealth);
            m_healthBar.value = _health;
        }
    }
    public float armor {
        get { return _armor; }
        private set
        {
            _armor = Mathf.Clamp(value, 0.0f, m_maxArmor);
            m_armorBar.value = _armor;
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
        health = m_maxHealth;
        origPos = transform.position;
        origRot = transform.rotation;
        m_healthBar.maxValue = m_maxHealth;
        m_armorBar.maxValue = m_maxArmor;
        rightHand.GetComponent<OVRGrabber>().QueryOffset += OnGrabberQueryOffset;
        leftHand.GetComponent<OVRGrabber>().QueryOffset += OnGrabberQueryOffset;
        m_uiHealth = health; //initial values
        m_uiArmor = armor; //initial values
        print("PlayerHandler started!");
    }
    
    void Update()
    {
        if (transform.position.y < m_fallThreshold)
            ResetPose();
        shakeElapsed -= Time.deltaTime;
        armorRegenElapsed += Time.deltaTime;
        if (armorRegenElapsed > m_armorRegenDelay)
        {
            armor = Mathf.Min(m_maxArmor, armor + m_armorRegenRate * Time.deltaTime);
            if(startedRecharge)
            {
                PlayerUIManager.instance.AddStringToProcessingQueue(new SystemFluffMessage("Recharging shields...", 0.1f, 0.05f));
                startedRecharge = false;
            }
        }
        else
        {
            startedRecharge = true;
        }


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
            if(walkHapticReady && sin < -0.85f)
            {
                walkHapticReady = false;
                float strength = 0.15f * walkMultiplier;
                VibrationManager.SetControllerVibration(OVRInput.Controller.RTouch, 0.03f, strength, false, 0.005f);
                VibrationManager.SetControllerVibration(OVRInput.Controller.LTouch, 0.03f, strength, false, 0.005f);
            }
            else if(!walkHapticReady && sin > 0.5f)
            {
                walkHapticReady = true;
            }
            m_cameraOffset = Vector3.LerpUnclamped(Vector3.zero, new Vector3(Mathf.Cos(time_mult) * m_camSwayIntensity * 0.25f, sin * m_camSwayIntensity, 0), walkMultiplier);
            m_mechLegs.SetFloat("Blend", walkMultiplier);
        }
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.G))
            Shake(0.2f);
#endif
        if(Input.GetKey(KeyCode.R) || (OVRInput.Get(OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.RTouch) && OVRInput.Get(OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.LTouch)))
        {
            DEV_resetLevelTimer += Time.deltaTime;
            if (DEV_resetLevelTimer > 2)
            {
                DEV_resetLevelTimer = -9999999;
                StartCoroutine(SetNextLevel("NewDesertMap"));
            }
        }
        else
        {
            DEV_resetLevelTimer = 0;
        }
        if(Input.GetKeyDown(KeyCode.M))
        {
            Instantiate(Persistent.instance.PREFAB_SUPPLYCRATE_DROP, transform.position + Vector3.up * -4.5f + transform.forward * 4, Quaternion.Euler(0, Random.Range(0, 360), 0), Persistent.instance.GO_STATIC.transform);
        }
        ////update ui
        //m_healthBar.value = Mathf.Lerp(m_uiHealth, _health, Time.deltaTime * healthLerpSpeed);
        //m_armorBar.value = Mathf.Lerp(m_uiArmor, _armor, Time.deltaTime * armorLerpSpeed);

        if(health < m_maxHealth * 0.5f)
        {
            m_warningTimer += Time.deltaTime * warningUIFadeSpeed;
            for (int i = 0; i < m_warningUI.Length; ++i)
            {
                Color color = m_warningUI[i].color;
                color.a = Mathf.PingPong(m_warningTimer, 1);
                m_warningUI[i].color = color;
            }
        }
        else
        {   
            for (int i = 0; i < m_warningUI.Length; ++i)
            {
                Color color = m_warningUI[i].color;
                color.a = Mathf.Lerp(color.a, 0, Time.deltaTime * warningUIFadeSpeed);
                m_warningUI[i].color = color;
            }
        }


    }

    private void FixedUpdate()
    {
        if (isShaking && ++shakeInterval % 2 == 0 && shakeElapsed > 0)
        {
            cameraShake = Vector3.LerpUnclamped(Vector3.zero, new Vector3(Random.Range(-m_damageShake, m_damageShake), Random.Range(-m_damageShake, m_damageShake)), shakeElapsed);
        }
        else if (isShaking && shakeElapsed < 0)
        {
            isShaking = false;
            cameraShake = Vector3.zero;
        }
        m_camPivot.localPosition = finalCamOffset = Vector3.LerpUnclamped(m_camPivot.localPosition, m_cameraOffset, 0.12f) + cameraShake;
        if (m_vignette.color.a > 0)
            m_vignette.color = Color.LerpUnclamped(m_vignette.color, Persistent.instance.COLOR_TRANSPARENT, 0.05f);
        
        //rightHand.GetComponent<OVRGrabber>().SetAnchorOffsetPosition(-m_camPivot.localPosition);
        //leftHand.GetComponent<OVRGrabber>().SetAnchorOffsetPosition(-m_camPivot.localPosition);
    }

    private void LateUpdate()
    {
        rightHand.GetComponent<OVRGrabber>().SetAnchorOffsetPosition(finalCamOffset);
        leftHand.GetComponent<OVRGrabber>().SetAnchorOffsetPosition(finalCamOffset);
    }

    //public GameObject GetNearestObjective()
    //{
    //}

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
        //Fix the thingy
        health = m_maxHealth;
        armor = m_maxArmor;
        isResettingPose = false;
    }

    public IEnumerator SetNextLevel(string _sceneName)
    {
        isResettingPose = true;
        m_camScreenFade.FadeOut();
        yield return new WaitForSeconds(m_camScreenFade.fadeTime + 0.1f);
        SceneManager.LoadScene(_sceneName);
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
            armor = armor - damage;
            m_vignette.color = Color.cyan;

            if (armor < m_maxArmor * 0.10f)
                PlayerUIManager.instance.AddStringToProcessingQueue(new SystemFluffMessage("Shields critical!", 0.1f, 0.05f));
            else if(armor <=0)
                PlayerUIManager.instance.AddStringToProcessingQueue(new SystemFluffMessage("Shields broken!", 0.1f, 0.05f));
        }
        else
        {
            health -= damage;
            m_vignette.color = Color.red;

            if (health < m_maxHealth * 0.10f)
                PlayerUIManager.instance.AddStringToProcessingQueue(new SystemFluffMessage("Hull strength critical!", 0.1f, 0.05f));
            else if (health <= 0)
                PlayerUIManager.instance.AddStringToProcessingQueue(new SystemFluffMessage("System failure!", 0.1f, 0.05f));
        }
        armorRegenElapsed = 0;
        float intensity = Mathf.Min(damage * 0.05f, 0.5f);
        Shake(intensity);
        VibrationManager.SetControllerVibration(OVRInput.Controller.RTouch, 0.08f, intensity);
        VibrationManager.SetControllerVibration(OVRInput.Controller.LTouch, 0.08f, intensity);
        if (health == 0)
            Die();
    }

    public override void Die()
    {
        health = m_maxHealth;
        InvokeDie();
        ResetPose();
    }

}
