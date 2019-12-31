using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class GUIReticleModule
{
    [Header("Left Reticle Configuration")]
    [SerializeField]
    [Tooltip("Left Reticle")]
    private GUIReticleConfig m_leftReticle;
    [Header("Right Reticle Configuration")]

    [SerializeField]
    [Tooltip("Left Reticle")]
    private GUIReticleConfig m_rightReticle;

    [Header("Reticle Color Configuration")]
    [SerializeField]
    [Tooltip("Colors for reticles")]
    private GUIReticleColorConfig m_reticleColors;

    //Local variables
    private SpriteRenderer leftRenderer, rightRenderer;

    public GUIReticleConfig LeftReticle
    {
        get
        {
            return m_leftReticle;
        }
        private set
        {
            m_leftReticle = value;
        }
    }

    public GUIReticleConfig RightReticle
    {
        get
        {
            return m_rightReticle;
        }
        private set
        {
            m_rightReticle = value;
        }
    }

    public GUIReticleColorConfig ReticleColors
    {
        get
        {
            return m_reticleColors;
        }
        private set
        {
            m_reticleColors = value;
        }
    }

    public void SetupReticleModule()
    {
        //Setup reticles
        leftRenderer = m_leftReticle.reticleReference.GetComponent<SpriteRenderer>();
        rightRenderer = m_rightReticle.reticleReference.GetComponent<SpriteRenderer>();

        //Setup renderer sprite
        leftRenderer.sprite = m_leftReticle.reticleSprite;
        rightRenderer.sprite = m_rightReticle.reticleSprite;

        //Set texts and tints
        leftRenderer.color = m_leftReticle.reticleColor;
        rightRenderer.color = m_rightReticle.reticleColor;

        //Text Color
        m_leftReticle.reticleText.color = m_leftReticle.reticleTextColor;
        m_rightReticle.reticleText.color = m_rightReticle.reticleTextColor;
        
        //Set strings
        m_leftReticle.reticleText.text = m_leftReticle.reticleString;
        m_rightReticle.reticleText.text = m_rightReticle.reticleString;

        //Set initial size
        m_leftReticle.initialReticleSize = m_leftReticle.reticleSize;
        m_rightReticle.initialReticleSize = m_rightReticle.reticleSize;

        //Set size delta
        m_leftReticle.sizeDelta = m_leftReticle.reticleInterestSize - m_leftReticle.initialReticleSize;
        m_rightReticle.sizeDelta = m_rightReticle.reticleInterestSize - m_rightReticle.initialReticleSize;
    }

    public void SetupReticleColors()
    {
        m_reticleColors.SetupLayerColors();
    }

    public void UpdateScale()
    {
        //Update scale function
        m_leftReticle.UpdateScale();
        m_rightReticle.UpdateScale();
    }




}
