using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GUIReticleLayerColor
{
    [SerializeField]
    private LayerMask m_layerMask;
    [SerializeField]
    [ColorUsage(true, true)]
    private Color m_layerColor;

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
