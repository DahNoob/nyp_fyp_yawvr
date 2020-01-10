using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PlayerUIMinimap
{
    [Header("Minimap Configuration")]
    [SerializeField]
    [Tooltip("Allow animations to be handled")]
    private bool doAnimations = true;
    [SerializeField]
    [Tooltip("The camera rendering this minimap")]
    private Camera m_minimapCamera;
    [SerializeField]
    private float m_minimapOffset;
    [SerializeField] [Range(0,40)]
    private float m_minimapZoom;

    [Header("Inner Ring Configuration")]
    [SerializeField]
    [Tooltip("The inner ring object")]
    private Image m_innerRing;
    [SerializeField]
    [Tooltip("Inner Ring Rotation Speed")]
    private float m_innerRingRotationSpeed;
    [SerializeField]
    [Tooltip("Direction to rotate")]
    private bool m_innerRingRotateClockwise;

    [Header("Outer Ring Configuration")]
    [SerializeField]
    [Tooltip("The inner ring object")]
    private Image m_outerRing;
    [SerializeField]
    [Tooltip("Inner Ring Rotation Speed")] //Lerps towards the desired rotation
    private float m_outerRingRotationSpeed;


    //Local variables
    private Transform m_playerReference;
    [SerializeField]
    private RectTransform m_innerRingTransform;
    [SerializeField]
    private RectTransform m_outerRingTransform;


    // Start is called before the first frame update
    public void Start()
    {
        //Assign the players reference
        m_playerReference = PlayerHandler.instance.transform;
    }

    // Update is called once per frame
    public void Update()
    {
        bool result = doAnimations ? AnimatedMinimap() : NonAnimatedMinimap();

        //Calculate the offsets for minimaps and stuff
        Vector3 cameraPosition = m_minimapCamera.transform.localPosition;
        cameraPosition.y = m_minimapOffset;
        m_minimapCamera.transform.localPosition = cameraPosition;

        //Size of camera
        m_minimapCamera.orthographicSize = m_minimapZoom;

    }
    bool AnimatedMinimap()
    {

        //Apply rotations to the inner ring on the z-axis
        if (m_innerRing != null)
        {
            float innerRotationResult = m_innerRingRotateClockwise ? -m_innerRingRotationSpeed : m_outerRingRotationSpeed;
            m_innerRingTransform.Rotate(Vector3.forward * Time.smoothDeltaTime * innerRotationResult);
        }
        
        if(m_outerRing != null)
        {
            //m_innerRingTransform.Rotate()
        }

        return true;
    }
    bool NonAnimatedMinimap()
    {
        //Do not even attempt to spin the inner ring.

        return true;
    }


}
