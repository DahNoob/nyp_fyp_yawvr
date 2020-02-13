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

/// <summary>
/// Abstract class that subsequent modules classes would inherit from.
/// </summary>
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

    /// <summary>
    /// Abstract function for calls of Activate by sub classes
    ///  To be called when the module is first used/pressed/activated/derp
    /// </summary>
    /// <param name="_controller">Which controller is controlling this action</param>
    /// <returns></returns>
    abstract public bool Activate(OVRInput.Controller _controller);

    /// <summary>
    /// Abstract function for calls of Hold by sub classes
    /// To be called while the module is activated/in usage
    /// </summary>
    /// <param name="_controller">Which controller is controlling this action</param>
    /// <returns></returns>
    abstract public bool Hold(OVRInput.Controller _controller);

    /// <summary>
    /// Abstract function for calls of Stop by sub classes
    /// To be called once the module is deactivated
    /// </summary>
    /// <param name="_controller">Which controller is controlling this action</param>
    /// <returns></returns>
    abstract public bool Stop(OVRInput.Controller _controller);

    /// <summary>
    /// Get this module's icon for UI purposes.
    /// </summary>
    /// <returns>Module Icon Sprite</returns>
    public Sprite GetIcon()
    {
        return m_moduleIcon;
    }
}
