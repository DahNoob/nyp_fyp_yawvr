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
    [Tooltip("Crosshair Sprite")]
    private Sprite m_reticleSprite;

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

    [SerializeField]
    [Tooltip("Reticle Size")]
    private float m_reticleInterestSize;

    [Header("Shooting Test")]
    [SerializeField]
    [Tooltip("Max Reticle Size")]
    private float m_maxReticleSize;

    [SerializeField]
    [Tooltip("Recovery Time")]
    private float m_reticleRecoveryTime;

    //Intiail size of reticle
    private float m_initialReticleSize;
    //When the reticle has highlighted an enemy or something
    private bool m_objectInterest = false;
    //Size difference between the two
    private float m_sizeDelta;
    //Has been changed
    private bool changed = false;

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
        set
        {
            m_reticleSize = value;
        }
    }

    public Sprite reticleSprite
    {
        get
        {
            return m_reticleSprite;
        }
        private set
        {
            m_reticleSprite = value;
        }
    }

    public float reticleRecoveryTime
    {
        get
        {
            return m_reticleRecoveryTime;
        }
        private set
        {
            m_reticleRecoveryTime = value;
        }
    }

    public float maxReticleSize
    {
        get
        {
            return m_maxReticleSize;
        }
        set
        {
            m_maxReticleSize = value;
        }
    }

    public float initialReticleSize
    {
        get
        {
            return m_initialReticleSize;
        }
        set
        {
            m_initialReticleSize = value;
        }
    }

    public float reticleInterestSize
    {
        get
        {
            return m_reticleInterestSize;
        }
        set
        {
            m_reticleInterestSize = value;
        }
    }

    public bool objectInterest
    {
        get
        {
            return m_objectInterest;
        }
        set
        {
            m_objectInterest = value;
        }
    }

    public float sizeDelta
    {
        get
        {
            return m_sizeDelta;
        }
        set
        {
            m_sizeDelta = value;
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

    //Reset the reticle default size
    public void SetReticleDefaultSize()
    {
        m_reticleSize = m_initialReticleSize;
    }

    //Triggered this reticle
    public void Triggered()
    {
        //if the object is interest, then change size accordingly   
        m_reticleSize = m_objectInterest ? m_maxReticleSize + sizeDelta : m_maxReticleSize;
    }

    public void UpdateScale()
    {
        float initialSize = objectInterest ? m_initialReticleSize + sizeDelta : m_initialReticleSize;
        Debug.Log(initialSize);
        if (m_reticleSize != initialSize)
        {
            m_reticleSize -= Time.deltaTime * m_reticleRecoveryTime;
            m_reticleSize = Mathf.Max(m_reticleSize, initialSize);
        }
    }

    public void ObjectOfInterest(bool returnValue)
    {
        if(objectInterest != returnValue)
        {
            objectInterest = returnValue;
            m_reticleSize = objectInterest ? m_reticleInterestSize : m_initialReticleSize; 
        }
    }

}
