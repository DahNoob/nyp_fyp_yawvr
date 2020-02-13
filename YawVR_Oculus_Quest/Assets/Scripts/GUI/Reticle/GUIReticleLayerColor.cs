using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class provides the variables for colors using tags needed for initialization in the UnityEditor.
/// </summary>
[System.Serializable]
public class GUIReticleLayerColor
{
    [SerializeField]
    private LayerMask m_layerMask;
    [SerializeField]
    [ColorUsage(true, true)]
    private Color m_layerColor;

    /// <summary>
    /// Returns the layerMask.
    /// </summary>
    // Get functions, just in case I need them later
    public LayerMask layerMask
    {
        get
        {
            return m_layerMask;
        }
        private set
        {
            m_layerMask = value; 
        }
    }

    /// <summary>
    /// Returns the layerColor
    /// </summary>
    public Color layerColor
    {
        get
        {
            return m_layerColor;
        }
        private set
        {
            m_layerColor = value;
        }
    }

}
