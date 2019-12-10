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
** 2    09/12/2019, 5:14PM      DahNoob   Overhauled the variables and logic such that it should be implemented to one script per arm now
*******************************/
[System.Serializable]
abstract public class MechArmModule : BaseMechModule
{
    [Header("Arm Configuration")]
    [SerializeField]
    protected bool m_enabled = true;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    protected float m_followerSpeed = 0.1f;
    [SerializeField]
    protected OVRInput.Controller m_controller;

    [Header("Arm Object")]
    [SerializeField]
    protected GameObject m_armObject;

    [Header("Holo Object")]
    [SerializeField]
    protected GameObject m_holoObject;

    [HideInInspector]
    public GameObject armObject { get { return m_armObject; } private set { m_armObject = value; } }
    [HideInInspector]
    public GameObject holoObject { get { return m_holoObject; } private set { m_holoObject = value; } }
    //[HideInInspector]
    //public GameObject armModel { get; private set; }
    [HideInInspector]
    public MeshRenderer holoModel { get; private set; }
    [HideInInspector]
    public ControllerFollower follower { protected get; set; }

    protected void Awake()
    {
        if (m_holoObject.transform.Find("HandReference"))
            Destroy(m_holoObject.transform.Find("HandReference").gameObject);
        name = m_moduleName;
        holoObject.name = name + "Holo";
        holoModel = holoObject.transform.Find("Model").GetComponent<MeshRenderer>();
        armObject.name = name + "Arm";
    }

    //public MechArmModule InitController()
    //{
    //    name = m_moduleName;
    //    holoObject.name = name + "Holo";
    //    holoModel = holoObject.transform.Find("Model").GetComponent<MeshRenderer>();
    //    armObject.name = name + "Arm";
    //    //armModel = armObject.transform.Find("Model").gameObject;
    //    return this;
    //}

    public OVRInput.Controller GetController()
    {
        return m_controller;
    }
}
