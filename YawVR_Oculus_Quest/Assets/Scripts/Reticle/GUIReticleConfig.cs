using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GUIReticleConfig
{
    [SerializeField]
    [Tooltip("Reticle")]
    private GameObject m_reticleReference;

    [SerializeField]
    [Tooltip("Reticle Text")]
    private TextMesh m_reticleText;

    [SerializeField]
    [Tooltip("Reticle String")]
    private string m_reticleString;

    [SerializeField]
    [Tooltip("Reticle Color")]
    [ColorUsage(true, true)]
    private Color m_reticleColor;

    [SerializeField]
    [Tooltip("Reticle Text Color")]
    [ColorUsage(true, true)]
    private Color m_reticleTextColor;

    [SerializeField]
    [Tooltip("Reticle Size")]
    private float m_reticleSize;

    //Get functions
    public GameObject reticleReference
    {
        get
        {
            return m_reticleReference;
        }
        private set
        {
            m_reticleReference = value;
        }
    }

    public TextMesh reticleText
    {
        get
        {
            return m_reticleText;
        }
        private set
        {
            m_reticleText = value;
        }
    }

    public string reticleString
    {
        get
        {
            return m_reticleString;
        }
        private set
        {
            m_reticleString = value;
        }
    }

    public Color reticleColor
    {
        get
        {
            return m_reticleColor;
        }
        private set
        {
            m_reticleColor = value;
        }
    }

    public Color reticleTextColor
    {
        get
        {
            return m_reticleTextColor;
        }
        private set
        {
            m_reticleTextColor = value;
        }
    }

    public float reticleSize
    {
        get
        {
            return m_reticleSize;
        }
        private set
        {
            m_reticleSize = value;
        }
    }

    public void SetReticleColor(Color resultColor)
    {
        if (m_reticleReference != null)
            m_reticleReference.GetComponent<SpriteRenderer>().color = resultColor;
    }

    //Sets back reticle to default color
    public void SetReticleDefaultColor()
    {
        if (m_reticleReference != null)
            m_reticleReference.GetComponent<SpriteRenderer>().color = m_reticleColor;
    }
}
