using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class GUIReticleModule
{
    [System.Serializable]
    public struct GUIReticleConfig
    {
        [Tooltip("Reticle")]
        public GameObject m_reticleReference;

        [Tooltip("Reticle Text")]
        public TextMesh m_reticleText;

        [Tooltip("Reticle String")]
        public string m_reticleString;

        [Tooltip("Reticle Color")]
        [ColorUsage(true, true)]
        public Color m_reticleColor;

        [Tooltip("Reticle Text Color")]
        [ColorUsage(true, true)]
        public Color m_reticleTextColor;

        [Tooltip("Reticle Size")]
        public float reticleSize;

    }

    [Header("Left Reticle Configuration")]
    [SerializeField]
    [Tooltip("Left Reticle")]
    private GUIReticleConfig leftReticle;
    [Header("Right Reticle Configuration")]

    [SerializeField]
    [Tooltip("Left Reticle")]
    private GUIReticleConfig rightReticle;


    public GUIReticleConfig LeftReticle
    {
        get
        {
            return leftReticle;
        }
        private set
        {

        }
    }

    public GUIReticleConfig RightReticle
    {
        get
        {
            return rightReticle;
        }
        private set
        {

        }
    }


    //Local variables
    SpriteRenderer leftRenderer, rightRenderer;

    public void SetupReticleModule()
    {
        //Setup reticles
        leftRenderer = leftReticle.m_reticleReference.GetComponent<SpriteRenderer>();
        rightRenderer = rightReticle.m_reticleReference.GetComponent<SpriteRenderer>();

        //Set texts and tints
        leftRenderer.color = leftReticle.m_reticleColor;
        rightRenderer.color = leftReticle.m_reticleColor;

        //Text Color
        leftReticle.m_reticleText.color = leftReticle.m_reticleTextColor;
        rightReticle.m_reticleText.color = rightReticle.m_reticleTextColor;

        //Set strings
        leftReticle.m_reticleText.text = leftReticle.m_reticleString;
        rightReticle.m_reticleText.text = rightReticle.m_reticleString;
    }




}
