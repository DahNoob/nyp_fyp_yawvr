using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerUIManager : MonoBehaviour
{
    //Honestly liek idk what is this tbh liek if i have time i will get back to this but...
    public static PlayerUIManager instance;

    [Header("Player Minimap")]
    [SerializeField]
    [Tooltip("The camera rendering this minimap")]
    private Camera m_minimapCamera;
    [SerializeField]
    [Tooltip("The rect transform that holds the minimap")]
    private RectTransform m_minimapMask;
    //Minimap ranges
    [SerializeField]
    [Tooltip("Minimap Component")]
    private PlayerUIMinimap m_playerMinimap;

    [Header("Minimap Ranges")]
    public float m_customRange = 20;
    public float m_customRangeTwo = 0.1f;
    public float m_rejectionRange = 3;

    [Header("Minimap Trail Configuration")]
    [SerializeField]
    private PlayerUIMinimapTrail m_playerMinimapTrail;

    [Header("Objectives UI Configuration")]
    [SerializeField]
    private UIObjectiveHandler m_playerUIObjectives;

    [Header("Reticle UI Configuration")]
    [SerializeField]
    private UIReticleHandler m_reticleHandler;

    [Header("Weapons UI/Info Configuration")]
    [SerializeField]
    private GUIWeaponInfo m_weaponInfo;
    [SerializeField]
    private Image m_leftWeaponIcon;
    [SerializeField]
    private Image m_rightWeaponIcon;



    //Local variables
    [HideInInspector]
    //Normalized scale for the update of size between other things
    public float normalizedScale;

    public Camera minimapCamera
    {
        get
        {
            return m_minimapCamera;
        }
        private set
        {
            m_minimapCamera = value;
        }
    }

    //Get stuff
    public PlayerUIMinimap playerMinimap
    {
        get { return m_playerMinimap; }
        private set { }
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;

        m_playerUIObjectives.Awake();
        m_reticleHandler.Awake();
        //Load sounds so it can be used immediately
        //m_playerUISounds.Awake();
    }


    // Start is called before the first frame update
    void Start()
    {
        //Set rejection range
        m_rejectionRange = m_minimapMask.rect.width * 0.5f;

        m_playerMinimap.Start();
        m_playerMinimapTrail.Start();
        m_reticleHandler.Start();

        //foreach (SystemFluffMessage startingFluffs in m_startingSystemFluffs)
        //{
        //    AddStringToProcessingQueue(startingFluffs);
        //}

        //StartCoroutine(m_playerMinimap.UpdateMinimap());
    }

    // Update is called once per frame
    void Update()
    {
        m_playerMinimap.Update();
        m_playerMinimapTrail.Update();
        m_playerUIObjectives.Update();
        m_reticleHandler.Update();

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ObjectiveTriggered(1);
        }

        m_playerMinimap.m_minimapBounds.position = transform.position;
        //lerp the objective hex thing


        //if (m_processingQueue.Count > 0 && !isAlreadyTyping)
        //{
        //    AddStringToSystemQueue(m_processingQueue.Dequeue());
        //}

        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    AddStringToProcessingQueue(FormatFluff("Interesting, to say the least."));
        //}
    }

    public void ObjectiveTriggered(int objectiveIndex)
    {
        if (m_playerUIObjectives != null)
            m_playerUIObjectives.ObjectiveTriggered(objectiveIndex);
    }

    //Weapons stuff
    public void SetWeaponIconSprite(OVRInput.Controller m_controller, Sprite m_moduleIcon, MechGunWeapon.GUN_TYPE m_gunType)
    {
        Image resultantSprite = m_controller == OVRInput.Controller.RTouch ? m_rightWeaponIcon : m_leftWeaponIcon;
        resultantSprite.sprite = m_moduleIcon;

        RectTransform rectTransform = resultantSprite.GetComponent<RectTransform>();
        switch (m_gunType)
        {
            case MechGunWeapon.GUN_TYPE.SHOT_GUN:
                rectTransform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
                break;
            case MechGunWeapon.GUN_TYPE.TRIGATLING:
                rectTransform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
                break;
            case MechGunWeapon.GUN_TYPE.HANDCANNON:
                rectTransform.localScale = new Vector3(1, 0.6f, 0.6f);
                break;
            default:
                break;
        }
    }

    #region WeaponPanelUpdates
    public void SetWeaponInfo(OVRInput.Controller _controller,
        Sprite m_weaponSprite,
        string m_weaponName,
        float m_currWeaponAmmo,
        float m_maxWeaponAmmo,
        float normalized)
    {
        GUIWeaponInfoConfig weaponInfo = _controller == OVRInput.Controller.RTouch ? m_weaponInfo.rightWeaponInfo : m_weaponInfo.leftWeaponInfo;

        if (m_weaponSprite != null)
            weaponInfo.weaponSprite.sprite = m_weaponSprite;

        //weaponInfo.weaponNameText.text = m_weaponName;
        //Format the string
        //weaponInfo.weaponAmmoText.text = m_currWeaponAmmo.ToString() + "/" + m_maxWeaponAmmo.ToString();
        weaponInfo.weaponAmmoText.text = m_currWeaponAmmo.ToString();
        weaponInfo.weaponAmmoSlider.value = normalized;
    }

    public void SetWeaponInfoAmmo(OVRInput.Controller _controller, float m_currAmmo, float m_maxAmmo, float normalized)
    {
        GUIWeaponInfoConfig weaponInfo = _controller == OVRInput.Controller.RTouch ? m_weaponInfo.rightWeaponInfo : m_weaponInfo.leftWeaponInfo;
        //Format the string
        weaponInfo.weaponAmmoText.fontSize = 20;
        //weaponInfo.weaponAmmoText.text = m_currAmmo.ToString() + "/" + m_maxAmmo.ToString();
        weaponInfo.weaponAmmoText.text = m_currAmmo.ToString();
        weaponInfo.weaponAmmoSlider.value = normalized;
    }

    public void SetWeaponInfoReloading(OVRInput.Controller _controller)
    {
        GUIWeaponInfoConfig weaponInfo = _controller == OVRInput.Controller.RTouch ? m_weaponInfo.rightWeaponInfo : m_weaponInfo.leftWeaponInfo;
        weaponInfo.weaponAmmoText.fontSize = 9;
        StartCoroutine(StartReloadingTextAnimation(weaponInfo));
    }

    //Going to cheese it by adding two coroutines
    IEnumerator StartReloadingTextAnimation(GUIWeaponInfoConfig weaponInfo)
    {
        weaponInfo.weaponAmmoText.text = "";
        foreach (char letter in "Reloading...")
        {
            weaponInfo.weaponAmmoText.text += letter;
            yield return new WaitForSeconds(0.05f);
        }
    }
    #endregion
}

//[System.Serializable]
//public class SystemFluffMessage
//{
//    public string message;
//    public float delay;
//    public float messageSpeed;

//    public SystemFluffMessage(string _message, float _delay, float _messageSpeed)
//    {
//        message = _message;
//        delay = _delay;
//        messageSpeed = _messageSpeed;
//    }
//}


//[Header("Player HUD Configuration")]
////Will move to class later if there is time
//[SerializeField]
//[Tooltip("HUD Text")]
//UnityEngine.UI.Text m_systemFluffs;

//[SerializeField]
//[Tooltip("Starting Fluffs")]
//private List<SystemFluffMessage> m_startingSystemFluffs;

////Local variables
//Queue<SystemFluffMessage> m_systemQueue = new Queue<SystemFluffMessage>();
//Queue<SystemFluffMessage> m_processingQueue = new Queue<SystemFluffMessage>();
//static int MAX_SYSTEM_QUEUE_COUNT = 12;
//private int m_systemFluffCount = 0;
//private string m_previousSystemText;
//private bool isAlreadyTyping = false;

//public void AddStringToProcessingQueue(SystemFluffMessage fluffs)
//{
//    if (isAlreadyTyping)
//        m_processingQueue.Enqueue(fluffs);
//    else
//        AddStringToSystemQueue(fluffs);
//}

//bool AddStringToSystemQueue(SystemFluffMessage fluffs)
//{
//    fluffs.message = FormatFluff(fluffs.message);
//    if (m_systemFluffCount < MAX_SYSTEM_QUEUE_COUNT)
//    {
//        m_systemQueue.Enqueue(fluffs);
//        //Add one more to the thingy
//        StartCoroutine(TypeNewSystemLine(fluffs));
//        m_systemFluffCount++;
//    }
//    else
//    {
//        m_systemQueue.Dequeue();
//        AssignLastText();
//        StartCoroutine(TypeNewSystemLine(fluffs));
//        m_systemQueue.Enqueue(fluffs);
//    }

//    return true;
//}

//public string AssignLastText()
//{
//    if (m_systemQueue.Count == 0)
//        return "";

//    m_previousSystemText = "";
//    Queue<SystemFluffMessage> newQueue = new Queue<SystemFluffMessage>(m_systemQueue);
//    while (newQueue.Count > 0)
//    {
//        m_previousSystemText += newQueue.Dequeue().message;
//    }

//    m_systemFluffs.text = m_previousSystemText + "\n";

//    return m_previousSystemText;
//}

//IEnumerator TypeNewSystemLine(SystemFluffMessage fluff)
//{
//    isAlreadyTyping = true;

//    yield return new WaitForSeconds(fluff.delay);

//    foreach (char letter in fluff.message)
//    {
//        string previousText = "";
//        char[] systemFluffArray = m_systemFluffs.text.ToCharArray();

//        for (int i = 0; i < systemFluffArray.Length - 1; ++i)
//        {
//            previousText += systemFluffArray[i];
//        }
//        m_systemFluffs.text = previousText;

//        m_systemFluffs.text += letter + "|";
//        //m_systemFluffs.text += letter;
//        yield return new WaitForSeconds(fluff.messageSpeed);
//    }
//    isAlreadyTyping = false;
//}


//string FormatFluff(string message)
//{
//    return "-" + message + "\n";
//}