using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GUIReticleTagColor
{
    [SerializeField]
    private string m_tag;
    [SerializeField]
    [ColorUsage(true, true)]
    private Color m_tagColor;

    // Get functions, just in case I need them later
    public string tag
    {
        get
        {
            return m_tag;
        }
        private set
        {
            m_tag = value; 
        }
    }
    public Color tagColor
    {
        get
        {
            return m_tagColor;
        }
        private set
        {
            m_tagColor = value;
        }
    }

}
