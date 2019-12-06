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
*******************************/
[System.Serializable]
abstract public class BaseMechModule : MonoBehaviour
{
    [Header("Base Configuration")]
    [SerializeField]
    protected string m_moduleName;
    [SerializeField]
    protected Sprite m_moduleIcon;
}
