using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class provides a clear easy way to initialize values and customise reticles in the Unity Inspector with proper headings and tooltips.
/// </summary>
[Serializable]
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
    [HideInInspector]
    private TextMesh m_reticleText;

    [SerializeField]
    [Tooltip("Reticle String")]
    [HideInInspector]
    private string m_reticleString;

    [SerializeField]
    [Tooltip("Reticle Color")]
    [ColorUsage(true, true)]
    private Color m_reticleColor;

    [SerializeField]
    [Tooltip("Reticle Text Color")]
    [ColorUsage(true, true)]
    [HideInInspector]
    private Color m_reticleTextColor;

    [SerializeField]
    [Tooltip("Reticle Size")]
    private float m_reticleSize;

    [SerializeField]
    [Tooltip("Reticle Size")]
    private float m_reticleInterestSize;

    [SerializeField]
    [Tooltip("How fast does the reticle shrink in size")]
    private float m_reticleRecoveryTimeScale;

    [SerializeField]
    [Tooltip("Time for the animation to finish")]
    private float m_reticleRecoveryTime = 1;

    //Local variables
    //When the reticle has highlighted an enemy or something
    private bool m_objectInterest = false;
    //Size difference between the two
    private float m_sizeDelta;
    //Initial reticleSize to calculate the size delta
    private float m_initialReticleSize;
    //The timer for the ease
    private float m_reticleRecoveryTimer = 0;

    //This is a variable to store the reticle size before tween
    private float m_reticleSizeBeforeEase = 0;


    //Get functions
    /// <summary>
    /// Returns the reticleReference
    /// </summary>
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

    /// <summary>
    /// Returns reticleText[no longer used]
    /// </summary>
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

    /// <summary>
    /// Returns the reticleString
    /// </summary>
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

    /// <summary>
    /// Returns the current reticleColor
    /// </summary>
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

    /// <summary>
    /// Returns the reticle's text color
    /// </summary>
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

    /// <summary>
    /// Returns the reticleSize
    /// </summary>
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

    /// <summary>
    /// Returns the current reticle sprite
    /// </summary>
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

    /// <summary>
    /// Returns the reticleInterestSize
    /// </summary>
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

    /// <summary>
    /// Returns whether the cursor is highlighting an object of interest (an enemy for example)
    /// </summary>
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

    /// <summary>
    /// Size difference between normal reticle size and interest size
    /// </summary>
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

    /// <summary>
    /// Returns initial reticle size
    /// </summary>
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

    /// <summary>
    /// Returns reticle's recovery time scale.
    /// </summary>
    public float reticleRecoveryTimeScale
    {
        get
        {
            return m_reticleRecoveryTimeScale;
        }
        private set
        {
            m_reticleRecoveryTimeScale = value;
        }
    }

    /// <summary>
    /// Sets the reticleColor to be resultColor.
    /// </summary>
    /// <param name="resultColor">Color to be used as reticle's new color.</param>
    public void SetReticleColor(Color resultColor)
    {
        if (m_reticleReference != null)
            m_reticleReference.GetComponent<SpriteRenderer>().color = resultColor;
    }

    //Sets back reticle to default color
    /// <summary>
    /// Set reticle back to default color.
    /// </summary>
    public void SetReticleDefaultColor()
    {
        if (m_reticleReference != null)
            m_reticleReference.GetComponent<SpriteRenderer>().color = m_reticleColor;
    }

    //Variables for ease
    private float toInterestSize;
    private float toInitialSize;

    public void ObjectOfInterest(bool comparisonValue)
    {
        if (m_objectInterest != comparisonValue)
        {
            m_objectInterest = comparisonValue;
            //Any one time sets can be done here
            //Reset the timer
            m_reticleRecoveryTimer = 0;
            //Set these stuff
            m_reticleSizeBeforeEase = m_reticleSize;

            //if its of interest
            if (comparisonValue)
                toInterestSize = m_reticleInterestSize - m_reticleSize;
            else
                toInitialSize = m_initialReticleSize - m_reticleSize;       
        }
    }

    /// <summary>
    /// Updates the easing function sfor the reticle
    /// </summary>
    public void UpdateEase()
    {
        if (m_reticleSizeBeforeEase == 0)
            return;

        if (m_reticleRecoveryTimer < m_reticleRecoveryTime)
        {
            m_reticleRecoveryTimer += Time.deltaTime * m_reticleRecoveryTimeScale;
            m_reticleRecoveryTimer = Mathf.Min(m_reticleRecoveryTimer, m_reticleRecoveryTime);

            m_reticleSize = m_objectInterest
                ? Easing.OutBack(m_reticleRecoveryTimer, m_reticleSizeBeforeEase, toInterestSize, m_reticleRecoveryTime)
                : Mathf.Lerp(m_reticleSize, m_initialReticleSize, m_reticleRecoveryTimer);
        }
    }

}
