using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************  
** Name: Pilot Controller Behaviour
** Desc: Detect player hands in control holes
** Author: DahNoob
** Date: 27/11/2019, 5:05 PM
**************************
** Change History
**************************
** PR   Date                    Author    Description 
** --   --------                -------   ------------------------------------
** 1    27/11/2019, 5:05 PM     DahNoob   Created and implemented
** 2    02/12/2019, 2:01 PM     DahNoob   Made hologram material change depending on grab status
** 3    FORGOT AGAIN            DahNoob   Started implementation of MechArmModule aka weapon system
** 4    09/12/2019, 11:39 AM    DahNoob   Minor changes and optimisations
** 5    09/12/2019, 1:07 PM     DahNoob   Implemented usage of MechArmModules
** 6    09/12/2019, 5:30 PM     DahNoob   Refactored to accomodate for the overhauled MechArmModule system
** 7    10/12/2019, 12:08 PM    DahNoob   Refactored to accomodate for HandPivot thing
*******************************/
public class PilotController : MonoBehaviour
{
    [SerializeField]
    //The corresponding controller
    private OVRInput.Controller m_controller;

    [Header("Configuration")]
    [SerializeField]
    private float m_handTriggerBegin = 0.55f;
    [SerializeField]
    private float m_handTriggerEnd = 0.35f;
    [SerializeField]
    private float m_indexTriggerBegin = 0.55f;
    [SerializeField]
    private float m_indexTriggerEnd = 0.35f;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float m_armMaxSpeed = 0.2f;

    [Header("References")]
    [SerializeField]
    GameObject m_ringObject;
    [SerializeField]
    Transform m_holos;
    [SerializeField]
    Canvas m_armModulesCanvas;

    [Header("Offsets")]
    [SerializeField]
    Transform m_pivotOffset;
    [SerializeField]
    Transform m_ringOffset;

    //Local variables
    private ControllerFollower follower;
    [SerializeField]
    private MechHandHandler mechHand;

    private bool isAttached = false, isHandTriggered = false, isIndexTriggered = false;
    private List<MechArmModule> modules = new List<MechArmModule>();
    private int currModuleIndex = 0;
    private float currCanvasUIRotation = 0.0f;
    private Color currHoloInnerColor, currHoloRimColor;

    //Constant variables
    //private float ARM_MINSPEED;

    void Awake()
    {
        print("PilotController " + (m_controller == OVRInput.Controller.LTouch ? "L" : "R") + " awake!");
    }
    void Start()
    {
        //ARM_MINSPEED = m_armFollower.m_followSpeed;
        follower = m_controller == OVRInput.Controller.RTouch ? PlayerHandler.instance.GetRightFollower() : PlayerHandler.instance.GetLeftFollower();
        mechHand = m_controller == OVRInput.Controller.RTouch ? PlayerHandler.instance.GetRightMechHand() : PlayerHandler.instance.GetLeftMechHand();
        Attach();
        print("PilotController " + (m_controller == OVRInput.Controller.LTouch ? "L" : "R") + " started!");
    }
    void Update()
    {
        if (!isAttached)
            return;
        
        if ((isHandTriggered && OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, m_controller) < m_handTriggerEnd) ||
            (!isHandTriggered && OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, m_controller) > m_handTriggerBegin))
        {
            HandStateChange(!isHandTriggered);
        }
        if ((isIndexTriggered && OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, m_controller) < m_indexTriggerEnd) ||
            (!isIndexTriggered && OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, m_controller) > m_indexTriggerBegin))
        {
            IndexStateChange(!isIndexTriggered);
        }

        if (OVRInput.GetDown(OVRInput.Button.One, m_controller))
        {
            SetCurrentModule(currModuleIndex + 1);
        }
        else if (OVRInput.GetDown(OVRInput.Button.Two, m_controller))
        {
            SetCurrentModule(currModuleIndex - 1);
        }

        if (IsModuleActivated())
            modules[currModuleIndex].Hold(m_controller);
    }
    void FixedUpdate()
    {
        if (!isAttached)
            return;
        if (isHandTriggered)
        {
            currHoloInnerColor = Color.Lerp(currHoloInnerColor, PlayerHandler.instance.GetArmInnerColor(), 0.1f);
            currHoloRimColor = Color.Lerp(currHoloRimColor, PlayerHandler.instance.GetArmRimColor(), 0.1f);
        }
        else
        {
            currHoloInnerColor = Color.Lerp(currHoloInnerColor, Persistent.instance.COLOR_TRANSPARENT, 0.1f);
            currHoloRimColor = Color.Lerp(currHoloRimColor, Persistent.instance.COLOR_TRANSPARENT, 0.1f);
        }
        SetHoloArmMaterialColor(currModuleIndex, currHoloInnerColor, currHoloRimColor);

        //m_armFollower.m_followSpeed = Mathf.Lerp(m_armFollower.m_followSpeed, isIndexTriggered ? m_armMaxSpeed : ARM_MINSPEED, 0.15f);

        currCanvasUIRotation = Mathf.LerpAngle(currCanvasUIRotation, (float)(currModuleIndex) / (float)(modules.Count) * 360.0f + (m_controller == OVRInput.Controller.RTouch ? 90 : 90), 0.08f);

        m_armModulesCanvas.GetComponent<RectTransform>().localEulerAngles = new Vector3(270, 180, currCanvasUIRotation);
        for (int i = 0; i < modules.Count; ++i)
        {
            m_armModulesCanvas.transform.GetChild(i).GetComponent<RectTransform>().localEulerAngles = new Vector3(0, 0, -currCanvasUIRotation);
        }
    }

    public void AttachArmModules(GameObject[] _armModulePackages)
    {
        foreach (GameObject armModulePackage in _armModulePackages)
        {
            Transform validFind = armModulePackage.transform.Find(m_controller == OVRInput.Controller.RTouch ? "Right" : "Left");
            if (validFind != null)
            {
                MechArmModule armModuleAgain = (CustomUtility.IsObjectPrefab(validFind.gameObject) ? Instantiate(validFind.gameObject, transform) : validFind.gameObject).GetComponent<MechArmModule>();
                modules.Add(armModuleAgain);
                GameObject holoArm = armModuleAgain.holoObject;
                holoArm.transform.SetParent(m_holos);
                holoArm.transform.localPosition = Vector3.zero;
                holoArm.transform.localRotation = Quaternion.identity;
                GameObject armObject = armModuleAgain.armObject;
                armObject.transform.SetParent(mechHand.weaponsTransform);
                armObject.transform.localPosition = Vector3.zero;
                armObject.transform.localRotation = Quaternion.identity;
                armModuleAgain.follower = follower;
                armModuleAgain.mechHand = mechHand;
            }
            else
            {
                throw new System.Exception("Error! ArmModulePackage could not find MechArmModule!");
            }
        }
        foreach (Transform item in m_armModulesCanvas.transform)
        {
            Destroy(item.gameObject);
        }
        for (int i = 0; i < modules.Count; ++i)
        {
            float rad = (float)(i) / (float)(modules.Count) * 360.0f * Mathf.Deg2Rad;
            RectTransform modIcon = Instantiate(Persistent.instance.PREFAB_MODULE_ICON, m_armModulesCanvas.transform).GetComponent<RectTransform>();
            modIcon.localPosition = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad)) * 50.0f;
            modIcon.GetComponentInChildren<UnityEngine.UI.Image>().sprite = modules[i].GetIcon();
            modIcon.GetComponentInChildren<UnityEngine.UI.Text>().text = modules[i].name;
        }
        SetCurrentModule(0);
    }

    void VibrateCrescendo()
    {
        OVRHapticsClip clip = new OVRHapticsClip();
        for (int i = 0; i < 80; ++i)
        {
            clip.WriteSample(i % 3 == 0 ? (byte)Mathf.Min(i * 4.0f, 255) : (byte)0);
        }
        VibrationManager.SetControllerVibration(m_controller, clip);
    }

    void SetHoloArmMaterialColor(int _index, Color _newInnerColor, Color _newRimColor)
    {
        MeshRenderer[] holoMeshes = modules[_index].m_holoMeshes;
        for (int i = 0; i < holoMeshes.Length; ++i)
        {
            for (int j = 0; j < holoMeshes[i].materials.Length; ++j)
            {
                holoMeshes[i].materials[j].SetColor("_InnerColor", _newInnerColor);
                holoMeshes[i].materials[j].SetColor("_RimColor", _newRimColor);
            }
        }
    }

    void SetCurrentModule(int _index)
    {
        int prevModuleIndex = currModuleIndex;
        currModuleIndex = _index;
        if (currModuleIndex >= modules.Count)
            currModuleIndex = 0;
        else if (currModuleIndex < 0)
            currModuleIndex = modules.Count - 1; //very shitty way of doin it rn but wuteva
        if(IsModuleActivated())
        {
            modules[prevModuleIndex].Stop(m_controller);
            modules[currModuleIndex].Activate(m_controller);
        }
        ResetArmModules();
    }

    void ResetArmModules()
    {
        for (int i = 0; i < modules.Count; ++i)
        {
            //for (int j = 0; j < modules[i].holoModel.materials.Length; ++j)
            //{
            //    modules[i].holoModel.materials[j].SetColor("_InnerColor", Persistent.instance.COLOR_TRANSPARENT);
            //    modules[i].holoModel.materials[j].SetColor("_RimColor", Persistent.instance.COLOR_TRANSPARENT);
            //}
            SetHoloArmMaterialColor(i, Persistent.instance.COLOR_TRANSPARENT, Persistent.instance.COLOR_TRANSPARENT);
            modules[i].armObject.SetActive(false);
            modules[i].holoObject.SetActive(false);
            modules[i].gameObject.SetActive(false);
        }
        modules[currModuleIndex].armObject.SetActive(true);
        modules[currModuleIndex].holoObject.SetActive(true);
        modules[currModuleIndex].gameObject.SetActive(true);
    }

    void HandStateChange(bool _isTriggered)
    {
        if (!_isTriggered && IsModuleActivated())
            modules[currModuleIndex].Stop(m_controller);
        isHandTriggered = follower.m_enabled = _isTriggered;
        mechHand.SetEnabled(_isTriggered);
        GUIManager.instance.EnableReticle(m_controller, _isTriggered);
    }

    void IndexStateChange(bool _isTriggered)
    {
        isIndexTriggered = _isTriggered;
        if (isHandTriggered)
        {
            if(isIndexTriggered)
            {
                //VibrationManager.SetControllerVibration(m_controller, 16, 2, 100);
                modules[currModuleIndex].Activate(m_controller);

            }
            else
            {
                modules[currModuleIndex].Stop(m_controller);
            }
        }
    }

    public bool IsModuleActivated()
    {
        return isHandTriggered && isIndexTriggered;
    }

    void OnTriggerEnter(Collider otherCollider)
    {
        // Get the grab trigger
        //print("ASD");
        if (isAttached)
            return;
        OVRGrabber grabber = otherCollider.GetComponent<OVRGrabber>() ?? otherCollider.GetComponentInParent<OVRGrabber>();
        if (grabber && grabber.GetController() == m_controller)
        {
            isAttached = true;
            //this.grabber = grabber;
            //VibrationManager.SetControllerVibration(m_controller, 20, 2, 200);
            VibrationManager.SetControllerVibration(m_controller, 0.01f, 0.4f);
            transform.Find("AnchorPivot").localPosition = m_pivotOffset.localPosition;
            transform.Find("AnchorPivot").localRotation = m_pivotOffset.localRotation;
            m_ringObject.transform.localPosition = m_ringOffset.localPosition;
            m_ringObject.transform.localRotation = m_ringOffset.localRotation;
            transform.SetParent(grabber.transform);
            //transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
    }

    void Attach()
    {
        if (isAttached)
            return;
        {
            isAttached = true;
            //this.grabber = grabber;
            //VibrationManager.SetControllerVibration(m_controller, 20, 2, 200);
            VibrationManager.SetControllerVibration(m_controller, 0.01f, 0.4f);
            transform.Find("AnchorPivot").localPosition = m_pivotOffset.localPosition;
            transform.Find("AnchorPivot").localRotation = m_pivotOffset.localRotation;
            m_ringObject.transform.localPosition = m_ringOffset.localPosition;
            m_ringObject.transform.localRotation = m_ringOffset.localRotation;
            transform.SetParent(m_controller == OVRInput.Controller.RTouch ? PlayerHandler.instance.rightHand.transform : PlayerHandler.instance.leftHand.transform);
            //transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
    }
}
