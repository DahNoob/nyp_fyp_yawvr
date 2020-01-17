using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************  
** Name: Base Mech Module
** Desc: An abstract class that all the "customisable" mech parts will inherit from
** Author: DahNoob
** Date: 06/12/2019, 2:29 PM
**************************
** Change History
**************************
** PR   Date                    Author    Description 
** --   --------                -------   ------------------------------------
** 1    06/12/2019, 2:30PM      DahNoob   Created
** 2    09/12/2019, 11:12PM     DahNoob   Implemented abstract functions Activate, Hold and Stop
** 3    09/12/2019, 4:48PM      DahNoob   Implemented energy reduction
*******************************/
[System.Serializable]
abstract public class BaseMechModule : MonoBehaviour
{
    [Header("Base Configuration")]
    [SerializeField]
    protected string m_moduleName;
    [SerializeField]
    protected Sprite m_moduleIcon;
    //[SerializeField]
    //protected float m_energyReduction = 10.0f;

    abstract public bool Activate(OVRInput.Controller _controller);    // To be called when the module is first used/pressed/activated/derp
    abstract public bool Hold(OVRInput.Controller _controller);        // To be called while the module is activated/in usage
    abstract public bool Stop(OVRInput.Controller _controller);        // To be called once the module is deactivated

    public Sprite GetIcon()
    {
        return m_moduleIcon;
    }
}
