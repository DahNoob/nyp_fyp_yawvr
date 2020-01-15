using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************  
** Name: Mech Base Weapon
** Desc: An abstract class that all mech weapons will inherit from (fists, guns, swords, etc)
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
abstract public class MechBaseWeapon : BaseMechModule
{
    [Header("Base Weapon Configuration")]
    [SerializeField]
    protected bool m_enabled = true;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    protected float m_followerSpeed = 0.1f;
    [SerializeField]
    protected OVRInput.Controller m_controller;
    [SerializeField]
    protected float m_fadeInThreshold = 2.0f;
    [SerializeField]
    protected float m_fadeOutThreshold = -2.0f;

    [Header("Arm Object")]
    [SerializeField]
    protected GameObject m_armObject;

    [Header("Holo Object")]
    [SerializeField]
    protected GameObject m_holoObject;
    [SerializeField]
    public MeshRenderer[] m_holoMeshes;

    [HideInInspector]
    public GameObject armObject { get { return m_armObject; } private set { m_armObject = value; } }
    [HideInInspector]
    public GameObject holoObject { get { return m_holoObject; } private set { m_holoObject = value; } }
    [HideInInspector]
    public ControllerFollower follower { protected get; set; }
    [HideInInspector]
    public MechHandHandler mechHand { protected get; set; }

    //Local variables
    protected float fadeValue;
    protected bool isFullyVisible = false;
    protected bool isSelected = false;

    virtual public bool Selected()       // To be called when the player has selected the weapon as the current
    {
        isSelected = true;
        StartCoroutine(fadeIn());
        return true;
    }
    abstract public bool Grip();         // To be called when the player mech arm's hand-trigger is activated
    abstract public bool Ungrip();       // To be called when the player mech arm's hand-trigger is deactivated
    virtual public bool Unselected()     // To be called when the player has unselected the weapon from the current
    {
        isSelected = false;
        StartCoroutine(fadeOut());
        return true;
    }


    protected void Awake()
    {
        if (m_holoObject.transform.Find("HandReference"))
            Destroy(m_holoObject.transform.Find("HandReference").gameObject);
        if (m_armObject.transform.Find("HandReference"))
            Destroy(m_armObject.transform.Find("HandReference").gameObject);
        name = m_moduleName;
        holoObject.name = name + "Holo";
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
    protected void SetModelFade(float _val)
    {
        MeshRenderer[] ms = m_armObject.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < ms.Length; ++i)
        {
            ms[i].material.SetFloat("_Amount", _val);
        }
    }

    IEnumerator fadeIn()
    {
        m_armObject.SetActive(true);
        while (isSelected)
        {
            yield return new WaitForFixedUpdate();
            fadeValue += 0.05f;
            SetModelFade(fadeValue + m_armObject.transform.position.y);
            if(fadeValue > m_fadeInThreshold)
            {
                isFullyVisible = true;
                SetModelFade(99999);
                break;
            }
        }
        
    }
    IEnumerator fadeOut()
    {
        while (!isSelected)
        {
            yield return new WaitForFixedUpdate();
            fadeValue -= 0.05f;
            SetModelFade(fadeValue + m_armObject.transform.position.y);
            if(fadeValue < m_fadeOutThreshold)// && !isSelected)
            {
                isFullyVisible = false;
                SetModelFade(-99999);
                m_armObject.SetActive(false);
                break;
            }
        }
    }
}
