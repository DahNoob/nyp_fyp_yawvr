using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GUIWeaponInfo 
{
    [Header("Left Weapon Info Configuration")]
    [SerializeField]
    [Tooltip("Left Weapon Info")]
    private GUIWeaponInfoConfig m_leftWeaponInfo;

    [Header("Right Weapon Info Configuration")]
    [SerializeField]
    [Tooltip("Right Weapon Info")]
    private GUIWeaponInfoConfig m_rightWeaponInfo;

    public GUIWeaponInfoConfig leftWeaponInfo
    {
        get
        {
            return m_leftWeaponInfo;
        }
        set
        {
            m_leftWeaponInfo = value;
        }
    }

    public GUIWeaponInfoConfig rightWeaponInfo
    {
        get
        {
            return m_rightWeaponInfo;
        }
        set
        {
            m_rightWeaponInfo = value;
        }
    }

}
