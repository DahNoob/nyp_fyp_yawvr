using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GUIManager : MonoBehaviour
{
    //[Header("Configurations")]
    //[SerializeField]
    //[Range(0.0f, 1.0f)]
    //private float m_uiRotationAlpha = 0.5f;

    public static GUIManager instance { private set; get; }

    [Header("Configuration")]
    [SerializeField]
    private GUIReticleModule reticleModule;
    [SerializeField]
    private RawImage m_minimap;
    [SerializeField]
    private RectTransform m_objectiveArrow;

    [Header("Resources")]
    [SerializeField]
    private Transform m_cameraTransform;
    [SerializeField]
    private GameObject m_objectiveArrowPrefab;
    [SerializeField]
    private GameObject m_objectiveTextPanelPrefab;

    [Header("Experimental Resources")]
    [SerializeField]
    private GameObject m_dumbCubes;
    [SerializeField]
    private GameObject m_heavyMech2Prefab;
    [SerializeField]
    private GameObject m_lightMech1Prefab;

    [Header("Debug Resources")]
    [SerializeField]
    private UnityEngine.UI.Text m_fpsValue;
    [SerializeField]
    private UnityEngine.UI.Text m_armRotationValue;

    [Header("Weapon Info Resources")]
    [SerializeField]
    private GUIWeaponInfo m_weaponInfo;

    [Header("UI Panels")]
    [SerializeField]
    private RectTransform m_objectiveListPanel;
    [SerializeField]
    private ObjectivesGUIInfo[] m_objectivesGUI;
    [SerializeField]
    private Sprite lockedObjectiveImage;
    [SerializeField]
    private Sprite failedObjectiveImage;
    [SerializeField]
    private Sprite successObjectiveImage;
    [SerializeField]
    private Sprite baseObjectiveImage;

    [Header("Weapon Icon")]
    [SerializeField]
    private Image m_leftWeaponIcon;
    [SerializeField]
    private Image m_rightWeaponIcon;

    //Local variables
    int frameCount = 0;
    float dt = 0.0f;
    float fps = 0.0f;
    float updateRate = 4.0f;  // 4 updates per sec.
    private ObjectiveInfo activeObjective;

    void Awake()
    {
        if (instance == null)
            instance = this;
        print("GUIManager awake!");
    }

    void Start()
    {
        Game.instance.onObjectiveStarted += Game_onObjectiveStarted;
        Game.instance.onObjectiveFinished += Game_onObjectiveFinished;
        print("GUIManager events +attached+ successfully!");
        //transform.position = m_cameraTransform.position;
        //transform.eulerAngles = new Vector3(0, m_cameraTransform.eulerAngles.y, 0);
        //transform.Rotate(Vector3.up, m_cameraTransform.rotation.y);
        reticleModule.SetupReticleModule();
        reticleModule.SetupReticleColors();

        //Disable both reticles
        EnableReticle(OVRInput.Controller.LTouch, false);
        EnableReticle(OVRInput.Controller.RTouch, false);

        foreach (Transform obj in m_objectiveListPanel)
        {
            Destroy(obj.gameObject);
        }
    }

    void Update()
    {
        frameCount++;
        dt += Time.deltaTime;
        if (dt > 1.0 / updateRate)
        {
            fps = frameCount / dt;
            frameCount = 0;
            dt -= 1.0f / updateRate;
        }
        //RaycastHit hit;
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //Debug.DrawRay(ray.origin, ray.direction * 1000);
        //LayerMask projectileMask = ~LayerMask.GetMask("PlayerProjectile");
        //if (Physics.Raycast(ray.origin, ray.direction, out hit, 300, projectileMask))
        //{
        //    SetReticleInformation(OVRInput.Controller.LTouch, hit.point, hit.collider.gameObject, true);
        //}

        //if (Input.GetMouseButtonDown(0))
        //{
        //    Triggered(OVRInput.Controller.LTouch);
        //}

        reticleModule.UpdateEase();

        //for (int i = 0; i < objectiveArrow.Length; ++i)
        //{
        //    if(objectiveArrow[i].gameObject.activeInHierarchy)
        //    {
        //        Vector3 displacement = Vector3.Scale(Game.instance.m_objectives[i].m_highlight.position - PlayerHandler.instance.transform.position, new Vector3(1, 0, 1));
        //        Vector3 bap = displacement.normalized;
        //        float lol = Mathf.Atan2(bap.x, bap.z) * Mathf.Rad2Deg;
        //        objectiveArrow[i].localPosition = Vector3.zero;
        //        objectiveArrow[i].eulerAngles = new Vector3(0, lol, 0);
        //        objectiveArrow[i].Rotate(90, 0, 0);
        //        objectiveArrow[i].Translate(0, Mathf.Min(0.14f, displacement.sqrMagnitude * 0.00005f), 0);
        //    }
        //}
        if (m_objectiveArrow.gameObject.activeInHierarchy)
        {
            if (activeObjective == null)
                m_objectiveArrow.gameObject.SetActive(false);
            else
            {
                if (activeObjective.m_highlight != null)
                {
                    Vector3 displacement = Vector3.Scale(activeObjective.m_highlight.position - PlayerHandler.instance.transform.position, new Vector3(1, 0, 1));
                    Vector3 bap = displacement.normalized;
                    float dist = displacement.magnitude;
                    float lol = Mathf.Atan2(bap.x, -bap.z) * Mathf.Rad2Deg;
                    m_objectiveArrow.localPosition = Vector3.zero;
                    m_objectiveArrow.localEulerAngles = new Vector3(0, 0, lol + PlayerHandler.instance.transform.eulerAngles.y + 180);
                    //m_objectiveArrow.Rotate(90, 0, 0);
                    m_objectiveArrow.Translate(0, Mathf.Min(0.14f, displacement.sqrMagnitude * 0.00005f), 0);

                    string additionalInfo = activeObjective.m_inProgress ? "Time left : " + ((int)activeObjective.m_timeLeft) : "Proximity : " + ((int)dist);
                    if (activeObjective.type == VariedObjectives.TYPE.BOUNTYHUNT)//hhhhhhhhhhhhh this is such garbage code fajfjofifaoopzpovzxda
                    {
                        activeObjective.panelInfo.panelText.text = string.Format("Bounty hunt\n{0}", additionalInfo);
                    }
                    else if (activeObjective.type == VariedObjectives.TYPE.DEFEND_STRUCTURE)
                    {
                        activeObjective.panelInfo.panelText.text = string.Format("Defend structure\n{0}", additionalInfo);
                    }
                }
            }
        }
    }

    void LateUpdate()
    {
        //transform.position = m_cameraTransform.position;
        //transform.eulerAngles = new Vector3(0, m_cameraTransform.eulerAngles.y, 0);
        m_fpsValue.text = fps.ToString();
        m_armRotationValue.text = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch).eulerAngles.ToString();
        //transform.Rotate(Vector3.up, m_cameraTransform.rotation.y);

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.L))
        {
            SpawnLightMech1();
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            SpawnHeavyMech2();
        }
#endif
    }

    public void SpawnCubes()
    {
        Instantiate(m_dumbCubes).transform.SetParent(GameObject.Find("CubePile").transform);
    }
    public void ClearCubes()
    {
        foreach (Transform item in GameObject.Find("CubePile").transform)
        {
            Destroy(item.gameObject);
        }
    }
    public void SpawnLightMech1()
    {
        //Instantiate(m_lightMech1Prefab, PlayerHandler.instance.transform.position + PlayerHandler.instance.transform.forward * 5 + Vector3.up * 3, PlayerHandler.instance.transform.rotation, Persistent.instance.GO_DYNAMIC.transform);
        ObjectPooler.instance.SpawnFromPool(PoolObject.OBJECTTYPES.LIGHT_MECH1, PlayerHandler.instance.transform.position + PlayerHandler.instance.transform.forward * 10 + Vector3.up * 3, PlayerHandler.instance.transform.rotation);
    }
    public void SpawnHeavyMech2()
    {
        //Instantiate(m_heavyMech2Prefab, PlayerHandler.instance.transform.position + PlayerHandler.instance.transform.forward * 10 + Vector3.up * 3, PlayerHandler.instance.transform.rotation, Persistent.instance.GO_DYNAMIC.transform);
        ObjectPooler.instance.SpawnFromPool(PoolObject.OBJECTTYPES.HEAVY_MECH2, PlayerHandler.instance.transform.position + PlayerHandler.instance.transform.forward * 10 + Vector3.up * 3, PlayerHandler.instance.transform.rotation);
    }
    public void RecenterPose()
    {
        OVRManager.display.RecenterPose();
        PlayerHandler.instance.ResetPose();
    }
    public void SetActiveObjective(ObjectiveInfo _objectiveInfo = null)
    {
        if (_objectiveInfo != null)
            m_objectiveArrow.gameObject.SetActive(true);

        if (_objectiveInfo != null && !_objectiveInfo.m_completed)
        {
            if (_objectiveInfo.type == VariedObjectives.TYPE.BOUNTYHUNT)//srsly fuckin garbage
            {
                _objectiveInfo.panelInfo.panelText.text = "Bounty Hunt";
            }
            else if (_objectiveInfo.type == VariedObjectives.TYPE.DEFEND_STRUCTURE)
            {
                _objectiveInfo.panelInfo.panelText.text = "Defend structure";
            }

            //Set the appropriate sprites
            //Remove lock icon from base
            _objectiveInfo.panelInfo.firstFill.sprite = baseObjectiveImage;
            _objectiveInfo.panelInfo.objectiveResult.gameObject.SetActive(false);
            _objectiveInfo.panelInfo.secondFill.gameObject.SetActive(true);
            _objectiveInfo.panelInfo.panelProgress.gameObject.SetActive(true);
        }

        activeObjective = _objectiveInfo;
    }
    public void AddObjectiveToPanel(ref ObjectiveInfo _objectiveInfo, int index)
    {
        //Text panel = Instantiate(m_objectiveTextPanelPrefab, m_objectiveListPanel.transform).GetComponent<Text>();

        _objectiveInfo.panelInfo = m_objectivesGUI[index];
        if (_objectiveInfo.type == VariedObjectives.TYPE.BOUNTYHUNT)//hhhhhhhhhhhhh this is such garbage code fajfjofifaoopzpovzxda
        {
            m_objectivesGUI[index].panelText.text = "Bounty Hunt";
        }
        else if (_objectiveInfo.type == VariedObjectives.TYPE.DEFEND_STRUCTURE)
        {
            m_objectivesGUI[index].panelText.text = "Defend structure";
        }

    }

    public void UpdateObjectiveProgress(ref ObjectiveInfo _objectiveInfo)
    {
        switch (_objectiveInfo.type)
        {
            case VariedObjectives.TYPE.BOUNTYHUNT:
                {
                    //m_objectiveTexts[index].backBlack.gameObject.SetActive(false);

                    //m_objectiveTexts[index].panelSecondFill.fillAmount = 1;
                    ////Set to 100%
                    //m_objectiveTexts[index].panelText.text = "100%";
                    break;
                }
            case VariedObjectives.TYPE.DEFEND_STRUCTURE:
                {
                    float fillAmount = 1 - (_objectiveInfo.m_timeLeft / _objectiveInfo.m_initialTime);
                    int fillAmountInt = (int)(fillAmount * 100);
                    _objectiveInfo.panelInfo.secondFill.fillAmount = fillAmount;
                    //Set to 100%
                    _objectiveInfo.panelInfo.panelProgress.text = fillAmountInt.ToString() + "%";
                    break;
                }
            case VariedObjectives.TYPE.TOTAL:
                break;
            default:
                break;
        }
    }

    public void FailedObjectiveGUI(ref ObjectiveInfo _objectiveInfo)
    {
        //Set the back black and the failed thing
        _objectiveInfo.panelInfo.firstFill.sprite = baseObjectiveImage;
        _objectiveInfo.panelInfo.objectiveResult.gameObject.SetActive(true);
        _objectiveInfo.panelInfo.objectiveResult.sprite = failedObjectiveImage;

        //Disable the panel stuff
        _objectiveInfo.panelInfo.secondFill.gameObject.SetActive(false);
        _objectiveInfo.panelInfo.panelProgress.gameObject.SetActive(false);

        //Play sound that failed
        PlayerUISoundManager.instance.PlaySound(PlayerUISoundManager.UI_SOUNDTYPE.OBJECTIVE_FAILED);
        //m_objectivesGUI[index].panelFirstFill.color = Color.red;
        //m_objectivesGUI[index].panelSecondFill.color = Color.red;
        ////Set to 100%
        //m_objectivesGUI[index].panelProgress.color = Color.red;

    }

    public void SucceededObjectiveGUI(ref ObjectiveInfo _objectiveInfo)
    {
        //Set the back black and the failed thing
        _objectiveInfo.panelInfo.firstFill.sprite = baseObjectiveImage;
        _objectiveInfo.panelInfo.objectiveResult.gameObject.SetActive(true);
        _objectiveInfo.panelInfo.objectiveResult.sprite = successObjectiveImage;

        _objectiveInfo.panelInfo.secondFill.gameObject.SetActive(false);
        _objectiveInfo.panelInfo.panelProgress.gameObject.SetActive(false);

        //Play sound that suceed
        PlayerUISoundManager.instance.PlaySound(PlayerUISoundManager.UI_SOUNDTYPE.OBJECTIVE_SUCCESS);
    }


    #region Reticles

    public void SetReticleInformation(OVRInput.Controller _controller, Vector3 hitPoint, GameObject hitObject, bool useTag = true)
    {
        //Set the reticle position based on raycasted position
        SetReticlePosition(_controller, hitPoint);
        //Set hit object name by accessing the object
        //SetHitObjectName(_controller, hitObject);
        //Set the hit color by accessing the layer of the object
        SetReticleColor(_controller, hitObject, useTag);
        //Scale the reticle to be correct scale
        ScaleReticle(_controller);

        if (hitObject != null)
        {
            GUIReticleConfig reticleConfig = _controller == OVRInput.Controller.RTouch ? reticleModule.RightReticle : reticleModule.LeftReticle;
            //Temporary code
            reticleConfig.ObjectOfInterest(hitObject.tag == "Enemy");
        }
    }

    public void SetReticlePosition(OVRInput.Controller _controller, Vector3 _worldPosition)
    {
        //Get config I suppose.
        GUIReticleConfig reticleConfig = _controller == OVRInput.Controller.RTouch ? reticleModule.RightReticle : reticleModule.LeftReticle;

        reticleConfig.reticleReference.transform.position = _worldPosition;
        reticleConfig.reticleReference.transform.LookAt(Camera.main.transform);
        reticleConfig.reticleReference.transform.Rotate(0, 180, 0);

        ScaleReticle(_controller);
    }

    public void SetHitObjectName(OVRInput.Controller _controller, GameObject hitObject)
    {
        GUIReticleConfig reticleConfig = _controller == OVRInput.Controller.RTouch ? reticleModule.RightReticle : reticleModule.LeftReticle;

        reticleConfig.reticleText.text = hitObject == null ? "N/A" : hitObject.name;
    }

    public void SetReticleColor(OVRInput.Controller _controller, GameObject hitObject, bool useTag)
    {
        //Access the reticle
        GUIReticleConfig reticleConfig = _controller == OVRInput.Controller.RTouch ? reticleModule.RightReticle : reticleModule.LeftReticle;

        if (hitObject == null)
        {
            reticleConfig.SetReticleDefaultColor();
            return;
        }

        //Access the color config
        GUIReticleColorConfig colorConfig = reticleModule.ReticleColors;

        //If the existing result is contained
        bool containsResult = useTag ? colorConfig.ContainsTag(hitObject.tag) : colorConfig.ContainsLayer(hitObject.layer);

        if (containsResult)
        {
            //If the result is contained
            Color resultantColor = useTag ? colorConfig.QueryTagColor(hitObject.tag) : colorConfig.QueryLayerColor(hitObject.layer);
            reticleConfig.SetReticleColor(resultantColor);
        }
        else
        {
            reticleConfig.SetReticleDefaultColor();
        }
    }

    //Scaling the reticle to make its same size no matter what
    public void ScaleReticle(OVRInput.Controller _controller)
    {
        //Get config I suppose.
        GUIReticleConfig reticleConfig = _controller == OVRInput.Controller.RTouch ? reticleModule.RightReticle : reticleModule.LeftReticle;

        //Distance between camera and the reticle
        float pos = Vector3.Distance(Camera.main.transform.position, reticleConfig.reticleReference.transform.position);

        //Some scaling formula from my other fill stuff
        float h = Mathf.Tan(Camera.main.fieldOfView * Mathf.Deg2Rad * 0.5f) * pos * 2f;
        reticleConfig.reticleReference.transform.localScale = new Vector3(h, h, h) * (reticleConfig.reticleSize * 0.01f);
    }

    public void Triggered(OVRInput.Controller _controller)
    {
        //Get config I suppose.
        GUIReticleConfig reticleConfig = _controller == OVRInput.Controller.RTouch ? reticleModule.RightReticle : reticleModule.LeftReticle;
        //reticleConfig.Triggered();
    }

    public void EnableReticle(OVRInput.Controller _controller, bool enabled)
    {
        //Get config I suppose.
        GUIReticleConfig reticleConfig = _controller == OVRInput.Controller.RTouch ? reticleModule.RightReticle : reticleModule.LeftReticle;
        reticleConfig.reticleReference.SetActive(enabled);
    }


    #endregion


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

    private void Game_onObjectiveStarted(ObjectiveInfo _objectiveInfo)
    {

    }

    private void Game_onObjectiveFinished(ObjectiveInfo _objectiveInfo, bool _succeeded)
    {
        if (_objectiveInfo.type == VariedObjectives.TYPE.BOUNTYHUNT)//srsly fuckin garbage
        {
            _objectiveInfo.panelInfo.panelText.text = "Bounty Hunt";
        }
        else if (_objectiveInfo.type == VariedObjectives.TYPE.DEFEND_STRUCTURE)
        {
            _objectiveInfo.panelInfo.panelText.text = "Defend structure";
        }

        if (_succeeded)
        {
            _objectiveInfo.panelInfo.panelText.color = Color.green;
            _objectiveInfo.panelInfo.panelText.text += "\nSuccess!";
            //Update UI
            SucceededObjectiveGUI(ref _objectiveInfo);
        }
        else
        {
            _objectiveInfo.panelInfo.panelText.color = Color.red;
            _objectiveInfo.panelInfo.panelText.text += "\nFailed!";

            //Update UI
            FailedObjectiveGUI(ref _objectiveInfo);

        }
        Game.instance.SetRandomObjective();
    }

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

    void OnEnable()
    {
        if (Game.instance)
        {
            Game.instance.onObjectiveStarted += Game_onObjectiveStarted;
            Game.instance.onObjectiveFinished += Game_onObjectiveFinished;
            print("GUIManager events +attached+ successfully!");
        }
    }

    void OnDisable()
    {
        Game.instance.onObjectiveStarted -= Game_onObjectiveStarted;
        Game.instance.onObjectiveFinished -= Game_onObjectiveFinished;
        print("GUIManager events -detached- successfully!");
    }
}
