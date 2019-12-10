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
    ControllerFollower m_armFollower;
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
    private bool isAttached, isHandTriggered, isIndexTriggered = false;
    private MeshRenderer currentHoloArm;
    private GameObject currentArmObject;
    private List<MechArmModule> modules = new List<MechArmModule>();
    private int currModuleIndex = 0;
    private float currCanvasUIRotation = 0.0f;

    //Constant variables
    //private float ARM_MINSPEED;

    void Awake()
    {
        print("PilotController " + (m_controller == OVRInput.Controller.LTouch ? "L" : "R") + " awake!");
    }
    void Start()
    {
        //ARM_MINSPEED = m_armFollower.m_followSpeed;
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

        if (isHandTriggered && isIndexTriggered)
            modules[currModuleIndex].Hold(m_controller);
    }
    void FixedUpdate()
    {
        if (!isAttached)
            return;
        Color prevArmInnerColor = currentHoloArm.material.GetColor("_InnerColor");
        Color prevArmRimColor = currentHoloArm.material.GetColor("_RimColor");
        Color newArmInnerColor, newArmRimColor;
        if (isHandTriggered)
        {
            newArmInnerColor = Color.Lerp(prevArmInnerColor, PlayerHandler.instance.GetArmInnerColor(), 0.1f);
            newArmRimColor = Color.Lerp(prevArmRimColor, PlayerHandler.instance.GetArmRimColor(), 0.1f);
        }
        else
        {
            newArmInnerColor = Color.Lerp(prevArmInnerColor, Persistent.instance.COLOR_TRANSPARENT, 0.1f);
            newArmRimColor = Color.Lerp(prevArmRimColor, Persistent.instance.COLOR_TRANSPARENT, 0.1f);
        }
        currentHoloArm.material.SetColor("_InnerColor", newArmInnerColor);
        currentHoloArm.material.SetColor("_RimColor", newArmRimColor);

        //m_armFollower.m_followSpeed = Mathf.Lerp(m_armFollower.m_followSpeed, isIndexTriggered ? m_armMaxSpeed : ARM_MINSPEED, 0.15f);

        currCanvasUIRotation = Mathf.LerpAngle(currCanvasUIRotation, (float)(currModuleIndex) / (float)(modules.Count) * 360.0f + 90, 0.08f);

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
                armObject.transform.SetParent(m_armFollower.transform);
                armObject.transform.localPosition = Vector3.zero;
                armObject.transform.localRotation = Quaternion.identity;
                armModuleAgain.follower = m_armFollower;
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

    void SetCurrentModule(int _index)
    {
        int prevModuleIndex = currModuleIndex;
        currModuleIndex = _index;
        if (currModuleIndex >= modules.Count)
            currModuleIndex = 0;
        else if (currModuleIndex < 0)
            currModuleIndex = modules.Count; //very shitty way of doin it rn but wuteva
        currentHoloArm = modules[currModuleIndex].holoModel;
        currentArmObject = modules[currModuleIndex].armObject;
        if(isHandTriggered && isIndexTriggered)
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
            modules[i].holoModel.material.SetColor("_InnerColor", Persistent.instance.COLOR_TRANSPARENT);
            modules[i].holoModel.material.SetColor("_RimColor", Persistent.instance.COLOR_TRANSPARENT);
            modules[i].armObject.SetActive(false);
        }
        currentArmObject.SetActive(true);
    }

    void HandStateChange(bool _isTriggered)
    {
        isHandTriggered = m_armFollower.m_enabled = _isTriggered;
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
            VibrationManager.SetControllerVibration(m_controller, 20, 2, 200);
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

}
