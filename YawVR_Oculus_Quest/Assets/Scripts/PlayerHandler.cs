using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
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
/// <summary>
/// This class serves as a form of easy access for all objects to access player's values at any given time.
/// </summary>
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
    private TextMeshProUGUI m_coinsText;
    [SerializeField]
    private Camera m_minimapCamera;
    [SerializeField]
    private float healthLerpSpeed = 5;
    [SerializeField]
    private float armorLerpSpeed = 5;
    [SerializeField]
    private float warningUIFadeSpeed = 5;

    [Header("Configuration")]
    [SerializeField]
    [ColorUsage(true, true)]
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
    [Range(0.0f, 0.2f)]
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

    //Public variables
    public float goalPitch { private get; set; } = 0;
    public float currentPitch { private set; get; } = 0;

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
    private float prevHeight;
    private MechMovement mechMovement;

    //Hidden variables
    private float _health, _armor;
    private int _currency;
    private float m_uiHealth, m_uiArmor;
    private float m_warningTimer;

    //For sounds? Idk maybe better way later on
    bool isPlayHealthSound = false;

    /// <summary>
    /// Returns player's health
    /// </summary>
    public new float health
    {
        get { return _health; }
        private set
        {
            _health = Mathf.Clamp(value, 0.0f, m_maxHealth);
            m_healthBar.value = _health;
        }
    }
    /// <summary>
    /// Returns player's armor
    /// </summary>
    public float armor
    {
        get { return _armor; }
        private set
        {
            _armor = Mathf.Clamp(value, 0.0f, m_maxArmor);
            m_armorBar.value = _armor;
        }
    }
    /// <summary>
    /// Returns player's currency value
    /// </summary>
    public int currency
    {
        get { return _currency; }
        private set
        {
            _currency = value;
            m_coinsText.SetText(_currency.ToString());
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
        mechMovement = GetComponent<MechMovement>();
        m_vignette.color = Persistent.instance.COLOR_TRANSPARENT;
        m_camScreenFade.FadeIn();
        armor = m_maxArmor;
        health = m_maxHealth;
        origPos = transform.position;
        origRot = transform.rotation;
        m_healthBar.maxValue = m_maxHealth;
        m_armorBar.maxValue = m_maxArmor;
        //rightHand.GetComponent<OVRGrabber>().QueryOffset += OnGrabberQueryOffset;
        //leftHand.GetComponent<OVRGrabber>().QueryOffset += OnGrabberQueryOffset;
        m_uiHealth = health; //initial values
        m_uiArmor = armor; //initial values
        m_coinsText.text = "0";
        prevHeight = transform.position.y;
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
            if (startedRecharge)
            {
                //PlayerUIManager.instance.AddStringToProcessingQueue(new SystemFluffMessage("Recharging shields...", 0.1f, 0.05f));
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
        else if (state == STATE.WALK)
        {
            float walkMultiplier = mechMovement.movementAlpha;
            float time_mult = mechMovement.startWalkTime + Time.time * 5.6f;
            float sin = Mathf.Sin(time_mult * 2);
            if (walkHapticReady && sin < -0.45f)
            {
                walkHapticReady = false;
                float strength = 0.65f * walkMultiplier;
                VibrationManager.SetControllerVibration(OVRInput.Controller.RTouch, 0.03f, strength, false, 0.02f);
                VibrationManager.SetControllerVibration(OVRInput.Controller.LTouch, 0.03f, strength, false, 0.02f);
                mechMovement.PlayStepSound();
            }
            else if (!walkHapticReady && sin > 0.5f)
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
        if (Input.GetKey(KeyCode.R) || (OVRInput.Get(OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.RTouch) && OVRInput.Get(OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.LTouch)))
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
        if (Input.GetKeyDown(KeyCode.M))
        {
            Instantiate(Persistent.instance.PREFAB_SUPPLYCRATE_DROP, transform.position + Vector3.up * -4.5f + transform.forward * 4, Quaternion.Euler(0, Random.Range(0, 360), 0), Persistent.instance.GO_STATIC.transform);
        }
        ////update ui
        //m_healthBar.value = Mathf.Lerp(m_uiHealth, _health, Time.deltaTime * healthLerpSpeed);
        //m_armorBar.value = Mathf.Lerp(m_uiArmor, _armor, Time.deltaTime * armorLerpSpeed);

        if (health < m_maxHealth * 0.5f && armor <= 0f)
        {
            m_warningTimer += Time.deltaTime * warningUIFadeSpeed;
            for (int i = 0; i < m_warningUI.Length; ++i)
            {
                Color color = m_warningUI[i].color;
                color.a = Mathf.PingPong(m_warningTimer, 1);
                m_warningUI[i].color = color;
            }

            if (!isPlayHealthSound)
            {
                isPlayHealthSound = true;
                PlayerUISoundManager.instance.PlaySound(PlayerUISoundManager.UI_SOUNDTYPE.LOW_HEALTH);
            }
        }
        else
        {
            for (int i = 0; i < m_warningUI.Length; ++i)
            {
                Color color = m_warningUI[i].color;
                color.a = Mathf.Lerp(color.a, 0, Time.deltaTime * warningUIFadeSpeed * 2);
                m_warningUI[i].color = color;
            }

            //else play sound = false
            isPlayHealthSound = false;
        }
        if(Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            ExitToHub(false);
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
        m_camPivot.localPosition = finalCamOffset = Vector3.LerpUnclamped(m_camPivot.localPosition, m_cameraOffset + new Vector3(0, prevHeight - transform.position.y, 0) * 0.15f, 0.12f) + cameraShake;
        //transform.localEulerAngles = new Vector3(currentPitch, transform.localEulerAngles.y, 0);
        if (m_vignette.color.a > 0)
            m_vignette.color = Color.LerpUnclamped(m_vignette.color, Persistent.instance.COLOR_TRANSPARENT, 0.05f);
        prevHeight = transform.position.y;
        currentPitch = Mathf.LerpUnclamped(currentPitch, goalPitch, 0.1f);
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

    /// <summary>
    /// Resets the view and rotation of the player for centering.
    /// </summary>
    public void ResetPose()
    {
        if (!isResettingPose)
        {
            StartCoroutine(ResetPoseThread());
        }
        //GetComponent<OVRScreenFade>().FadeIn();
        //GetComponent<CharacterController>().enabled = false;
        //transform.SetPositionAndRotation(origPos, origRot);
        //GetComponent<CharacterController>().enabled = true;
    }

    /// <summary>
    /// Adds currency to the player's currency values.
    /// </summary>
    /// <param name="_amount">Amount to add to player's currency values.</param>
    public void AddCurrency(int _amount)
    {
        currency += _amount;
    }

    /// <summary>
    /// Adds currency to the player's health value.
    /// </summary>
    /// <param name="_amount">Amount to add to health currency values.</param>
    public void AddHealth(int _amount)
    {
        health += _armor;
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

    /// <summary>
    /// Function to handle scene fading and setting of next scene
    /// </summary>
    /// <param name="_sceneName">Name of scene to change to.</param>
    /// <returns></returns>
    public IEnumerator SetNextLevel(string _sceneName)
    {
        Game.instance.StopAllBGM();
        isResettingPose = true;
        m_camScreenFade.FadeOut();
        yield return new WaitForSeconds(m_camScreenFade.fadeTime + 0.1f);
        SceneManager.LoadScene(_sceneName);
    }

    /// <summary>
    /// Returns the player's right pilot controller
    /// </summary>
    /// <returns>PilotController</returns>
    public PilotController GetRightPilotController() { return m_rightController; }
    /// <summary>
    /// Returns the player's left pilot controller
    /// </summary>
    /// <returns>PilotController</returns>
    public PilotController GetLeftPilotController() { return m_leftController; }
    /// <summary>
    /// Returns the player's right follower
    /// </summary>
    /// <returns>ControllerFollower</returns>
    public ControllerFollower GetRightFollower() { return m_rightFollower; }
    /// <summary>
    /// Returns the player's left follower
    /// </summary>
    /// <returns>ControllerFollower</returns>
    public ControllerFollower GetLeftFollower() { return m_leftFollower; }
    /// <summary>
    /// Returns the player's right mech hand
    /// </summary>
    /// <returns>MechHandHandler</returns>
    public MechHandHandler GetRightMechHand() { return m_rightMechHand; }
    /// <summary>
    /// Returns the player's left mech hand
    /// </summary>
    /// <returns>MechHandHandler</returns>
    public MechHandHandler GetLeftMechHand() { return m_leftMechHand; }

    /// <summary>
    /// Returns the arm inner color
    /// </summary>
    /// <returns>Color</returns>
    public Color GetArmInnerColor()
    {
        return m_armInnerColor;
    }
    /// <summary>
    /// Returns the arm rim color
    /// </summary>
    /// <returns>Color</returns>
    public Color GetArmRimColor()
    {
        return m_armRimColor;
    }

    /// <summary>
    /// Returns the offset of player's camera.
    /// </summary>
    /// <returns>Camera's localPosition</returns>
    public Vector3 GetCameraOffset()
    {
        return m_camPivot.localPosition;
    }
    
    /// <summary>
    /// Sets state of the player
    /// </summary>
    /// <param name="_newState">State to set as new state.</param>
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
        //rightHand.GetComponent<OVRGrabber>().QueryOffset -= OnGrabberQueryOffset;
        //leftHand.GetComponent<OVRGrabber>().QueryOffset -= OnGrabberQueryOffset;

    }

    /// <summary>
    /// Override function for the player to take damage
    /// </summary>
    /// <param name="damage">Amount of damage to take</param>
    public override void takeDamage(int damage)
    {
        if (armor > 0)
        {
            armor = armor - damage;
            m_vignette.color = Color.cyan;

            //if (armor < m_maxArmor * 0.10f)
            //    PlayerUIManager.instance.AddStringToProcessingQueue(new SystemFluffMessage("Shields critical!", 0.1f, 0.05f));
            //else if(armor <=0)
            //    PlayerUIManager.instance.AddStringToProcessingQueue(new SystemFluffMessage("Shields broken!", 0.1f, 0.05f));
        }
        else
        {
            health -= damage;
            m_vignette.color = Color.red;

            //if (health < m_maxHealth * 0.10f)
            //    PlayerUIManager.instance.AddStringToProcessingQueue(new SystemFluffMessage("Hull strength critical!", 0.1f, 0.05f));
            //else if (health <= 0)
            //    PlayerUIManager.instance.AddStringToProcessingQueue(new SystemFluffMessage("System failure!", 0.1f, 0.05f));
        }
        armorRegenElapsed = 0;
        float intensity = Mathf.Min(damage * 0.05f, 0.5f);
        Shake(intensity);
        VibrationManager.SetControllerVibration(OVRInput.Controller.RTouch, 0.08f, intensity);
        VibrationManager.SetControllerVibration(OVRInput.Controller.LTouch, 0.08f, intensity);
        if (health == 0)
            Die();
    }

    /// <summary>
    /// Override function for when the player dies
    /// </summary>
    public override void Die()
    {
        health = m_maxHealth;
        InvokeDie();
        ResetPose();

    }

    /// <summary>
    /// Function to exit back to MainHubScene
    /// </summary>
    /// <param name="_allObjectivesCleared">Is all objectives cleared?</param>
    public void ExitToHub(bool _allObjectivesCleared)
    {
        if (_allObjectivesCleared)
        {
            PlayerPrefs.SetInt("Currency", PlayerPrefs.GetInt("Currency", 0) + currency);
            PlayerPrefs.Save();
        }
        StartCoroutine(SetNextLevel("MainHub"));
    }

    /// <summary>
    /// Triggers all objectives to be cleared.
    /// </summary>
    public void TriggerAllObjectivesCleared()
    {
        GetComponent<PlayerUIManager>().TriggerAllObjectivesCleared();
        GetComponent<PlayerUISoundManager>().PlayAllObjectivesClearedSound();
    }
}
