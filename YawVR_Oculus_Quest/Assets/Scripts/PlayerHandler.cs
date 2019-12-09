using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVR;

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
    [Header("Configuration")]
    [SerializeField]
    private Color m_armInnerColor;
    [SerializeField]
    private Color m_armRimColor;

    void Awake()
    {
        if (instance == null)
            instance = this;
        print("PlayerHandler awake!");
    }

    void Start()
    {
        print("PlayerHandler started!");
    }
    
    // Update is called once per frame
    void Update()
    {
        //if (OVRInput.GetDown(OVRInput.Button.One))
        //{
        //    OVRManager.display.RecenterPose();
        //}
        //if (OVRInput.GetDown(OVRInput.Button.Three))
        //{
        //    Instantiate(dumbCubesPrefab).transform.SetParent(GameObject.Find("CubePile").transform);
        //}
        //if(OVRInput.GetDown(OVRInput.Button.Four))
        //{
        //    foreach (Transform item in GameObject.Find("CubePile").transform)
        //    {
        //        Destroy(item.gameObject);
        //    }
        //}
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
