using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************  
** Name: Mech Arm Module
** Desc: An abstract class that the arm-related "customisable" mech parts will inherit from (fists, hand-gun-cannon-thing, sword, etc)
** Author: DahNoob
** Date: 06/12/2019, 2:35 PM
**************************
** Change History
**************************
** PR   Date                    Author    Description 
** --   --------                -------   ------------------------------------
** 1    06/12/2019, 2:35PM      DahNoob   Created
*******************************/
[System.Serializable]
abstract public class MechArmModule : BaseMechModule
{
    [Header("Arm Configuration")]
    [SerializeField]
    protected bool m_rightEnabled = true;
    [SerializeField]
    protected bool m_leftEnabled = true;

    [Header("Arm Objects")]
    [SerializeField]
    protected GameObject m_rightArmObject;
    [SerializeField]
    protected GameObject m_leftArmObject;

    [Header("Holo Objects")]
    [SerializeField]
    protected GameObject m_rightHoloObject;
    [SerializeField]
    protected GameObject m_leftHoloObject;

    [HideInInspector]
    public GameObject armObject { get; private set; }
    [HideInInspector]
    public GameObject holoObject { get; private set; }
    [HideInInspector]
    public GameObject armModel { get; private set; }
    [HideInInspector]
    public MeshRenderer holoModel { get; private set; }

    protected void Start()
    {
        if (m_rightHoloObject && m_rightHoloObject.transform.Find("HandReference"))
        {
            Destroy(m_rightHoloObject.transform.Find("HandReference").gameObject);
        }
        if (m_leftHoloObject && m_leftHoloObject.transform.Find("HandReference"))
        {
            Destroy(m_leftHoloObject.transform.Find("HandReference").gameObject);
        }
    }

    //Initialises the arm module so it sets the armObject and holoObject to the corresponding controller whilst deleting the opposing ones
    public MechArmModule InitController(OVRInput.Controller _controller)
    {
        if(_controller.Equals(OVRInput.Controller.LTouch))
        {
            Destroy(m_rightHoloObject);
            Destroy(m_rightArmObject);
            holoObject = m_leftHoloObject;
            armObject = m_leftArmObject;
        }
        else
        {
            Destroy(m_leftHoloObject);
            Destroy(m_leftArmObject);
            holoObject = m_rightHoloObject;
            armObject = m_rightArmObject;
        }
        name = m_moduleName;
        holoObject.name = name + "Holo";
        holoModel = holoObject.transform.Find("Model").GetComponent<MeshRenderer>();
        //armObject.name = name + "Arm";
        return this;
    }

    public GameObject GetRightArmObject()
    {
        return m_rightArmObject;
    }
    public GameObject GetLeftArmObject()
    {
        return m_leftArmObject;
    }
    public GameObject GetRightHoloObject()
    {
        return m_rightHoloObject;
    }
    public GameObject GetLeftHoloObject()
    {
        return m_leftHoloObject;
    }
}
