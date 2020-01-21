using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    [Header("Object Pool Resources")]
    [SerializeField]
    private UnityEngine.UI.Text m_projectileText;
    [SerializeField]
    private UnityEngine.UI.Text m_projectileText2;

    [Header("Weapon Info Resources")]
    [SerializeField]
    private GUIWeaponInfo m_weaponInfo;

    [Header("UI Panels")]
    [SerializeField]
    private RectTransform m_objectiveListPanel;


    //Local variables
    int frameCount = 0;
    float dt = 0.0f;
    float fps = 0.0f;
    float updateRate = 4.0f;  // 4 updates per sec.
    private Transform objectiveTarget;

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

        m_projectileText.text = ObjectPooler.instance.AmountActive(PoolObject.OBJECTTYPES.LIGHT_MECH2);
        m_projectileText2.text = ObjectPooler.instance.AmountActive(PoolObject.OBJECTTYPES.PLAYER_PROJECTILE_IMPACT);

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
            if (objectiveTarget == null)
                m_objectiveArrow.gameObject.SetActive(false);
            else
            {
                Vector3 displacement = Vector3.Scale(objectiveTarget.position - PlayerHandler.instance.transform.position, new Vector3(1, 0, 1));
                Vector3 bap = displacement.normalized;
                float lol = Mathf.Atan2(bap.x, bap.z) * Mathf.Rad2Deg;
                m_objectiveArrow.localPosition = Vector3.zero;
                m_objectiveArrow.eulerAngles = new Vector3(0, lol, 0);
                m_objectiveArrow.Rotate(90, 0, 0);
                m_objectiveArrow.Translate(0, Mathf.Min(0.14f, displacement.sqrMagnitude * 0.00005f), 0);
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

        if (Input.GetKeyDown(KeyCode.L))
        {
            SpawnLightMech1();
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            SpawnHeavyMech2();
        }
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
    public void SetActiveObjective(Transform _target)
    {
        if (_target)
            m_objectiveArrow.gameObject.SetActive(true);
        objectiveTarget = _target;
        //for (int i = 0; i < objectiveArrow.Length; ++i)
        //{
        //    objectiveArrow[i].gameObject.SetActive(_index == i);
        //}
    }
    public void AddObjectiveToPanel(ref ObjectiveInfo _objectiveInfo)
    {
        Text panel = Instantiate(m_objectiveTextPanelPrefab, m_objectiveListPanel.transform).GetComponent<Text>();
        _objectiveInfo.m_panelUI = panel;
        if(_objectiveInfo.type == VariedObjectives.TYPE.BOUNTYHUNT)//hhhhhhhhhhhhh this is such garbage code fajfjofifaoopzpovzxda
        {
            panel.text = "- Bounty Hunt";
        }
        else if(_objectiveInfo.type == VariedObjectives.TYPE.DEFEND_STRUCTURE)
        {
            panel.text = "- Defend structure";
        }
    }


    #region Reticles

    public void SetReticleInformation(OVRInput.Controller _controller, Vector3 hitPoint, GameObject hitObject, bool useTag = true)
    {
        //Set the reticle position based on raycasted position
        SetReticlePosition(_controller, hitPoint);
        //Set hit object name by accessing the object
        SetHitObjectName(_controller, hitObject);
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
        float m_maxWeaponAmmo)
    {
        GUIWeaponInfoConfig weaponInfo = _controller == OVRInput.Controller.RTouch ? m_weaponInfo.rightWeaponInfo : m_weaponInfo.leftWeaponInfo;

        if (m_weaponSprite != null)
            weaponInfo.weaponSprite.sprite = m_weaponSprite;

        weaponInfo.weaponNameText.text = m_weaponName;
        //Format the string
        weaponInfo.weaponAmmoText.text = m_currWeaponAmmo.ToString() + "/" + m_maxWeaponAmmo.ToString();
    }
    
    public void ReloadGunUI(OVRInput.Controller _controller)
    {
        GUIWeaponInfoConfig weaponInfo = _controller == OVRInput.Controller.RTouch ? m_weaponInfo.rightWeaponInfo : m_weaponInfo.leftWeaponInfo;


    }

    public void SetWeaponInfoAmmo(OVRInput.Controller _controller, float m_currAmmo, float m_maxAmmo)
    {
        GUIWeaponInfoConfig weaponInfo = _controller == OVRInput.Controller.RTouch ? m_weaponInfo.rightWeaponInfo : m_weaponInfo.leftWeaponInfo;
        //Format the string
        weaponInfo.weaponAmmoText.fontSize = 18;
        weaponInfo.weaponAmmoText.text = m_currAmmo.ToString() + "/" + m_maxAmmo.ToString();
    }

    public void SetWeaponInfoReloading(OVRInput.Controller _controller)
    {
        GUIWeaponInfoConfig weaponInfo = _controller == OVRInput.Controller.RTouch ? m_weaponInfo.rightWeaponInfo : m_weaponInfo.leftWeaponInfo;
        weaponInfo.weaponAmmoText.fontSize = 13;
        StartCoroutine(StartReloadingTextAnimation(weaponInfo));
    }

    //Going to cheese it by adding two coroutines
    IEnumerator StartReloadingTextAnimation(GUIWeaponInfoConfig weaponInfo)
    {
        weaponInfo.weaponAmmoText.text = "";
        foreach(char letter in "Reloading...")
        {
            weaponInfo.weaponAmmoText.text += letter;
            yield return new WaitForSeconds(0.05f);
        }
    }
    #endregion

    private void Game_onObjectiveStarted(ObjectiveInfo _objectiveInfo)
    {
        throw new System.NotImplementedException();
    }

    private void Game_onObjectiveFinished(ObjectiveInfo _objectiveInfo, bool _succeeded)
    {
        throw new System.NotImplementedException();
    }

    void OnEnable()
    {
        if(Game.instance)
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
